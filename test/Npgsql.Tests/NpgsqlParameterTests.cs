using NpgsqlTypes;
using NUnit.Framework;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Npgsql.Internal.Postgres;

namespace Npgsql.Tests;

public class NpgsqlParameterTest : TestBase
{
    [Test, Description("Makes sure that when NpgsqlDbType or Value/NpgsqlValue are set, DbType and NpgsqlDbType are set accordingly")]
    public void Implicit_setting_of_DbType()
    {
        var p = new NpgsqlParameter("p", DbType.Int32);
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));

        // As long as NpgsqlDbType/DbType aren't set explicitly, infer them from Value
        p = new NpgsqlParameter("p", 8);
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer));
        Assert.That(p.DbType, Is.EqualTo(DbType.Int32));

        p.Value = 3.0;
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Double));
        Assert.That(p.DbType, Is.EqualTo(DbType.Double));

        p.NpgsqlDbType = NpgsqlDbType.Bytea;
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea));
        Assert.That(p.DbType, Is.EqualTo(DbType.Binary));

        p.Value = "dont_change";
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea));
        Assert.That(p.DbType, Is.EqualTo(DbType.Binary));

        p = new NpgsqlParameter("p", new int[0]);
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Array | NpgsqlDbType.Integer));
        Assert.That(p.DbType, Is.EqualTo(DbType.Object));
    }

    [Test]
    public void DataTypeName()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT @p", conn);
        var p1 = new NpgsqlParameter { ParameterName = "p", Value = 8, DataTypeName = "integer" };
        cmd.Parameters.Add(p1);
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(8));
        // Purposefully try to send int as string, which should fail. This makes sure
        // the above doesn't work simply because of type inference from the CLR type.
        p1.DataTypeName = "text";
        Assert.That(() => cmd.ExecuteScalar(), Throws.Exception.TypeOf<InvalidCastException>());

        cmd.Parameters.Clear();

        var p2 = new NpgsqlParameter<int> { ParameterName = "p", TypedValue = 8, DataTypeName = "integer" };
        cmd.Parameters.Add(p2);
        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(8));
        // Purposefully try to send int as string, which should fail. This makes sure
        // the above doesn't work simply because of type inference from the CLR type.
        p2.DataTypeName = "text";
        Assert.That(() => cmd.ExecuteScalar(), Throws.Exception.TypeOf<InvalidCastException>());
    }

    [Test]
    public void Positional_parameter_is_positional()
    {
        var p = new NpgsqlParameter(NpgsqlParameter.PositionalName, 1);
        Assert.That(p.IsPositional, Is.True);

        var p2 = new NpgsqlParameter(null, 1);
        Assert.That(p2.IsPositional, Is.True);
    }

    [Test]
    public void Infer_data_type_name_from_NpgsqlDbType()
    {
        var p = new NpgsqlParameter("par_field1", NpgsqlDbType.Varchar, 50);
        Assert.That(p.DataTypeName, Is.EqualTo("character varying"));
    }

    [Test]
    public void Infer_data_type_name_from_DbType()
    {
        var p = new NpgsqlParameter("par_field1", DbType.String , 50);
        Assert.That(p.DataTypeName, Is.EqualTo("text"));
    }

    [Test]
    public void Infer_data_type_name_from_NpgsqlDbType_for_array()
    {
        var p = new NpgsqlParameter("int_array", NpgsqlDbType.Array | NpgsqlDbType.Integer);
        Assert.That(p.DataTypeName, Is.EqualTo("integer[]"));
    }

    [Test]
    public void Infer_data_type_name_from_NpgsqlDbType_for_built_in_range()
    {
        var p = new NpgsqlParameter("numeric_range", NpgsqlDbType.Range | NpgsqlDbType.Numeric);
        Assert.That(p.DataTypeName, Is.EqualTo("numrange"));
    }

    [Test]
    public void Cannot_infer_data_type_name_from_NpgsqlDbType_for_unknown_range()
    {
        var p = new NpgsqlParameter("text_range", NpgsqlDbType.Range | NpgsqlDbType.Text);
        Assert.That(p.DataTypeName, Is.EqualTo(null));
    }

    [Test]
    public void Infer_data_type_name_from_ClrType()
    {
        var p = new NpgsqlParameter("p1", Array.Empty<byte>());
        Assert.That(p.DataTypeName, Is.EqualTo("bytea"));
    }

    [Test]
    public void Setting_DbType_sets_NpgsqlDbType()
    {
        var p = new NpgsqlParameter();
        p.DbType = DbType.Binary;
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea));
    }

    [Test]
    public void Setting_NpgsqlDbType_sets_DbType()
    {
        var p = new NpgsqlParameter();
        p.NpgsqlDbType = NpgsqlDbType.Bytea;
        Assert.That(p.DbType, Is.EqualTo(DbType.Binary));
    }

    [Test]
    public void Setting_value_does_not_change_DbType()
    {
        var p = new NpgsqlParameter { DbType = DbType.String, NpgsqlDbType = NpgsqlDbType.Bytea };
        p.Value = 8;
        Assert.That(p.DbType, Is.EqualTo(DbType.Binary));
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea));
    }

    // Older tests

    #region Constructors

    [Test]
    public void Constructor1()
    {
        var p = new NpgsqlParameter();
        Assert.That(p.DbType, Is.EqualTo(DbType.Object), "DbType");
        Assert.That(p.Direction, Is.EqualTo(ParameterDirection.Input), "Direction");
        Assert.That(p.IsNullable, Is.False, "IsNullable");
        Assert.That(p.ParameterName, Is.Empty, "ParameterName");
        Assert.That(p.Precision, Is.EqualTo(0), "Precision");
        Assert.That(p.Scale, Is.EqualTo(0), "Scale");
        Assert.That(p.Size, Is.EqualTo(0), "Size");
        Assert.That(p.SourceColumn, Is.Empty, "SourceColumn");
        Assert.That(p.SourceVersion, Is.EqualTo(DataRowVersion.Current), "SourceVersion");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown), "NpgsqlDbType");
        Assert.That(p.Value, Is.Null, "Value");
    }

    [Test]
    public void Constructor2_Value_DateTime()
    {
        var value = new DateTime(2004, 8, 24);

        var p = new NpgsqlParameter("address", value);
        Assert.That(p.DbType, Is.EqualTo(DbType.DateTime2), "B:DbType");
        Assert.That(p.Direction, Is.EqualTo(ParameterDirection.Input), "B:Direction");
        Assert.That(p.IsNullable, Is.False, "B:IsNullable");
        Assert.That(p.ParameterName, Is.EqualTo("address"), "B:ParameterName");
        Assert.That(p.Precision, Is.EqualTo(0), "B:Precision");
        Assert.That(p.Scale, Is.EqualTo(0), "B:Scale");
        //Assert.AreEqual (0, p.Size, "B:Size");
        Assert.That(p.SourceColumn, Is.Empty, "B:SourceColumn");
        Assert.That(p.SourceVersion, Is.EqualTo(DataRowVersion.Current), "B:SourceVersion");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp), "B:NpgsqlDbType");
        Assert.That(p.Value, Is.EqualTo(value), "B:Value");
    }

    [Test]
    public void Constructor2_Value_DBNull()
    {
        var p = new NpgsqlParameter("address", DBNull.Value);
        Assert.That(p.DbType, Is.EqualTo(DbType.Object), "B:DbType");
        Assert.That(p.Direction, Is.EqualTo(ParameterDirection.Input), "B:Direction");
        Assert.That(p.IsNullable, Is.False, "B:IsNullable");
        Assert.That(p.ParameterName, Is.EqualTo("address"), "B:ParameterName");
        Assert.That(p.Precision, Is.EqualTo(0), "B:Precision");
        Assert.That(p.Scale, Is.EqualTo(0), "B:Scale");
        Assert.That(p.Size, Is.EqualTo(0), "B:Size");
        Assert.That(p.SourceColumn, Is.Empty, "B:SourceColumn");
        Assert.That(p.SourceVersion, Is.EqualTo(DataRowVersion.Current), "B:SourceVersion");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown), "B:NpgsqlDbType");
        Assert.That(p.Value, Is.EqualTo(DBNull.Value), "B:Value");
    }

    [Test]
    public void Constructor2_Value_null()
    {
        var p = new NpgsqlParameter("address", null);
        Assert.That(p.DbType, Is.EqualTo(DbType.Object), "A:DbType");
        Assert.That(p.Direction, Is.EqualTo(ParameterDirection.Input), "A:Direction");
        Assert.That(p.IsNullable, Is.False, "A:IsNullable");
        Assert.That(p.ParameterName, Is.EqualTo("address"), "A:ParameterName");
        Assert.That(p.Precision, Is.EqualTo(0), "A:Precision");
        Assert.That(p.Scale, Is.EqualTo(0), "A:Scale");
        Assert.That(p.Size, Is.EqualTo(0), "A:Size");
        Assert.That(p.SourceColumn, Is.Empty, "A:SourceColumn");
        Assert.That(p.SourceVersion, Is.EqualTo(DataRowVersion.Current), "A:SourceVersion");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown), "A:NpgsqlDbType");
        Assert.That(p.Value, Is.Null, "A:Value");
    }

    [Test]
    //.ctor (String, NpgsqlDbType, Int32, String, ParameterDirection, bool, byte, byte, DataRowVersion, object)
    public void Constructor7()
    {
        var p1 = new NpgsqlParameter("p1Name", NpgsqlDbType.Varchar, 20,
            "srcCol", ParameterDirection.InputOutput, false, 0, 0,
            DataRowVersion.Original, "foo");
        Assert.That(p1.DbType, Is.EqualTo(DbType.String), "DbType");
        Assert.That(p1.Direction, Is.EqualTo(ParameterDirection.InputOutput), "Direction");
        Assert.That(p1.IsNullable, Is.EqualTo(false), "IsNullable");
        //Assert.AreEqual (999, p1.LocaleId, "#");
        Assert.That(p1.ParameterName, Is.EqualTo("p1Name"), "ParameterName");
        Assert.That(p1.Precision, Is.EqualTo(0), "Precision");
        Assert.That(p1.Scale, Is.EqualTo(0), "Scale");
        Assert.That(p1.Size, Is.EqualTo(20), "Size");
        Assert.That(p1.SourceColumn, Is.EqualTo("srcCol"), "SourceColumn");
        Assert.That(p1.SourceColumnNullMapping, Is.EqualTo(false), "SourceColumnNullMapping");
        Assert.That(p1.SourceVersion, Is.EqualTo(DataRowVersion.Original), "SourceVersion");
        Assert.That(p1.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Varchar), "NpgsqlDbType");
        //Assert.AreEqual (3210, p1.NpgsqlValue, "#");
        Assert.That(p1.Value, Is.EqualTo("foo"), "Value");
        //Assert.AreEqual ("database", p1.XmlSchemaCollectionDatabase, "XmlSchemaCollectionDatabase");
        //Assert.AreEqual ("name", p1.XmlSchemaCollectionName, "XmlSchemaCollectionName");
        //Assert.AreEqual ("schema", p1.XmlSchemaCollectionOwningSchema, "XmlSchemaCollectionOwningSchema");
    }

    [Test]
    public void Clone()
    {
        var expected = new NpgsqlParameter
        {
            Value = 42,
            ParameterName = "TheAnswer",

            DbType = DbType.Int32,
            NpgsqlDbType = NpgsqlDbType.Integer,
            DataTypeName = "integer",

            Direction = ParameterDirection.InputOutput,
            IsNullable = true,
            Precision = 1,
            Scale = 2,
            Size = 4,

            SourceVersion = DataRowVersion.Proposed,
            SourceColumn = "source",
            SourceColumnNullMapping = true,
        };
        var actual = expected.Clone();

        Assert.That(actual.Value, Is.EqualTo(expected.Value));
        Assert.That(actual.ParameterName, Is.EqualTo(expected.ParameterName));

        Assert.That(actual.DbType, Is.EqualTo(expected.DbType));
        Assert.That(actual.NpgsqlDbType, Is.EqualTo(expected.NpgsqlDbType));
        Assert.That(actual.DataTypeName, Is.EqualTo(expected.DataTypeName));

        Assert.That(actual.Direction, Is.EqualTo(expected.Direction));
        Assert.That(actual.IsNullable, Is.EqualTo(expected.IsNullable));
        Assert.That(actual.Precision, Is.EqualTo(expected.Precision));
        Assert.That(actual.Scale, Is.EqualTo(expected.Scale));
        Assert.That(actual.Size, Is.EqualTo(expected.Size));

        Assert.That(actual.SourceVersion, Is.EqualTo(expected.SourceVersion));
        Assert.That(actual.SourceColumn, Is.EqualTo(expected.SourceColumn));
        Assert.That(actual.SourceColumnNullMapping, Is.EqualTo(expected.SourceColumnNullMapping));
    }

    [Test]
    public void Clone_generic()
    {
        var expected = new NpgsqlParameter<int>
        {
            TypedValue = 42,
            ParameterName = "TheAnswer",

            DbType = DbType.Int32,
            NpgsqlDbType = NpgsqlDbType.Integer,
            DataTypeName = "integer",

            Direction = ParameterDirection.InputOutput,
            IsNullable = true,
            Precision = 1,
            Scale = 2,
            Size = 4,

            SourceVersion = DataRowVersion.Proposed,
            SourceColumn ="source",
            SourceColumnNullMapping = true,
        };
        var actual = (NpgsqlParameter<int>)expected.Clone();

        Assert.That(actual.Value, Is.EqualTo(expected.Value));
        Assert.That(actual.TypedValue, Is.EqualTo(expected.TypedValue));
        Assert.That(actual.ParameterName, Is.EqualTo(expected.ParameterName));

        Assert.That(actual.DbType, Is.EqualTo(expected.DbType));
        Assert.That(actual.NpgsqlDbType, Is.EqualTo(expected.NpgsqlDbType));
        Assert.That(actual.DataTypeName, Is.EqualTo(expected.DataTypeName));

        Assert.That(actual.Direction, Is.EqualTo(expected.Direction));
        Assert.That(actual.IsNullable, Is.EqualTo(expected.IsNullable));
        Assert.That(actual.Precision, Is.EqualTo(expected.Precision));
        Assert.That(actual.Scale, Is.EqualTo(expected.Scale));
        Assert.That(actual.Size, Is.EqualTo(expected.Size));

        Assert.That(actual.SourceVersion, Is.EqualTo(expected.SourceVersion));
        Assert.That(actual.SourceColumn, Is.EqualTo(expected.SourceColumn));
        Assert.That(actual.SourceColumnNullMapping, Is.EqualTo(expected.SourceColumnNullMapping));
    }

    #endregion

    [Test]
    [Ignore("")]
    public void InferType_invalid_throws()
    {
        var notsupported = new object[]
        {
            ushort.MaxValue,
            uint.MaxValue,
            ulong.MaxValue,
            sbyte.MaxValue,
            new NpgsqlParameter()
        };

        var param = new NpgsqlParameter();

        for (var i = 0; i < notsupported.Length; i++)
        {
            try
            {
                param.Value = notsupported[i];
                Assert.Fail("#A1:" + i);
            }
            catch (FormatException)
            {
                // appears to be bug in .NET 1.1 while
                // constructing exception message
            }
            catch (ArgumentException ex)
            {
                // The parameter data type of ... is invalid
                Assert.That(ex.GetType(), Is.EqualTo(typeof(ArgumentException)), "#A2");
                Assert.That(ex.InnerException, Is.Null, "#A3");
                Assert.That(ex.Message, Is.Not.Null, "#A4");
                Assert.That(ex.ParamName, Is.Null, "#A5");
            }
        }
    }

    [Test] // bug #320196
    public void Parameter_null()
    {
        var param = new NpgsqlParameter("param", NpgsqlDbType.Numeric);
        Assert.That(param.Scale, Is.EqualTo(0), "#A1");
        param.Value = DBNull.Value;
        Assert.That(param.Scale, Is.EqualTo(0), "#A2");

        param = new NpgsqlParameter("param", NpgsqlDbType.Integer);
        Assert.That(param.Scale, Is.EqualTo(0), "#B1");
        param.Value = DBNull.Value;
        Assert.That(param.Scale, Is.EqualTo(0), "#B2");
    }

    [Test]
    [Ignore("")]
    public void Parameter_type()
    {
        NpgsqlParameter p;

        // If Type is not set, then type is inferred from the value
        // assigned. The Type should be inferred everytime Value is assigned
        // If value is null or DBNull, then the current Type should be reset to Text.
        p = new NpgsqlParameter();
        Assert.That(p.DbType, Is.EqualTo(DbType.String), "#A1");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Text), "#A2");
        p.Value = DBNull.Value;
        Assert.That(p.DbType, Is.EqualTo(DbType.String), "#B1");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Text), "#B2");
        p.Value = 1;
        Assert.That(p.DbType, Is.EqualTo(DbType.Int32), "#C1");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer), "#C2");
        p.Value = DBNull.Value;
        Assert.That(p.DbType, Is.EqualTo(DbType.String), "#D1");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Text), "#D2");
        p.Value = new byte[] { 0x0a };
        Assert.That(p.DbType, Is.EqualTo(DbType.Binary), "#E1");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea), "#E2");
        p.Value = null;
        Assert.That(p.DbType, Is.EqualTo(DbType.String), "#F1");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Text), "#F2");
        p.Value = DateTime.Now;
        Assert.That(p.DbType, Is.EqualTo(DbType.DateTime), "#G1");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp), "#G2");
        p.Value = null;
        Assert.That(p.DbType, Is.EqualTo(DbType.String), "#H1");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Text), "#H2");

        // If DbType is set, then the NpgsqlDbType should not be
        // inferred from the value assigned.
        p = new NpgsqlParameter();
        p.DbType = DbType.DateTime;
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp), "#I1");
        p.Value = 1;
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp), "#I2");
        p.Value = null;
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp), "#I3");
        p.Value = DBNull.Value;
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp), "#I4");

        // If NpgsqlDbType is set, then the DbType should not be
        // inferred from the value assigned.
        p = new NpgsqlParameter();
        p.NpgsqlDbType = NpgsqlDbType.Bytea;
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea), "#J1");
        p.Value = 1;
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea), "#J2");
        p.Value = null;
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea), "#J3");
        p.Value = DBNull.Value;
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Bytea), "#J4");
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/5428")]
    public async Task Match_param_index_case_insensitively()
    {
        await using var conn = await OpenConnectionAsync();
        await using var cmd = new NpgsqlCommand("SELECT @p,@P", conn);
        cmd.Parameters.AddWithValue("p", "Hello world");
        await cmd.ExecuteNonQueryAsync();
    }

    [Test]
    [Ignore("")]
    public void ParameterName()
    {
        var p = new NpgsqlParameter();
        p.ParameterName = "name";
        Assert.That(p.ParameterName, Is.EqualTo("name"), "#A:ParameterName");
        Assert.That(p.SourceColumn, Is.Empty, "#A:SourceColumn");

        p.ParameterName = null;
        Assert.That(p.ParameterName, Is.Empty, "#B:ParameterName");
        Assert.That(p.SourceColumn, Is.Empty, "#B:SourceColumn");

        p.ParameterName = " ";
        Assert.That(p.ParameterName, Is.EqualTo(" "), "#C:ParameterName");
        Assert.That(p.SourceColumn, Is.Empty, "#C:SourceColumn");

        p.ParameterName = " name ";
        Assert.That(p.ParameterName, Is.EqualTo(" name "), "#D:ParameterName");
        Assert.That(p.SourceColumn, Is.Empty, "#D:SourceColumn");

        p.ParameterName = string.Empty;
        Assert.That(p.ParameterName, Is.Empty, "#E:ParameterName");
        Assert.That(p.SourceColumn, Is.Empty, "#E:SourceColumn");
    }

    [Test]
    public void ResetDbType()
    {
        NpgsqlParameter p;

        //Parameter with an assigned value but no DbType specified
        p = new NpgsqlParameter("foo", 42);
        p.ResetDbType();
        Assert.That(p.DbType, Is.EqualTo(DbType.Int32), "#A:DbType");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer), "#A:NpgsqlDbType");
        Assert.That(p.Value, Is.EqualTo(42), "#A:Value");

        p.DbType = DbType.DateTime; //assigning a DbType
        Assert.That(p.DbType, Is.EqualTo(DbType.DateTime), "#B:DbType1");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.TimestampTz), "#B:SqlDbType1");
        p.ResetDbType();
        Assert.That(p.DbType, Is.EqualTo(DbType.Int32), "#B:DbType2");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer), "#B:SqlDbtype2");

        //Parameter with an assigned NpgsqlDbType but no specified value
        p = new NpgsqlParameter("foo", NpgsqlDbType.Integer);
        p.ResetDbType();
        Assert.That(p.DbType, Is.EqualTo(DbType.Object), "#C:DbType");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown), "#C:NpgsqlDbType");

        p.NpgsqlDbType = NpgsqlDbType.TimestampTz; //assigning a NpgsqlDbType
        Assert.That(p.DbType, Is.EqualTo(DbType.DateTime), "#D:DbType1");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.TimestampTz), "#D:SqlDbType1");
        p.ResetDbType();
        Assert.That(p.DbType, Is.EqualTo(DbType.Object), "#D:DbType2");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown), "#D:SqlDbType2");

        p = new NpgsqlParameter();
        p.Value = DateTime.MaxValue;
        Assert.That(p.DbType, Is.EqualTo(DbType.DateTime2), "#E:DbType1");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp), "#E:SqlDbType1");
        p.Value = null;
        p.ResetDbType();
        Assert.That(p.DbType, Is.EqualTo(DbType.Object), "#E:DbType2");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown), "#E:SqlDbType2");

        p = new NpgsqlParameter("foo", NpgsqlDbType.Varchar);
        p.Value = DateTime.MaxValue;
        p.ResetDbType();
        Assert.That(p.DbType, Is.EqualTo(DbType.DateTime2), "#F:DbType");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Timestamp), "#F:NpgsqlDbType");
        Assert.That(p.Value, Is.EqualTo(DateTime.MaxValue), "#F:Value");

        p = new NpgsqlParameter("foo", NpgsqlDbType.Varchar);
        p.Value = DBNull.Value;
        p.ResetDbType();
        Assert.That(p.DbType, Is.EqualTo(DbType.Object), "#G:DbType");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown), "#G:NpgsqlDbType");
        Assert.That(p.Value, Is.EqualTo(DBNull.Value), "#G:Value");

        p = new NpgsqlParameter("foo", NpgsqlDbType.Varchar);
        p.Value = null;
        p.ResetDbType();
        Assert.That(p.DbType, Is.EqualTo(DbType.Object), "#G:DbType");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown), "#G:NpgsqlDbType");
        Assert.That(p.Value, Is.Null, "#G:Value");
    }

    [Test]
    public void ParameterName_retains_prefix()
        => Assert.That(new NpgsqlParameter("@p", DbType.String).ParameterName, Is.EqualTo("@p"));

    [Test]
    [Ignore("")]
    public void SourceColumn()
    {
        var p = new NpgsqlParameter();
        p.SourceColumn = "name";
        Assert.That(p.ParameterName, Is.Empty, "#A:ParameterName");
        Assert.That(p.SourceColumn, Is.EqualTo("name"), "#A:SourceColumn");

        p.SourceColumn = null;
        Assert.That(p.ParameterName, Is.Empty, "#B:ParameterName");
        Assert.That(p.SourceColumn, Is.Empty, "#B:SourceColumn");

        p.SourceColumn = " ";
        Assert.That(p.ParameterName, Is.Empty, "#C:ParameterName");
        Assert.That(p.SourceColumn, Is.EqualTo(" "), "#C:SourceColumn");

        p.SourceColumn = " name ";
        Assert.That(p.ParameterName, Is.Empty, "#D:ParameterName");
        Assert.That(p.SourceColumn, Is.EqualTo(" name "), "#D:SourceColumn");

        p.SourceColumn = string.Empty;
        Assert.That(p.ParameterName, Is.Empty, "#E:ParameterName");
        Assert.That(p.SourceColumn, Is.Empty, "#E:SourceColumn");
    }

    [Test]
    public void Bug1011100_NpgsqlDbType()
    {
        var p = new NpgsqlParameter();
        p.Value = DBNull.Value;
        Assert.That(p.DbType, Is.EqualTo(DbType.Object), "#A:DbType");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Unknown), "#A:NpgsqlDbType");

        // Now change parameter value.
        // Note that as we didn't explicitly specified a dbtype, the dbtype property should change when
        // the value changes...

        p.Value = 8;

        Assert.That(p.DbType, Is.EqualTo(DbType.Int32), "#A:DbType");
        Assert.That(p.NpgsqlDbType, Is.EqualTo(NpgsqlDbType.Integer), "#A:NpgsqlDbType");

        //Assert.AreEqual(3510, p.Value, "#A:Value");
        //p.NpgsqlDbType = NpgsqlDbType.Varchar;
        //Assert.AreEqual(DbType.String, p.DbType, "#B:DbType");
        //Assert.AreEqual(NpgsqlDbType.Varchar, p.NpgsqlDbType, "#B:NpgsqlDbType");
        //Assert.AreEqual(3510, p.Value, "#B:Value");
    }

    [Test]
    public void NpgsqlParameter_Clone()
    {
        var param = new NpgsqlParameter();

        param.Value = 5;
        param.Precision = 1;
        param.Scale = 1;
        param.Size = 1;
        param.Direction = ParameterDirection.Input;
        param.IsNullable = true;
        param.ParameterName = "parameterName";
        param.SourceColumn = "source_column";
        param.SourceVersion = DataRowVersion.Current;
        param.NpgsqlValue = 5;
        param.SourceColumnNullMapping = false;

        var newParam = param.Clone();

        Assert.That(newParam.Value, Is.EqualTo(param.Value));
        Assert.That(newParam.Precision, Is.EqualTo(param.Precision));
        Assert.That(newParam.Scale, Is.EqualTo(param.Scale));
        Assert.That(newParam.Size, Is.EqualTo(param.Size));
        Assert.That(newParam.Direction, Is.EqualTo(param.Direction));
        Assert.That(newParam.IsNullable, Is.EqualTo(param.IsNullable));
        Assert.That(newParam.ParameterName, Is.EqualTo(param.ParameterName));
        Assert.That(newParam.TrimmedName, Is.EqualTo(param.TrimmedName));
        Assert.That(newParam.SourceColumn, Is.EqualTo(param.SourceColumn));
        Assert.That(newParam.SourceVersion, Is.EqualTo(param.SourceVersion));
        Assert.That(newParam.NpgsqlValue, Is.EqualTo(param.NpgsqlValue));
        Assert.That(newParam.SourceColumnNullMapping, Is.EqualTo(param.SourceColumnNullMapping));
        Assert.That(newParam.NpgsqlValue, Is.EqualTo(param.NpgsqlValue));

    }

    [Test]
    public void Precision_via_interface()
    {
        var parameter = new NpgsqlParameter();
        var paramIface = (IDbDataParameter)parameter;

        paramIface.Precision = 42;

        Assert.That(paramIface.Precision, Is.EqualTo((byte)42));
    }

    [Test]
    public void Precision_via_base_class()
    {
        var parameter = new NpgsqlParameter();
        var paramBase = (DbParameter)parameter;

        paramBase.Precision = 42;

        Assert.That(paramBase.Precision, Is.EqualTo((byte)42));
    }

    [Test]
    public void Scale_via_interface()
    {
        var parameter = new NpgsqlParameter();
        var paramIface = (IDbDataParameter)parameter;

        paramIface.Scale = 42;

        Assert.That(paramIface.Scale, Is.EqualTo((byte)42));
    }

    [Test]
    public void Scale_via_base_class()
    {
        var parameter = new NpgsqlParameter();
        var paramBase = (DbParameter)parameter;

        paramBase.Scale = 42;

        Assert.That(paramBase.Scale, Is.EqualTo((byte)42));
    }

    [Test]
    public void Null_value_throws()
    {
        using var connection = OpenConnection();
        using var command = new NpgsqlCommand("SELECT @p", connection)
        {
            Parameters = { new NpgsqlParameter("p", null) }
        };

        Assert.That(() => command.ExecuteReader(), Throws.InvalidOperationException);
    }

    [Test]
    public void Null_value_with_nullable_type()
    {
        using var connection = OpenConnection();
        using var command = new NpgsqlCommand("SELECT @p", connection)
        {
            Parameters = { new NpgsqlParameter<int?>("p", null) }
        };
        using var reader = command.ExecuteReader();

        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetFieldValue<int?>(0), Is.Null);
    }

    [Test]
    public void DBNull_reuses_type_info([Values]bool generic)
    {
        var param = generic ? new NpgsqlParameter<object> { Value = "value" } : new NpgsqlParameter { Value = "value" };
        param.ResolveTypeInfo(DataSource.CurrentReloadableState.SerializerOptions);
        param.GetResolutionInfo(out var typeInfo, out _, out _);
        Assert.That(typeInfo, Is.Not.Null);

        // Make sure we don't reset the type info when setting DBNull.
        param.Value = DBNull.Value;
        param.GetResolutionInfo(out var secondTypeInfo, out _, out _);
        Assert.That(secondTypeInfo, Is.SameAs(typeInfo));

        // Make sure we don't resolve a different type info either.
        param.ResolveTypeInfo(DataSource.CurrentReloadableState.SerializerOptions);
        param.GetResolutionInfo(out var thirdTypeInfo, out _, out _);
        Assert.That(thirdTypeInfo, Is.SameAs(secondTypeInfo));
    }

    [Test]
    public void DBNull_followed_by_non_null_reresolves([Values]bool generic)
    {
        var param = generic ? new NpgsqlParameter<object> { Value = DBNull.Value } : new NpgsqlParameter { Value = DBNull.Value };
        param.ResolveTypeInfo(DataSource.CurrentReloadableState.SerializerOptions);
        param.GetResolutionInfo(out var typeInfo, out _, out var pgTypeId);
        Assert.That(typeInfo, Is.Not.Null);
        Assert.That(pgTypeId.IsUnspecified, Is.True);

        param.Value = "value";
        param.GetResolutionInfo(out var secondTypeInfo, out _, out _);
        Assert.That(secondTypeInfo, Is.Null);

        // Make sure we don't resolve the same type info either.
        param.ResolveTypeInfo(DataSource.CurrentReloadableState.SerializerOptions);
        param.GetResolutionInfo(out var thirdTypeInfo, out _, out _);
        Assert.That(thirdTypeInfo, Is.Not.SameAs(typeInfo));
    }

    [Test]
    public void Changing_value_type_reresolves([Values]bool generic)
    {
        var param = generic ? new NpgsqlParameter<object> { Value = "value" } : new NpgsqlParameter { Value = "value" };
        param.ResolveTypeInfo(DataSource.CurrentReloadableState.SerializerOptions);
        param.GetResolutionInfo(out var typeInfo, out _, out _);
        Assert.That(typeInfo, Is.Not.Null);

        param.Value = 1;
        param.GetResolutionInfo(out var secondTypeInfo, out _, out _);
        Assert.That(secondTypeInfo, Is.Null);

        // Make sure we don't resolve a different type info either.
        param.ResolveTypeInfo(DataSource.CurrentReloadableState.SerializerOptions);
        param.GetResolutionInfo(out var thirdTypeInfo, out _, out _);
        Assert.That(thirdTypeInfo, Is.Not.SameAs(typeInfo));
    }

    [Test]
    public void DataTypeName_prioritized_over_NpgsqlDbType([Values]bool generic)
    {
        var param = generic ? new NpgsqlParameter<object>
        {
            NpgsqlDbType = NpgsqlDbType.Integer,
            DataTypeName = "text",
            Value = "value"
        } : new NpgsqlParameter
        {
            NpgsqlDbType = NpgsqlDbType.Integer,
            DataTypeName = "text",
            Value = "value"
        };
        param.ResolveTypeInfo(DataSource.CurrentReloadableState.SerializerOptions);
        param.GetResolutionInfo(out var typeInfo, out _, out _);
        Assert.That(typeInfo, Is.Not.Null);
        Assert.That(typeInfo.PgTypeId, Is.EqualTo(DataSource.CurrentReloadableState.SerializerOptions.TextPgTypeId));
    }

