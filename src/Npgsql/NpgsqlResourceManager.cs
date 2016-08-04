#if NET45 || NET451
#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using Npgsql.Logging;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace Npgsql
{
    internal interface INpgsqlResourceManager
    {
        void Enlist(INpgsqlTransactionCallbacks transactionCallbacks, byte[] txToken);
        byte[] Promote(INpgsqlTransactionCallbacks transactionCallbacks);
        void CommitWork(string txName);
        void RollbackWork(string txName);
    }

    internal class NpgsqlResourceManager : MarshalByRefObject, INpgsqlResourceManager
    {
        private readonly Dictionary<string, CommittableTransaction> _transactions = new Dictionary<string, CommittableTransaction>();

#region INpgsqlTransactionManager Members

        public byte[] Promote(INpgsqlTransactionCallbacks callbacks)
        {
            CommittableTransaction tx = new CommittableTransaction();
            DurableResourceManager rm = new DurableResourceManager(this, callbacks, tx);
            byte[] token = TransactionInterop.GetTransmitterPropagationToken(tx);
            _transactions.Add(rm.TxName, tx);
            rm.Enlist(tx);
            return token;
        }

        public void Enlist(INpgsqlTransactionCallbacks callbacks, byte[] txToken)
        {
            DurableResourceManager rm = new DurableResourceManager(this, callbacks);
            rm.Enlist(txToken);
        }

        public void CommitWork(string txName)
        {
            CommittableTransaction tx;
            if (_transactions.TryGetValue(txName, out tx))
            {
                tx.Commit();
                _transactions.Remove(txName);
            }
        }

        public void RollbackWork(string txName)
        {
            CommittableTransaction tx;
            if (_transactions.TryGetValue(txName, out tx))
            {
                _transactions.Remove(txName);
                // you can fail to commit,
                // but you're not getting out
                // of a rollback.  Remove from list first.
                tx.Rollback();
            }
        }

#endregion

        private class DurableResourceManager : ISinglePhaseNotification
        {
            private CommittableTransaction _tx;
            private NpgsqlResourceManager _rm;

            private readonly INpgsqlTransactionCallbacks _callbacks;
            private string _txName;

            private static readonly NpgsqlLogger Log = NpgsqlLogManager.GetCurrentClassLogger();

            public DurableResourceManager(NpgsqlResourceManager rm, INpgsqlTransactionCallbacks callbacks)
                : this(rm, callbacks, null)
            {
            }

            public DurableResourceManager(NpgsqlResourceManager rm, INpgsqlTransactionCallbacks callbacks,
                                          CommittableTransaction tx)
            {
                _rm = rm;
                _tx = tx;
                _callbacks = callbacks;
            }

            public string TxName
            {
                get
                {
                    // delay initialize since callback methods may be expensive
                    if (_txName == null)
                    {
                        _txName = _callbacks.GetName();
                    }
                    return _txName;
                }
            }

#region IEnlistmentNotification Members

            public void Commit(Enlistment enlistment)
            {
                _callbacks.CommitTransaction();
                // TODO: remove record of prepared
                enlistment.Done();
                _callbacks.Dispose();
            }

            public void InDoubt(Enlistment enlistment)
            {
                // not going to happen when enlisted durably
                throw new NotImplementedException();
            }

            public void Prepare(PreparingEnlistment preparingEnlistment)
            {
                // PrepareTransaction may fail !
                if (_callbacks.PrepareTransaction())
                    preparingEnlistment.Prepared();
                else
                {
                    preparingEnlistment.ForceRollback();
                    // We should rollback manually
                    // http://blog.jordanterrell.com/post/IEnlistmentNotification-Implementation-Nuances.aspx
                    _callbacks.RollbackTransaction();
                }
            }

            public void Rollback(Enlistment enlistment)
            {
                // Everyting involving _callbacks may fail if the object was created more than 6 minutes ago.
                // http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception
                // We could'nt tell that the object lifetime is infinite, this would be a huge memory leak because we create many objects

                try
                {
                    _callbacks.RollbackTransaction();
                    _callbacks.Dispose();
                }
                catch (System.Runtime.Remoting.RemotingException re)
                {
                    if (_tx != null && _tx.TransactionInformation != null && _tx.TransactionInformation.DistributedIdentifier != null)
                        Log.Error($" The object underlying the transaction with DistributedId [{_tx.TransactionInformation.DistributedIdentifier}] has timed out.");
                    Log.Error($"Your lease has expired and your connection to that remote object is lost. The client that enlisted in the transaction is faulty.", re);
                }
                finally
                {
                    enlistment.Done();
                }
            }

#endregion

#region ISinglePhaseNotification Members

            public void SinglePhaseCommit(SinglePhaseEnlistment singlePhaseEnlistment)
            {
                _callbacks.CommitTransaction();
                singlePhaseEnlistment.Committed();
                _callbacks.Dispose();
            }

#endregion

            // A Durable resource manager should be identitifed with a same GUID across its entire lifetime
            // Npgsql is actually not a durable ressource manager as :
            // - This is a library that could be hosted in multiple process and/or appdomain. If it was a windows service this would be OK.
            // - It does not persist anything to recover a crash or restart
            // From now on a new guid is generated every time a Resource Manager is created.
            private static readonly Guid rmGuid =
                Guid.NewGuid();
            // new Guid("9e1b6d2d-8cdb-40ce-ac37-edfe5f880716");

            public Transaction Enlist(byte[] token)
            {
                return Enlist(TransactionInterop.GetTransactionFromTransmitterPropagationToken(token));
            }

            public Transaction Enlist(Transaction tx)
            {
                tx.EnlistDurable(rmGuid, this, EnlistmentOptions.None);
                return tx;
            }
        }
    }
}
#endif
