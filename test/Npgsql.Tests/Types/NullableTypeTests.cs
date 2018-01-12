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

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using Npgsql;
using NpgsqlTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests.Types
{
    public class NullableTypeTests : TestBase
    {
        [Test]
        public void NullableScalar()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                var p1 = new NpgsqlParameter { ParameterName = "p1", Value = DBNull.Value, NpgsqlDbType = NpgsqlDbType.Smallint };
                var p2 = new NpgsqlParameter { ParameterName = "p2", Value = (short)8 };
                Assert.That(p2.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Smallint));
                Assert.That(p2.DbType, Is.EqualTo(DbType.Int16));
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();

                    for (var i = 0; i < cmd.Parameters.Count; i++)
                    {
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(short)));
                        Assert.That(reader.GetDataTypeName(i), Is.EqualTo("int2"));
                    }

                    Assert.That(() => reader.GetFieldValue<object>(0), Throws.TypeOf<InvalidCastException>());
                    Assert.That(() => reader.GetFieldValue<int>(0), Throws.TypeOf<InvalidCastException>());
                    Assert.That(() => reader.GetFieldValue<int?>(0), Throws.Nothing);
                    Assert.That(reader.GetFieldValue<int?>(0), Is.Null);

                    Assert.That(() => reader.GetFieldValue<object>(1), Throws.Nothing);
                    Assert.That(() => reader.GetFieldValue<int>(1), Throws.Nothing);
                    Assert.That(() => reader.GetFieldValue<int?>(1), Throws.Nothing);
                    Assert.That(reader.GetFieldValue<object>(1), Is.EqualTo(8));
                    Assert.That(reader.GetFieldValue<int>(1), Is.EqualTo(8));
                    Assert.That(reader.GetFieldValue<int?>(1), Is.EqualTo(8));
                }
            }
        }
    }
}
