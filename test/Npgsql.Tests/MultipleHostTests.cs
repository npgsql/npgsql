#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
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

using System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using NUnit.Framework;

namespace Npgsql.Tests
{
    public class MultipleHostTests : TestBase
    {
        [Test]
        public void MultipleHostConnectMasterWithPort([Values(true, false)] bool pooling)
        {
            var csb = new NpgsqlConnectionStringBuilder(MultipleConnectionString)
            {
                TargetServerType = TargetServerType.master,
                Pooling = pooling,
            };
            using (var conn = OpenConnection(csb))
            using (var cmd = new NpgsqlCommand("SHOW transaction_read_only", conn))
            {
                var isSlave = cmd.ExecuteScalar();
                Assert.That(isSlave, Is.EqualTo("off"));
            }
        }
        [Test]
        public void MultipleHostConnectSlaveWithPort([Values(true, false)] bool pooling)
        {
            var csb = new NpgsqlConnectionStringBuilder(MultipleConnectionString)
            {
                TargetServerType = TargetServerType.slave,
                Pooling = pooling,
            };
            using (var conn = OpenConnection(csb))
            using (var cmd = new NpgsqlCommand("SHOW transaction_read_only", conn))
            {
                var isSlave = cmd.ExecuteScalar();
                Assert.That(isSlave, Is.EqualTo("on"));
            }
        }
        [Test]
        public void MultipleHostConnectPreferSlaveWithPort([Values(true, false)] bool pooling)
        {
            var csb = new NpgsqlConnectionStringBuilder(MultipleConnectionString)
            {
                TargetServerType = TargetServerType.preferSlave,
                Pooling = pooling,
            };
            using (var conn = OpenConnection(csb))
            using (var cmd = new NpgsqlCommand("SHOW transaction_read_only", conn))
            {
                var isSlave = cmd.ExecuteScalar();
                Assert.That(isSlave, Is.EqualTo("on"));
            }
        }
        [Test]
        public void MultipleHostConnectMasterWithoutPort([Values(true, false)] bool pooling)
        {
            var csb = new NpgsqlConnectionStringBuilder(MultipleConnectionStringNoPort)
            {
                TargetServerType = TargetServerType.master,
                Pooling = pooling,
            };
            using (var conn = OpenConnection(csb))
            using (var cmd = new NpgsqlCommand("SHOW transaction_read_only", conn))
            {
                var isSlave = cmd.ExecuteScalar();
                Assert.That(isSlave, Is.EqualTo("off"));
            }
        }
        [Test]
        public void MultipleHostConnectSlaveWithoutPort()
        {
            var csb = new NpgsqlConnectionStringBuilder(MultipleConnectionStringNoPort)
            {
                TargetServerType = TargetServerType.slave,
                Pooling = false,
            }.ToString();

            using (var conn = new NpgsqlConnection(csb))
            {
                Assert.That(() => conn.Open(), Throws.Exception.TypeOf<NpgsqlException>());
            }
        }
        [Test]
        public void MultipleHostConnectPreferSlaveWithoutPort([Values(true, false)] bool pooling)
        {
            var csb = new NpgsqlConnectionStringBuilder(MultipleConnectionStringNoPort)
            {
                TargetServerType = TargetServerType.preferSlave,
                Pooling = pooling,
            };
            using (var conn = OpenConnection(csb))
            using (var cmd = new NpgsqlCommand("SHOW transaction_read_only", conn))
            {
                var isSlave = cmd.ExecuteScalar();
                Assert.That(isSlave, Is.EqualTo("off"));
            }
        }
    }
}
