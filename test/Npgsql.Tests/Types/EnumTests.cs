#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Npgsql;
using Npgsql.NameTranslation;
using Npgsql.PostgresTypes;
using NpgsqlTypes;

namespace Npgsql.Tests.Types
{
    [NonParallelizable]
    class EnumTests : TestBase
    {
        enum Mood { Sad, Ok, Happy };

        [Test]
        public void UnmappedEnum()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(UnmappedEnum),
                Pooling = false
            };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.unmapped_enum AS ENUM ('sad', 'ok', 'happy')");
                conn.ReloadTypes();
                var tempSchema = conn.ExecuteScalar("SELECT nspname FROM pg_namespace WHERE oid = pg_my_temp_schema()");

                using (var cmd = new NpgsqlCommand("SELECT @scalar1, @scalar2, @scalar3, @scalar4", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter
                    {
                        ParameterName = "scalar1",
                        Value = Mood.Happy,
                        DataTypeName = $"{tempSchema}.unmapped_enum"
                    });
                    cmd.Parameters.Add(new NpgsqlParameter
                    {
                        ParameterName = "scalar2",
                        Value = "happy",
                        DataTypeName = $"{tempSchema}.unmapped_enum"
                    });
                    cmd.Parameters.Add(new NpgsqlParameter<Mood>
                    {
                        ParameterName = "scalar3",
                        TypedValue = Mood.Happy,
                        DataTypeName = $"{tempSchema}.unmapped_enum"
                    });
                    cmd.Parameters.Add(new NpgsqlParameter<string>
                    {
                        ParameterName = "scalar4",
                        TypedValue = "happy",
                        DataTypeName = $"{tempSchema}.unmapped_enum"
                    });
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();

                        for (var i = 0; i < 4; i++)
                        {
                            Assert.That(reader.GetDataTypeName(i),
                                Does.StartWith("pg_temp") & Does.EndWith(".unmapped_enum"));

                            Assert.That(reader.GetFieldValue<Mood>(i), Is.EqualTo(Mood.Happy));
                            Assert.That(reader.GetFieldValue<string>(i), Is.EqualTo("happy"));
                            Assert.That(reader.GetValue(i), Is.EqualTo("happy"));
                        }
                    }
                }
            }
        }

        [Test, Description("Resolves an enum type handler via the different pathways, with global mapping")]
        public void EnumTypeResolutionWithGlobalMapping()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(EnumTypeResolutionWithGlobalMapping),  // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.mood1 AS ENUM ('sad', 'ok', 'happy')");
                NpgsqlConnection.GlobalTypeMapper.MapEnum<Mood>("mood1");
                try
                {
                    conn.ReloadTypes();

                    // Resolve type by DataTypeName
                    using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter
                        {
                            ParameterName = "p",
                            DataTypeName = "mood1",
                            Value = DBNull.Value
                        });
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".mood1"));
                            Assert.That(reader.IsDBNull(0), Is.True);
                        }
                    }

                    // Resolve type by ClrType (type inference)
                    conn.ReloadTypes();
                    using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                    {
                        cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = Mood.Ok });
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".mood1"));
                        }
                    }

                    // Resolve type by OID (read)
                    conn.ReloadTypes();
                    using (var cmd = new NpgsqlCommand("SELECT 'happy'::MOOD1", conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".mood1"));
                    }
                }
                finally
                {
                    NpgsqlConnection.GlobalTypeMapper.UnmapEnum<Mood>("mood1");
                }
            }
        }

        [Test, Description("Resolves an enum type handler via the different pathways, with late mapping")]
        public void EnumTypeResolutionWithLateMapping()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(EnumTypeResolutionWithLateMapping),  // Prevent backend type caching in TypeHandlerRegistry
                Pooling = false
            };

            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.mood2 AS ENUM ('sad', 'ok', 'happy')");

                // Resolve type by NpgsqlDbType
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<Mood>("mood2");
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter
                    {
                        ParameterName = "p",
                        DataTypeName = "mood2",
                        Value = DBNull.Value
                    });
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".mood2"));
                        Assert.That(reader.IsDBNull(0), Is.True);
                    }
                }

                // Resolve type by ClrType (type inference)
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<Mood>("mood2");
                using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                {
                    cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Value = Mood.Ok });
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".mood2"));
                    }
                }

                // Resolve type by OID (read)
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<Mood>("mood2");
                using (var cmd = new NpgsqlCommand("SELECT 'happy'::MOOD2", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndWith(".mood2"));
                }
            }
        }

        [Test]
        public void LateMapping()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.mood3 AS ENUM ('sad', 'ok', 'happy')");
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<Mood>("mood3");
                const Mood expected = Mood.Ok;
                var cmd = new NpgsqlCommand("SELECT @p1::MOOD3, @p2::MOOD3", conn);
                var p1 = new NpgsqlParameter
                {
                    ParameterName = "p1",
                    DataTypeName = "mood3",
                    Value = expected
                };
                var p2 = new NpgsqlParameter { ParameterName = "p2", Value = expected };
                cmd.Parameters.Add(p1);
                cmd.Parameters.Add(p2);
                var reader = cmd.ExecuteReader();
                reader.Read();

                for (var i = 0; i < cmd.Parameters.Count; i++)
                {
                    Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Mood)));
                    Assert.That(reader.GetFieldValue<Mood>(i), Is.EqualTo(expected));
                    Assert.That(reader.GetValue(i), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void DualEnums()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.mood4 AS ENUM ('sad', 'ok', 'happy')");
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.test_enum AS ENUM ('label1', 'label2', 'label3')");
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<Mood>("mood4");
                conn.TypeMapper.MapEnum<TestEnum>("test_enum");
                var cmd = new NpgsqlCommand("SELECT @p1", conn);
                var expected = new[] { Mood.Ok, Mood.Sad };
                var p = new NpgsqlParameter
                {
                    ParameterName = "p1",
                    DataTypeName = "mood4[]",
                    Value = expected
                };
                cmd.Parameters.Add(p);
                var result = cmd.ExecuteScalar();
                Assert.AreEqual(expected, result);
            }
        }

        [Test]
        public void GlobalMapping()
        {
            try
            {
                using (var conn = OpenConnection())
                {
                    conn.ExecuteNonQuery("DROP TYPE IF EXISTS mood5");
                    conn.ExecuteNonQuery("CREATE TYPE mood5 AS ENUM ('sad', 'ok', 'happy')");
                    NpgsqlConnection.GlobalTypeMapper.MapEnum<Mood>("mood5");
                    conn.ReloadTypes();
                    const Mood expected = Mood.Ok;
                    using (var cmd = new NpgsqlCommand("SELECT @p::MOOD5", conn))
                    {
                        var p = new NpgsqlParameter { ParameterName = "p", Value = expected };
                        cmd.Parameters.Add(p);
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();

                            Assert.That(reader.GetFieldType(0), Is.EqualTo(typeof(Mood)));
                            Assert.That(reader.GetFieldValue<Mood>(0), Is.EqualTo(expected));
                            Assert.That(reader.GetValue(0), Is.EqualTo(expected));
                        }
                    }
                }

                // Unmap
                NpgsqlConnection.GlobalTypeMapper.UnmapEnum<Mood>("mood5");

                using (var conn = OpenConnection())
                {
                    // Enum should have been unmapped and so will return as text
                    Assert.That(conn.ExecuteScalar("SELECT 'ok'::MOOD5"), Is.EqualTo("ok"));
                }
            }
            finally
            {
                using (var conn = OpenConnection())
                    conn.ExecuteNonQuery("DROP TYPE IF EXISTS mood5");
            }
        }

        [Test]
        public void GlobalMappingWhenTypeNotFound()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("DROP TYPE IF EXISTS pg_temp.mood5");
                NpgsqlConnection.GlobalTypeMapper.MapEnum<Mood>("mood5");
                try
                {
                    Assert.That(conn.ReloadTypes, Throws.Nothing);
                }
                finally
                {
                    NpgsqlConnection.GlobalTypeMapper.UnmapEnum<Mood>("mood5");
                }
            }
        }

        [Test]
        public void Array()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.mood6 AS ENUM ('sad', 'ok', 'happy')");
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<Mood>("mood6");
                var expected = new[] {Mood.Ok, Mood.Happy};
                using (var cmd = new NpgsqlCommand("SELECT @p1::MOOD6[], @p2::MOOD6[]", conn))
                {
                    var p1 = new NpgsqlParameter
                    {
                        ParameterName = "p1",
                        DataTypeName = "mood6[]",
                        Value = expected
                    };
                    var p2 = new NpgsqlParameter {ParameterName = "p2", Value = expected};
                    cmd.Parameters.Add(p1);
                    cmd.Parameters.Add(p2);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();

                        for (var i = 0; i < cmd.Parameters.Count; i++)
                        {
                            Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(Array)));
                            Assert.That(reader.GetFieldValue<Mood[]>(i), Is.EqualTo(expected));
                            Assert.That(reader.GetValue(i), Is.EqualTo(expected));
                        }
                    }
                }
            }
        }

        [Test]
        public void ReadUnmappedEnumsAsString()
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                conn.Open();
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.mood7 AS ENUM ('Sad', 'Ok', 'Happy')");
                conn.ReloadTypes();
                using (var cmd = new NpgsqlCommand("SELECT 'Sad'::MOOD7, ARRAY['Ok', 'Happy']::MOOD7[]", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    Assert.That(reader[0], Is.EqualTo("Sad"));
                    Assert.That(reader.GetDataTypeName(0), Does.StartWith("pg_temp").And.EndsWith("mood7"));
                    Assert.That(reader[1], Is.EqualTo(new[] { "Ok", "Happy" }));
                }
            }
        }

        [Test, Description("Test that a c# string can be written to a backend enum when DbType is unknown")]
        public void WriteStringToBackendEnum()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.fruit AS ENUM ('Banana', 'Apple', 'Orange')");
                conn.ExecuteNonQuery("create table pg_temp.test_fruit ( id serial, value1 pg_temp.fruit, value2 pg_temp.fruit );");
                conn.ReloadTypes();
                const string expected = "Banana";
                using (var cmd = new NpgsqlCommand("insert into pg_temp.test_fruit(id, value1, value2) values(default, @p1, @p2);", conn))
                {
                    cmd.Parameters.AddWithValue("p2", NpgsqlDbType.Unknown, expected);
                    var p2 = new NpgsqlParameter("p1", NpgsqlDbType.Unknown) {Value = expected};
                    cmd.Parameters.Add(p2);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [Test, Description("Tests that a a C# enum an be written to an enum backend when passed as dbUnknown")]
        public void WriteEnumAsDbUnknwown()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.mood8 AS ENUM ('Sad', 'Ok', 'Happy')");
                conn.ExecuteNonQuery("CREATE TABLE pg_temp.test_mood_writes (value1 pg_temp.mood8)");
                conn.ReloadTypes();
                var expected = Mood.Happy;
                using (var cmd = new NpgsqlCommand("insert into pg_temp.test_mood_writes(value1) values(@p1);", conn))
                {
                    cmd.Parameters.AddWithValue("p1", NpgsqlDbType.Unknown, expected);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/859")]
        public void NameTranslationDefaultSnakeCase()
        {
            // Per-connection mapping
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.name_translation_enum AS ENUM ('simple', 'two_words', 'some_database_name')");
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<NameTranslationEnum>();
                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
                {
                    cmd.Parameters.AddWithValue("p1", NameTranslationEnum.Simple);
                    cmd.Parameters.AddWithValue("p2", NameTranslationEnum.TwoWords);
                    cmd.Parameters.AddWithValue("p3", NameTranslationEnum.SomeClrName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetFieldValue<NameTranslationEnum>(0), Is.EqualTo(NameTranslationEnum.Simple));
                        Assert.That(reader.GetFieldValue<NameTranslationEnum>(1), Is.EqualTo(NameTranslationEnum.TwoWords));
                        Assert.That(reader.GetFieldValue<NameTranslationEnum>(2), Is.EqualTo(NameTranslationEnum.SomeClrName));
                    }
                }
            }
            // Global mapping
            NpgsqlConnection.GlobalTypeMapper.MapEnum<NameTranslationEnum>();
            try
            {
                using (var conn = OpenConnection())
                {
                    conn.ExecuteNonQuery("CREATE TYPE pg_temp.name_translation_enum AS ENUM ('simple', 'two_words', 'some_database_name')");
                    conn.ReloadTypes();
                    using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
                    {
                        cmd.Parameters.AddWithValue("p1", NameTranslationEnum.Simple);
                        cmd.Parameters.AddWithValue("p2", NameTranslationEnum.TwoWords);
                        cmd.Parameters.AddWithValue("p3", NameTranslationEnum.SomeClrName);
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            Assert.That(reader.GetFieldValue<NameTranslationEnum>(0), Is.EqualTo(NameTranslationEnum.Simple));
                            Assert.That(reader.GetFieldValue<NameTranslationEnum>(1), Is.EqualTo(NameTranslationEnum.TwoWords));
                            Assert.That(reader.GetFieldValue<NameTranslationEnum>(2), Is.EqualTo(NameTranslationEnum.SomeClrName));
                        }
                    }
                }
            }
            finally
            {
                NpgsqlConnection.GlobalTypeMapper.UnmapEnum<NameTranslationEnum>();
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/859")]
        public void NameTranslationNull()
        {
            // Per-connection mapping
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery(@"CREATE TYPE pg_temp.""NameTranslationEnum"" AS ENUM ('Simple', 'TwoWords', 'some_database_name')");
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<NameTranslationEnum>(nameTranslator: new NpgsqlNullNameTranslator());
                using (var cmd = new NpgsqlCommand("SELECT @p1, @p2, @p3", conn))
                {
                    cmd.Parameters.AddWithValue("p1", NameTranslationEnum.Simple);
                    cmd.Parameters.AddWithValue("p2", NameTranslationEnum.TwoWords);
                    cmd.Parameters.AddWithValue("p3", NameTranslationEnum.SomeClrName);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        Assert.That(reader.GetFieldValue<NameTranslationEnum>(0), Is.EqualTo(NameTranslationEnum.Simple));
                        Assert.That(reader.GetFieldValue<NameTranslationEnum>(1),
                            Is.EqualTo(NameTranslationEnum.TwoWords));
                        Assert.That(reader.GetFieldValue<NameTranslationEnum>(2),
                            Is.EqualTo(NameTranslationEnum.SomeClrName));
                    }
                }
            }
        }

        enum NameTranslationEnum
        {
            Simple,
            TwoWords,
            [PgName("some_database_name")]
            SomeClrName
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/632")]
        public void Schemas()
        {
            try
            {
                using (var conn = OpenConnection())
                {
                    conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS a CASCADE; DROP SCHEMA IF EXISTS b CASCADE");
                    conn.ExecuteNonQuery("CREATE SCHEMA a; CREATE SCHEMA b");
                    conn.ExecuteNonQuery("CREATE TYPE a.my_enum AS ENUM ('one')");
                    conn.ExecuteNonQuery("CREATE TYPE b.my_enum AS ENUM ('alpha')");
                    conn.ReloadTypes();
                    conn.TypeMapper
                        .MapEnum<Enum1>("a.my_enum")
                        .MapEnum<Enum2>("b.my_enum");
                    using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
                    {
                        cmd.Parameters.AddWithValue("p1", Enum1.One);
                        cmd.Parameters.AddWithValue("p2", Enum2.Alpha);
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            Assert.That(reader[0], Is.EqualTo(Enum1.One));
                            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("a.my_enum"));
                            Assert.That(reader[1], Is.EqualTo(Enum2.Alpha));
                            Assert.That(reader.GetDataTypeName(1), Is.EqualTo("b.my_enum"));
                        }
                    }
                }

                // Global mapping
                NpgsqlConnection.GlobalTypeMapper.MapEnum<Enum1>("a.my_enum");
                NpgsqlConnection.GlobalTypeMapper.MapEnum<Enum2>("b.my_enum");
                using (var conn = OpenConnection())
                {
                    using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
                    {
                        cmd.Parameters.AddWithValue("p1", Enum1.One);
                        cmd.Parameters.AddWithValue("p2", Enum2.Alpha);
                        using (var reader = cmd.ExecuteReader())
                        {
                            reader.Read();
                            Assert.That(reader[0], Is.EqualTo(Enum1.One));
                            Assert.That(reader.GetDataTypeName(0), Is.EqualTo("a.my_enum"));
                            Assert.That(reader[1], Is.EqualTo(Enum2.Alpha));
                            Assert.That(reader.GetDataTypeName(1), Is.EqualTo("b.my_enum"));
                        }
                    }
                }
            }
            finally
            {
                NpgsqlConnection.GlobalTypeMapper.UnmapEnum<Enum1>("a.my_enum");
                NpgsqlConnection.GlobalTypeMapper.UnmapEnum<Enum2>("b.my_enum");
                using (var conn = OpenConnection())
                    conn.ExecuteNonQuery("DROP SCHEMA IF EXISTS a CASCADE; DROP SCHEMA IF EXISTS b CASCADE");
            }
        }

        enum Enum1 { One }
        enum Enum2 { Alpha }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1017")]
        public void GlobalMappingsAndPooling()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString) {
                ApplicationName = nameof(GlobalMappingsAndPooling)
            };

            int serverId;
            using (var conn = OpenConnection(csb))
            {
                serverId = conn.ProcessID;
                conn.ExecuteNonQuery("DROP TYPE IF EXISTS mood9");
                conn.ExecuteNonQuery("CREATE TYPE mood9 AS ENUM ('sad', 'ok', 'happy')");
                conn.ReloadTypes();
            }
            // At this point the backend type for the enum is loaded, but no global mapping
            // has been made. Reopening the same pooled connector should learn about the new
            // global mapping
            NpgsqlConnection.GlobalTypeMapper.MapEnum<Mood>("mood9");
            try
            {
                using (var conn = OpenConnection(csb))
                {
                    Assert.That(conn.ProcessID, Is.EqualTo(serverId));
                    Assert.That(conn.ExecuteScalar("SELECT 'sad'::mood9"), Is.EqualTo(Mood.Sad));
                }
            }
            finally
            {
                using (var conn = OpenConnection(csb))
                    conn.ExecuteNonQuery("DROP TYPE IF EXISTS mood9");
                NpgsqlConnection.GlobalTypeMapper.UnmapEnum<Mood>("mood1");
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1779")]
        public void EnumPostgresType()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                ApplicationName = nameof(PostgresType),
                Pooling = false
            };
            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("DROP TYPE IF EXISTS mood9; CREATE TYPE mood9 AS ENUM ('sad', 'ok', 'happy')");
                conn.ReloadTypes();

                using (var cmd = new NpgsqlCommand("SELECT 'ok'::mood9", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        var enumType = (PostgresEnumType)reader.GetPostgresType(0);
                        Assert.That(enumType.Name, Is.EqualTo("mood9"));
                        Assert.That(enumType.Labels, Is.EqualTo(new List<string> { "sad", "ok", "happy" }));
                    }
                }
            }
        }

        enum TestEnum
        {
            label1,
            label2,
            [PgName("label3")]
            Label3
        }
    }
}
