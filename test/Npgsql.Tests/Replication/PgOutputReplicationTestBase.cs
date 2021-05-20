using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Npgsql.Replication;
using Npgsql.Replication.PgOutput;
using Npgsql.Replication.PgOutput.Messages;
using NUnit.Framework;

namespace Npgsql.Tests.Replication
{
    public abstract class PgOutputReplicationTestBase : SafeReplicationTestBase<LogicalReplicationConnection>
    {
        protected ulong ProtocolVersion { get; }
        protected ReplicationDataMode DataMode { get; }
        protected TransactionStreamingMode TransactionMode { get; }
        protected bool IsBinaryMode => DataMode == ReplicationDataMode.BinaryReplicationData;
        protected bool IsStreaming => TransactionMode == TransactionStreamingMode.StreamingTransaction;

        bool? Binary => DataMode == ReplicationDataMode.BinaryReplicationData
            ? true
            : DataMode == ReplicationDataMode.TextReplicationData
                ? false
                : null;
        bool? Streaming => TransactionMode == TransactionStreamingMode.StreamingTransaction
            ? true
            : TransactionMode == TransactionStreamingMode.NonStreamingTransaction
                ? false
                : null;

        protected PgOutputReplicationTestBase(ProtocolVersionMode protocolVersion, ReplicationDataMode dataMode, TransactionStreamingMode transactionMode)
        {
            ProtocolVersion = (ulong)protocolVersion;
            DataMode = dataMode;
            TransactionMode = transactionMode;
        }

        protected async Task<uint?> AssertTransactionStart(IAsyncEnumerator<PgOutputReplicationMessage> messages)
        {
            Assert.True(await messages.MoveNextAsync());
            if (IsStreaming)
            {
                Assert.That(messages.Current, Is.TypeOf<StreamStartMessage>());
                var streamStartMessage = (messages.Current as StreamStartMessage)!;
                return streamStartMessage.TransactionXid;
            }
            Assert.That(messages.Current, Is.TypeOf<BeginMessage>());
            var beginMessage = (messages.Current as BeginMessage)!;
            return beginMessage.TransactionXid;
        }

        protected async Task AssertTransactionCommit(IAsyncEnumerator<PgOutputReplicationMessage> messages)
        {
            Assert.True(await messages.MoveNextAsync());
            if (IsStreaming)
            {
                Assert.That(messages.Current, Is.TypeOf<StreamStopMessage>());
                Assert.True(await messages.MoveNextAsync());
                Assert.That(messages.Current, Is.TypeOf<StreamCommitMessage>());
            }
            else
                Assert.That(messages.Current, Is.TypeOf<CommitMessage>());
        }

        protected async ValueTask<TExpected> NextMessage<TExpected>(IAsyncEnumerator<PgOutputReplicationMessage> enumerator, bool expectRelationMessage = false)
            where TExpected : PgOutputReplicationMessage
        {
            Assert.True(await enumerator.MoveNextAsync());
            if (IsStreaming && enumerator.Current is StreamStopMessage)
            {
                Assert.True(await enumerator.MoveNextAsync());
                Assert.That(enumerator.Current, Is.TypeOf<StreamStartMessage>());
                Assert.True(await enumerator.MoveNextAsync());
                if (expectRelationMessage)
                {
                    Assert.That(enumerator.Current, Is.TypeOf<RelationMessage>());
                    Assert.True(await enumerator.MoveNextAsync());
                }
            }

            Assert.That(enumerator.Current, Is.TypeOf<TExpected>());
            return (TExpected)enumerator.Current!;
        }

        /// <summary>
        /// Unfortunately, empty transactions may get randomly created by PG because of auto-vacuuming; these cause test failures as we
        /// assert for specific expected message types. This filters them out.
        /// </summary>
        protected async IAsyncEnumerable<PgOutputReplicationMessage> SkipEmptyTransactions(IAsyncEnumerable<PgOutputReplicationMessage> messages)
        {
            var enumerator = messages.GetAsyncEnumerator();
            while (await enumerator.MoveNextAsync())
            {
                if (enumerator.Current is BeginMessage)
                {
                    var current = enumerator.Current.Clone();
                    if (!await enumerator.MoveNextAsync())
                    {
                        yield return current;
                        yield break;
                    }

                    var next = enumerator.Current;
                    if (next is CommitMessage)
                        continue;

                    yield return current;
                    yield return next;
                    continue;
                }

                yield return enumerator.Current;
            }
        }

        protected PgOutputReplicationOptions GetOptions(string publicationName, bool? messages = null)
            => new(publicationName, ProtocolVersion, Binary, Streaming, messages);

        protected Task SafePgOutputReplicationTest(Func<string, string, string, Task> testAction, [CallerMemberName] string memberName = "")
            => SafeReplicationTest(testAction, GetObjectName(memberName));

        protected string GetObjectName(string memberName)
        {
            var sb = new StringBuilder(memberName)
                .Append("_v").Append(ProtocolVersion);
            if (Binary.HasValue)
                sb.Append("_b_").Append(BoolToChar(Binary.Value));
            if (Streaming.HasValue)
                sb.Append("_s_").Append(BoolToChar(Streaming.Value));
            return sb.ToString();
        }

        protected static char BoolToChar(bool value)
            => value ? 't' : 'f';


        protected override string Postfix => "pgoutput_l";

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await using var c = await OpenConnectionAsync();
            TestUtil.MinimumPgVersion(c, "10.0", "The Logical Replication Protocol (via pgoutput plugin) was introduced in PostgreSQL 10");
            if (ProtocolVersion > 1)
                TestUtil.MinimumPgVersion(c, "14.0", "Logical Streaming Replication Protocol version 2 was introduced in PostgreSQL 14");
            if (IsBinaryMode)
                TestUtil.MinimumPgVersion(c, "14.0", "Sending replication values in binary representation was introduced in PostgreSQL 14");
            if (IsStreaming)
                TestUtil.MinimumPgVersion(c, "14.0", "Streaming of in-progress transactions was introduced in PostgreSQL 14");
        }
    }

    public enum ProtocolVersionMode : ulong
    {
        ProtocolV1 = 1UL,
        ProtocolV2 = 2UL,
    }
    public enum TransactionStreamingMode
    {
        DefaultTransaction,
        NonStreamingTransaction,
        StreamingTransaction,
    }
    public enum ReplicationDataMode
    {
        DefaultReplicationData,
        TextReplicationData,
        BinaryReplicationData,
    }
}