#if NeedsPorting
    [Test]
    [Category ("NotWorking")]
    public void InferType_Char()
    {
        Char value = 'X';

        String string_value = "X";

        NpgsqlParameter p = new NpgsqlParameter ();
        p.Value = value;
        Assert.AreEqual (NpgsqlDbType.Text, p.NpgsqlDbType, "#A:NpgsqlDbType");
        Assert.AreEqual (DbType.String, p.DbType, "#A:DbType");
        Assert.AreEqual (string_value, p.Value, "#A:Value");

        p = new NpgsqlParameter ();
        p.Value = value;
        Assert.AreEqual (value, p.Value, "#B:Value1");
        Assert.AreEqual (NpgsqlDbType.Text, p.NpgsqlDbType, "#B:NpgsqlDbType");
        Assert.AreEqual (string_value, p.Value, "#B:Value2");

        p = new NpgsqlParameter ();
        p.Value = value;
        Assert.AreEqual (value, p.Value, "#C:Value1");
        Assert.AreEqual (DbType.String, p.DbType, "#C:DbType");
        Assert.AreEqual (string_value, p.Value, "#C:Value2");

        p = new NpgsqlParameter ("name", value);
        Assert.AreEqual (value, p.Value, "#D:Value1");
        Assert.AreEqual (DbType.String, p.DbType, "#D:DbType");
        Assert.AreEqual (NpgsqlDbType.Text, p.NpgsqlDbType, "#D:NpgsqlDbType");
        Assert.AreEqual (string_value, p.Value, "#D:Value2");

        p = new NpgsqlParameter ("name", 5);
        p.Value = value;
        Assert.AreEqual (value, p.Value, "#E:Value1");
        Assert.AreEqual (DbType.String, p.DbType, "#E:DbType");
        Assert.AreEqual (NpgsqlDbType.Text, p.NpgsqlDbType, "#E:NpgsqlDbType");
        Assert.AreEqual (string_value, p.Value, "#E:Value2");

        p = new NpgsqlParameter ("name", NpgsqlDbType.Text);
        p.Value = value;
        Assert.AreEqual (NpgsqlDbType.Text, p.NpgsqlDbType, "#F:NpgsqlDbType");
        Assert.AreEqual (value, p.Value, "#F:Value");
    }

    [Test]
    [Category ("NotWorking")]
    public void InferType_CharArray()
    {
        Char[] value = new Char[] { 'A', 'X' };

        String string_value = "AX";

        NpgsqlParameter p = new NpgsqlParameter ();
        p.Value = value;
        Assert.AreEqual (value, p.Value, "#A:Value1");
        Assert.AreEqual (NpgsqlDbType.Text, p.NpgsqlDbType, "#A:NpgsqlDbType");
        Assert.AreEqual (DbType.String, p.DbType, "#A:DbType");
        Assert.AreEqual (string_value, p.Value, "#A:Value2");

        p = new NpgsqlParameter ();
        p.Value = value;
        Assert.AreEqual (value, p.Value, "#B:Value1");
        Assert.AreEqual (NpgsqlDbType.Text, p.NpgsqlDbType, "#B:NpgsqlDbType");
        Assert.AreEqual (string_value, p.Value, "#B:Value2");

        p = new NpgsqlParameter ();
        p.Value = value;
        Assert.AreEqual (value, p.Value, "#C:Value1");
        Assert.AreEqual (DbType.String, p.DbType, "#C:DbType");
        Assert.AreEqual (string_value, p.Value, "#C:Value2");

        p = new NpgsqlParameter ("name", value);
        Assert.AreEqual (value, p.Value, "#D:Value1");
        Assert.AreEqual (DbType.String, p.DbType, "#D:DbType");
        Assert.AreEqual (NpgsqlDbType.Text, p.NpgsqlDbType, "#D:NpgsqlDbType");
        Assert.AreEqual (string_value, p.Value, "#D:Value2");

        p = new NpgsqlParameter ("name", 5);
        p.Value = value;
        Assert.AreEqual (value, p.Value, "#E:Value1");
        Assert.AreEqual (DbType.String, p.DbType, "#E:DbType");
        Assert.AreEqual (NpgsqlDbType.Text, p.NpgsqlDbType, "#E:NpgsqlDbType");
        Assert.AreEqual (string_value, p.Value, "#E:Value2");

        p = new NpgsqlParameter ("name", NpgsqlDbType.Text);
        p.Value = value;
        Assert.AreEqual (NpgsqlDbType.Text, p.NpgsqlDbType, "#F:NpgsqlDbType");
        Assert.AreEqual (value, p.Value, "#F:Value");
    }

    [Test]
    public void InferType_Object()
    {
        Object value = new Object();

        NpgsqlParameter param = new NpgsqlParameter();
        param.Value = value;
        Assert.AreEqual(NpgsqlDbType.Variant, param.NpgsqlDbType, "#1");
        Assert.AreEqual(DbType.Object, param.DbType, "#2");
    }

    [Test]
    public void LocaleId ()
    {
        NpgsqlParameter parameter = new NpgsqlParameter ();
        Assert.AreEqual (0, parameter.LocaleId, "#1");
        parameter.LocaleId = 15;
        Assert.AreEqual(15, parameter.LocaleId, "#2");
    }
#endif

    [OneTimeSetUp]
    public async Task Bootstrap()
    {
        // Bootstrap datasource.
        await using (var _ = await OpenConnectionAsync()) {}
    }
}
