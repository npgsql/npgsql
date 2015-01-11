//
// NpgsqlParameterTest.cs - NUnit Test Cases for testing the
//                          NpgsqlParameter class
// Author:
//    Senganal T (tsenganal@novell.com)
//    Amit Biswas (amit@amitbiswas.com)
//    Gert Driesen (drieseng@users.sourceforge.net)
//
// Copyright (c) 2004 Novell Inc., and the individuals listed
// on the ChangeLog entries.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#define NET_2_0

using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading;
using System.Xml;
using Npgsql;
using NpgsqlTypes;

using NUnit.Framework;

namespace NpgsqlTests
{
    [TestFixture]
    public class NpgsqlParameterTest
    {
        [Test, Description("Makes sure that when NpgsqlDbType or Value/NpgsqlValue are set, DbType and NpgsqlDbType are set accordingly")]
        public void ImplicitSettingOfDbTypes()
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

        // Older tests

        /// <summary>
        /// Test which validates that Clear() indeed cleans up the parameters in a command so they can be added to other commands safely.
        /// </summary>
        [Test]
        public void NpgsqlParameterCollectionClearTest()
        {
            var p = new NpgsqlParameter();
            var c1 = new NpgsqlCommand();
            var c2 = new NpgsqlCommand();
            c1.Parameters.Add(p);
            Assert.AreEqual(1, c1.Parameters.Count);
            Assert.AreEqual(0, c2.Parameters.Count);
            c1.Parameters.Clear();
            Assert.AreEqual(0, c1.Parameters.Count);
            c2.Parameters.Add(p);
            Assert.AreEqual(0, c1.Parameters.Count);
            Assert.AreEqual(1, c2.Parameters.Count);
        }

        #region Constructors

        [Test]
        public void Constructor1()
        {
            var p = new NpgsqlParameter();
            Assert.AreEqual(DbType.Object, p.DbType, "DbType");
            Assert.AreEqual(ParameterDirection.Input, p.Direction, "Direction");
            Assert.IsFalse(p.IsNullable, "IsNullable");
#if NET_2_0
            //Assert.AreEqual (0, p.LocaleId, "LocaleId");
#endif
            Assert.AreEqual(string.Empty, p.ParameterName, "ParameterName");
            Assert.AreEqual(0, p.Precision, "Precision");
            Assert.AreEqual(0, p.Scale, "Scale");
            Assert.AreEqual(0, p.Size, "Size");
            Assert.AreEqual(string.Empty, p.SourceColumn, "SourceColumn");
#if NET_2_0
            Assert.IsFalse(p.SourceColumnNullMapping, "SourceColumnNullMapping");
#endif
            Assert.AreEqual(DataRowVersion.Current, p.SourceVersion, "SourceVersion");
            Assert.AreEqual(NpgsqlDbType.Unknown, p.NpgsqlDbType, "NpgsqlDbType");
#if NET_2_0
            Assert.IsNull(p.NpgsqlValue, "NpgsqlValue");
#endif
            Assert.IsNull(p.Value, "Value");
#if NET_2_0
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionDatabase, "XmlSchemaCollectionDatabase");
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionName, "XmlSchemaCollectionName");
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionOwningSchema, "XmlSchemaCollectionOwningSchema");
#endif
        }

        [Test]
        public void Constructor2_Value_DateTime()
        {
            var value = new DateTime(2004, 8, 24);

            var p = new NpgsqlParameter("address", value);
            Assert.AreEqual(DbType.DateTime, p.DbType, "B:DbType");
            Assert.AreEqual(ParameterDirection.Input, p.Direction, "B:Direction");
            Assert.IsFalse(p.IsNullable, "B:IsNullable");
#if NET_2_0
            //Assert.AreEqual (0, p.LocaleId, "B:LocaleId");
#endif
            Assert.AreEqual("address", p.ParameterName, "B:ParameterName");
            Assert.AreEqual(0, p.Precision, "B:Precision");
            Assert.AreEqual(0, p.Scale, "B:Scale");
            //Assert.AreEqual (0, p.Size, "B:Size");
            Assert.AreEqual(string.Empty, p.SourceColumn, "B:SourceColumn");
#if NET_2_0
            Assert.IsFalse(p.SourceColumnNullMapping, "B:SourceColumnNullMapping");
#endif
            Assert.AreEqual(DataRowVersion.Current, p.SourceVersion, "B:SourceVersion");
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "B:NpgsqlDbType");
#if NET_2_0
            // FIXME
            //Assert.AreEqual (new SqlDateTime (value), p.NpgsqlValue, "B:NpgsqlValue");
#endif
            Assert.AreEqual(value, p.Value, "B:Value");
#if NET_2_0
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionDatabase, "B:XmlSchemaCollectionDatabase");
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionName, "B:XmlSchemaCollectionName");
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionOwningSchema, "B:XmlSchemaCollectionOwningSchema");
#endif
        }

        [Test]
        public void Constructor2_Value_DBNull()
        {
            var p = new NpgsqlParameter("address", DBNull.Value);
            Assert.AreEqual(DbType.Object, p.DbType, "B:DbType");
            Assert.AreEqual(ParameterDirection.Input, p.Direction, "B:Direction");
            Assert.IsFalse(p.IsNullable, "B:IsNullable");
#if NET_2_0
            //Assert.AreEqual (0, p.LocaleId, "B:LocaleId");
#endif
            Assert.AreEqual("address", p.ParameterName, "B:ParameterName");
            Assert.AreEqual(0, p.Precision, "B:Precision");
            Assert.AreEqual(0, p.Scale, "B:Scale");
            Assert.AreEqual(0, p.Size, "B:Size");
            Assert.AreEqual(string.Empty, p.SourceColumn, "B:SourceColumn");
#if NET_2_0
            Assert.IsFalse(p.SourceColumnNullMapping, "B:SourceColumnNullMapping");
#endif
            Assert.AreEqual(DataRowVersion.Current, p.SourceVersion, "B:SourceVersion");
            Assert.AreEqual(NpgsqlDbType.Unknown, p.NpgsqlDbType, "B:NpgsqlDbType");
#if NET_2_0
            // FIXME
            //Assert.AreEqual (SqlString.Null, p.NpgsqlValue, "B:NpgsqlValue");
#endif
            Assert.AreEqual(DBNull.Value, p.Value, "B:Value");
#if NET_2_0
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionDatabase, "B:XmlSchemaCollectionDatabase");
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionName, "B:XmlSchemaCollectionName");
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionOwningSchema, "B:XmlSchemaCollectionOwningSchema");
#endif
        }

        [Test]
        public void Constructor2_Value_Null()
        {
            var p = new NpgsqlParameter("address", (Object) null);
            Assert.AreEqual(DbType.Object, p.DbType, "A:DbType");
            Assert.AreEqual(ParameterDirection.Input, p.Direction, "A:Direction");
            Assert.IsFalse(p.IsNullable, "A:IsNullable");
#if NET_2_0
            //Assert.AreEqual (0, p.LocaleId, "A:LocaleId");
#endif
            Assert.AreEqual("address", p.ParameterName, "A:ParameterName");
            Assert.AreEqual(0, p.Precision, "A:Precision");
            Assert.AreEqual(0, p.Scale, "A:Scale");
            Assert.AreEqual(0, p.Size, "A:Size");
            Assert.AreEqual(string.Empty, p.SourceColumn, "A:SourceColumn");
#if NET_2_0
            Assert.IsFalse(p.SourceColumnNullMapping, "A:SourceColumnNullMapping");
#endif
            Assert.AreEqual(DataRowVersion.Current, p.SourceVersion, "A:SourceVersion");
            Assert.AreEqual(NpgsqlDbType.Unknown, p.NpgsqlDbType, "A:NpgsqlDbType");
#if NET_2_0
            Assert.IsNull(p.NpgsqlValue, "A:NpgsqlValue");
#endif
            Assert.IsNull(p.Value, "A:Value");
#if NET_2_0
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionDatabase, "A:XmlSchemaCollectionDatabase");
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionName, "A:XmlSchemaCollectionName");
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionOwningSchema, "A:XmlSchemaCollectionOwningSchema");
#endif
        }

#if NET_2_0
        [Test]
        //.ctor (String, NpgsqlDbType, Int32, String, ParameterDirection, bool, byte, byte, DataRowVersion, object)
        public void Constructor7()
        {
            var p1 = new NpgsqlParameter("p1Name", NpgsqlDbType.Varchar, 20,
                                         "srcCol", ParameterDirection.InputOutput, false, 0, 0,
                                         DataRowVersion.Original, "foo");
            Assert.AreEqual(DbType.String, p1.DbType, "DbType");
            Assert.AreEqual(ParameterDirection.InputOutput, p1.Direction, "Direction");
            Assert.AreEqual(false, p1.IsNullable, "IsNullable");
            //Assert.AreEqual (999, p1.LocaleId, "#");
            Assert.AreEqual("p1Name", p1.ParameterName, "ParameterName");
            Assert.AreEqual(0, p1.Precision, "Precision");
            Assert.AreEqual(0, p1.Scale, "Scale");
            Assert.AreEqual(20, p1.Size, "Size");
            Assert.AreEqual("srcCol", p1.SourceColumn, "SourceColumn");
            Assert.AreEqual(false, p1.SourceColumnNullMapping, "SourceColumnNullMapping");
            Assert.AreEqual(DataRowVersion.Original, p1.SourceVersion, "SourceVersion");
            Assert.AreEqual(NpgsqlDbType.Varchar, p1.NpgsqlDbType, "NpgsqlDbType");
            //Assert.AreEqual (3210, p1.NpgsqlValue, "#");
            Assert.AreEqual("foo", p1.Value, "Value");
            //Assert.AreEqual ("database", p1.XmlSchemaCollectionDatabase, "XmlSchemaCollectionDatabase");
            //Assert.AreEqual ("name", p1.XmlSchemaCollectionName, "XmlSchemaCollectionName");
            //Assert.AreEqual ("schema", p1.XmlSchemaCollectionOwningSchema, "XmlSchemaCollectionOwningSchema");
        }
#endif

        #endregion

#if NeedsPorting

        [Test]
#if NET_2_0
        [Category ("NotWorking")]
#endif
        public void InferType_Char()
        {
            Char value = 'X';

#if NET_2_0
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
#else
            NpgsqlParameter p = new NpgsqlParameter();
            try
            {
                p.Value = value;
                Assert.Fail("#1");
            }
            catch (ArgumentException ex)
            {
                // The parameter data type of Char is invalid
                Assert.AreEqual(typeof(ArgumentException), ex.GetType(), "#2");
                Assert.IsNull(ex.InnerException, "#3");
                Assert.IsNotNull(ex.Message, "#4");
                Assert.IsNull(ex.ParamName, "#5");
            }
#endif
        }

        [Test]
#if NET_2_0
        [Category ("NotWorking")]
#endif
        public void InferType_CharArray()
        {
            Char[] value = new Char[] { 'A', 'X' };

#if NET_2_0
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
#else
            NpgsqlParameter p = new NpgsqlParameter();
            try
            {
                p.Value = value;
                Assert.Fail("#1");
            }
            catch (FormatException)
            {
                // appears to be bug in .NET 1.1 while constructing
                // exception message
            }
            catch (ArgumentException ex)
            {
                // The parameter data type of Char[] is invalid
                Assert.AreEqual(typeof(ArgumentException), ex.GetType(), "#2");
                Assert.IsNull(ex.InnerException, "#3");
                Assert.IsNotNull(ex.Message, "#4");
                Assert.IsNull(ex.ParamName, "#5");
            }
#endif
        }

#endif

        [Test]
        [Ignore]
#if NET_2_0
        [Category("NotWorking")]
#endif
        public void InferType_Invalid()
        {
            var notsupported = new object[]
                                        {
                                            UInt16.MaxValue,
                                            UInt32.MaxValue,
                                            UInt64.MaxValue,
                                            SByte.MaxValue,
                                            new NpgsqlParameter()
                                        };

            var param = new NpgsqlParameter();

            for (var i = 0; i < notsupported.Length; i++)
            {
#if NET_2_0
                param.Value = notsupported[i];
                try
                {
                    var type = param.NpgsqlDbType;
                    Assert.Fail("#A1:" + i + " (" + type + ")");
                }
                catch (ArgumentException ex)
                {
                    // The parameter data type of ... is invalid
                    Assert.AreEqual(typeof (ArgumentException), ex.GetType(), "#A2");
                    Assert.IsNull(ex.InnerException, "#A3");
                    Assert.IsNotNull(ex.Message, "#A4");
                    Assert.IsNull(ex.ParamName, "#A5");
                }

                try
                {
                    var type = param.DbType;
                    Assert.Fail("#B1:" + i + " (" + type + ")");
                }
                catch (ArgumentException ex)
                {
                    // The parameter data type of ... is invalid
                    Assert.AreEqual(typeof (ArgumentException), ex.GetType(), "#B2");
                    Assert.IsNull(ex.InnerException, "#B3");
                    Assert.IsNotNull(ex.Message, "#B4");
                    Assert.IsNull(ex.ParamName, "#B5");
                }
#else
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
                    Assert.AreEqual(typeof(ArgumentException), ex.GetType(), "#A2");
                    Assert.IsNull(ex.InnerException, "#A3");
                    Assert.IsNotNull(ex.Message, "#A4");
                    Assert.IsNull(ex.ParamName, "#A5");
                }
#endif
            }
        }

#if NeedsPorting
        [Test]
        public void InferType_Object()
        {
            Object value = new Object();

            NpgsqlParameter param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(NpgsqlDbType.Variant, param.NpgsqlDbType, "#1");
            Assert.AreEqual(DbType.Object, param.DbType, "#2");
        }
#endif

#if NeedsPorting
#if NET_2_0
        [Test]
        public void LocaleId ()
        {
            NpgsqlParameter parameter = new NpgsqlParameter ();
            Assert.AreEqual (0, parameter.LocaleId, "#1");
            parameter.LocaleId = 15;
            Assert.AreEqual(15, parameter.LocaleId, "#2");
        }
#endif
#endif

        [Test] // bug #320196
        public void ParameterNullTest()
        {
            var param = new NpgsqlParameter("param", NpgsqlDbType.Numeric);
            Assert.AreEqual(0, param.Scale, "#A1");
            param.Value = DBNull.Value;
            Assert.AreEqual(0, param.Scale, "#A2");

            param = new NpgsqlParameter("param", NpgsqlDbType.Integer);
            Assert.AreEqual(0, param.Scale, "#B1");
            param.Value = DBNull.Value;
            Assert.AreEqual(0, param.Scale, "#B2");
        }

        [Test]
        [Ignore]
        public void ParameterType()
        {
            NpgsqlParameter p;

            // If Type is not set, then type is inferred from the value
            // assigned. The Type should be inferred everytime Value is assigned
            // If value is null or DBNull, then the current Type should be reset to Text.
            p = new NpgsqlParameter();
            Assert.AreEqual(DbType.String, p.DbType, "#A1");
            Assert.AreEqual(NpgsqlDbType.Text, p.NpgsqlDbType, "#A2");
            p.Value = DBNull.Value;
            Assert.AreEqual(DbType.String, p.DbType, "#B1");
            Assert.AreEqual(NpgsqlDbType.Text, p.NpgsqlDbType, "#B2");
            p.Value = 1;
            Assert.AreEqual(DbType.Int32, p.DbType, "#C1");
            Assert.AreEqual(NpgsqlDbType.Integer, p.NpgsqlDbType, "#C2");
            p.Value = DBNull.Value;
#if NET_2_0
            Assert.AreEqual(DbType.String, p.DbType, "#D1");
            Assert.AreEqual(NpgsqlDbType.Text, p.NpgsqlDbType, "#D2");
#else
            Assert.AreEqual(DbType.Int32, p.DbType, "#D1");
            Assert.AreEqual(NpgsqlDbType.Integer, p.NpgsqlDbType, "#D2");
#endif
            p.Value = new byte[] {0x0a};
            Assert.AreEqual(DbType.Binary, p.DbType, "#E1");
            Assert.AreEqual(NpgsqlDbType.Bytea, p.NpgsqlDbType, "#E2");
            p.Value = null;
#if NET_2_0
            Assert.AreEqual(DbType.String, p.DbType, "#F1");
            Assert.AreEqual(NpgsqlDbType.Text, p.NpgsqlDbType, "#F2");
#else
            Assert.AreEqual(DbType.Binary, p.DbType, "#F1");
            Assert.AreEqual(NpgsqlDbType.VarBinary, p.NpgsqlDbType, "#F2");
#endif
            p.Value = DateTime.Now;
            Assert.AreEqual(DbType.DateTime, p.DbType, "#G1");
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "#G2");
            p.Value = null;
#if NET_2_0
            Assert.AreEqual(DbType.String, p.DbType, "#H1");
            Assert.AreEqual(NpgsqlDbType.Text, p.NpgsqlDbType, "#H2");
#else
            Assert.AreEqual(DbType.DateTime, p.DbType, "#H1");
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "#H2");
#endif

            // If DbType is set, then the NpgsqlDbType should not be
            // inferred from the value assigned.
            p = new NpgsqlParameter();
            p.DbType = DbType.DateTime;
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "#I1");
            p.Value = 1;
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "#I2");
            p.Value = null;
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "#I3");
            p.Value = DBNull.Value;
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "#I4");

            // If NpgsqlDbType is set, then the DbType should not be
            // inferred from the value assigned.
            p = new NpgsqlParameter();
            p.NpgsqlDbType = NpgsqlDbType.Bytea;
            Assert.AreEqual(NpgsqlDbType.Bytea, p.NpgsqlDbType, "#J1");
            p.Value = 1;
            Assert.AreEqual(NpgsqlDbType.Bytea, p.NpgsqlDbType, "#J2");
            p.Value = null;
            Assert.AreEqual(NpgsqlDbType.Bytea, p.NpgsqlDbType, "#J3");
            p.Value = DBNull.Value;
            Assert.AreEqual(NpgsqlDbType.Bytea, p.NpgsqlDbType, "#J4");
        }

        [Test]
        [Ignore]
        public void ParameterName()
        {
            var p = new NpgsqlParameter();
            p.ParameterName = "name";
            Assert.AreEqual("name", p.ParameterName, "#A:ParameterName");
            Assert.AreEqual(string.Empty, p.SourceColumn, "#A:SourceColumn");

            p.ParameterName = null;
            Assert.AreEqual(string.Empty, p.ParameterName, "#B:ParameterName");
            Assert.AreEqual(string.Empty, p.SourceColumn, "#B:SourceColumn");

            p.ParameterName = " ";
            Assert.AreEqual(" ", p.ParameterName, "#C:ParameterName");
            Assert.AreEqual(string.Empty, p.SourceColumn, "#C:SourceColumn");

            p.ParameterName = " name ";
            Assert.AreEqual(" name ", p.ParameterName, "#D:ParameterName");
            Assert.AreEqual(string.Empty, p.SourceColumn, "#D:SourceColumn");

            p.ParameterName = string.Empty;
            Assert.AreEqual(string.Empty, p.ParameterName, "#E:ParameterName");
            Assert.AreEqual(string.Empty, p.SourceColumn, "#E:SourceColumn");
        }

#if NET_2_0
        [Test]
        public void ResetDbType()
        {
            NpgsqlParameter p;

            //Parameter with an assigned value but no DbType specified
            p = new NpgsqlParameter("foo", 42);
            p.ResetDbType();
            Assert.AreEqual(DbType.Int32, p.DbType, "#A:DbType");
            Assert.AreEqual(NpgsqlDbType.Integer, p.NpgsqlDbType, "#A:NpgsqlDbType");
            Assert.AreEqual(42, p.Value, "#A:Value");

            p.DbType = DbType.DateTime; //assigning a DbType
            Assert.AreEqual(DbType.DateTime, p.DbType, "#B:DbType1");
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "#B:SqlDbType1");
            p.ResetDbType();
            Assert.AreEqual(DbType.Int32, p.DbType, "#B:DbType2");
            Assert.AreEqual(NpgsqlDbType.Integer, p.NpgsqlDbType, "#B:SqlDbtype2");

            //Parameter with an assigned NpgsqlDbType but no specified value
            p = new NpgsqlParameter("foo", NpgsqlDbType.Integer);
            p.ResetDbType();
            Assert.AreEqual(DbType.Object, p.DbType, "#C:DbType");
            Assert.AreEqual(NpgsqlDbType.Unknown, p.NpgsqlDbType, "#C:NpgsqlDbType");

            p.DbType = DbType.DateTime; //assigning a NpgsqlDbType
            Assert.AreEqual(DbType.DateTime, p.DbType, "#D:DbType1");
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "#D:SqlDbType1");
            p.ResetDbType();
            Assert.AreEqual(DbType.Object, p.DbType, "#D:DbType2");
            Assert.AreEqual(NpgsqlDbType.Unknown, p.NpgsqlDbType, "#D:SqlDbType2");

            p = new NpgsqlParameter();
            p.Value = DateTime.MaxValue;
            Assert.AreEqual(DbType.DateTime, p.DbType, "#E:DbType1");
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "#E:SqlDbType1");
            p.Value = null;
            p.ResetDbType();
            Assert.AreEqual(DbType.Object, p.DbType, "#E:DbType2");
            Assert.AreEqual(NpgsqlDbType.Unknown, p.NpgsqlDbType, "#E:SqlDbType2");

            p = new NpgsqlParameter("foo", NpgsqlDbType.Varchar);
            p.Value = DateTime.MaxValue;
            p.ResetDbType();
            Assert.AreEqual(DbType.DateTime, p.DbType, "#F:DbType");
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "#F:NpgsqlDbType");
            Assert.AreEqual(DateTime.MaxValue, p.Value, "#F:Value");

            p = new NpgsqlParameter("foo", NpgsqlDbType.Varchar);
            p.Value = DBNull.Value;
            p.ResetDbType();
            Assert.AreEqual(DbType.Object, p.DbType, "#G:DbType");
            Assert.AreEqual(NpgsqlDbType.Unknown, p.NpgsqlDbType, "#G:NpgsqlDbType");
            Assert.AreEqual(DBNull.Value, p.Value, "#G:Value");

            p = new NpgsqlParameter("foo", NpgsqlDbType.Varchar);
            p.Value = null;
            p.ResetDbType();
            Assert.AreEqual(DbType.Object, p.DbType, "#G:DbType");
            Assert.AreEqual(NpgsqlDbType.Unknown, p.NpgsqlDbType, "#G:NpgsqlDbType");
            Assert.IsNull(p.Value, "#G:Value");
        }

#if NeedsPorting
        [Test]
        public void ResetSqlDbType ()
        {
            //Parameter with an assigned value but no NpgsqlDbType specified
            NpgsqlParameter p1 = new NpgsqlParameter ("foo", 42);
            Assert.AreEqual (42, p1.Value, "#1");
            Assert.AreEqual (DbType.Int32, p1.DbType, "#2");
            Assert.AreEqual (NpgsqlDbType.Integer, p1.NpgsqlDbType, "#3");

            p1.ResetSqlDbType ();
            Assert.AreEqual (DbType.Int32, p1.DbType, "#4 The parameter with value 42 must have DbType as Int32");
            Assert.AreEqual (NpgsqlDbType.Integer, p1.NpgsqlDbType, "#5 The parameter with value 42 must have NpgsqlDbType as Int");

            p1.NpgsqlDbType = NpgsqlDbType.Timestamp; //assigning a NpgsqlDbType
            Assert.AreEqual (DbType.DateTime, p1.DbType, "#6");
            Assert.AreEqual (NpgsqlDbType.Timestamp, p1.NpgsqlDbType, "#7");
            p1.ResetSqlDbType (); //Resetting NpgsqlDbType
            Assert.AreEqual (DbType.Int32, p1.DbType, "#8 Resetting NpgsqlDbType must infer the type from the value");
            Assert.AreEqual (NpgsqlDbType.Integer, p1.NpgsqlDbType, "#9 Resetting NpgsqlDbType must infer the type from the value");

            //Parameter with an assigned NpgsqlDbType but no specified value
            NpgsqlParameter p2 = new NpgsqlParameter ("foo", NpgsqlDbType.Integer);
            Assert.AreEqual (null, p2.Value, "#10");
            Assert.AreEqual (DbType.Int32, p2.DbType, "#11");
            Assert.AreEqual (NpgsqlDbType.Integer, p2.NpgsqlDbType, "#12");

            //Although a NpgsqlDbType is specified, calling ResetSqlDbType resets
            //the NpgsqlDbType and DbType properties to default values
            p2.ResetSqlDbType ();
            Assert.AreEqual (DbType.String, p2.DbType, "#13 Resetting NpgsqlDbType must infer the type from the value");
            Assert.AreEqual (NpgsqlDbType.Text, p2.NpgsqlDbType, "#14 Resetting NpgsqlDbType must infer the type from the value");

            p2.NpgsqlDbType = NpgsqlDbType.Timestamp; //assigning a NpgsqlDbType
            Assert.AreEqual (DbType.DateTime, p2.DbType, "#15");
            Assert.AreEqual (NpgsqlDbType.Timestamp, p2.NpgsqlDbType, "#16");
            p2.ResetSqlDbType (); //Resetting NpgsqlDbType
            Assert.AreEqual (DbType.String, p2.DbType, "#17 Resetting NpgsqlDbType must infer the type from the value");
            Assert.AreEqual (NpgsqlDbType.Text, p2.NpgsqlDbType, "#18 Resetting NpgsqlDbType must infer the type from the value");
        }
#endif
#endif

        [Test]
        [Ignore]
        public void SourceColumn()
        {
            var p = new NpgsqlParameter();
            p.SourceColumn = "name";
            Assert.AreEqual(string.Empty, p.ParameterName, "#A:ParameterName");
            Assert.AreEqual("name", p.SourceColumn, "#A:SourceColumn");

            p.SourceColumn = null;
            Assert.AreEqual(string.Empty, p.ParameterName, "#B:ParameterName");
            Assert.AreEqual(string.Empty, p.SourceColumn, "#B:SourceColumn");

            p.SourceColumn = " ";
            Assert.AreEqual(string.Empty, p.ParameterName, "#C:ParameterName");
            Assert.AreEqual(" ", p.SourceColumn, "#C:SourceColumn");

            p.SourceColumn = " name ";
            Assert.AreEqual(string.Empty, p.ParameterName, "#D:ParameterName");
            Assert.AreEqual(" name ", p.SourceColumn, "#D:SourceColumn");

            p.SourceColumn = string.Empty;
            Assert.AreEqual(string.Empty, p.ParameterName, "#E:ParameterName");
            Assert.AreEqual(string.Empty, p.SourceColumn, "#E:SourceColumn");
        }

#if NET_2_0
        [Test]
        public void SourceColumnNullMapping()
        {
            var p = new NpgsqlParameter();
            Assert.IsFalse(p.SourceColumnNullMapping, "#1");
            p.SourceColumnNullMapping = true;
            Assert.IsTrue(p.SourceColumnNullMapping, "#2");
            p.SourceColumnNullMapping = false;
            Assert.IsFalse(p.SourceColumnNullMapping, "#3");
        }
#endif

        [Test]
        public void Bug1011100NpgsqlDbTypeTest()
        {
            var p = new NpgsqlParameter();
            p.Value = DBNull.Value;
            Assert.AreEqual(DbType.Object, p.DbType, "#A:DbType");
            Assert.AreEqual(NpgsqlDbType.Unknown, p.NpgsqlDbType, "#A:NpgsqlDbType");

            // Now change parameter value.
            // Note that as we didn't explicitly specified a dbtype, the dbtype property should change when
            // the value changes...

            p.Value = 8;

            Assert.AreEqual(DbType.Int32, p.DbType, "#A:DbType");
            Assert.AreEqual(NpgsqlDbType.Integer, p.NpgsqlDbType, "#A:NpgsqlDbType");

            //Assert.AreEqual(3510, p.Value, "#A:Value");
            //p.NpgsqlDbType = NpgsqlDbType.Varchar;
            //Assert.AreEqual(DbType.String, p.DbType, "#B:DbType");
            //Assert.AreEqual(NpgsqlDbType.Varchar, p.NpgsqlDbType, "#B:NpgsqlDbType");
            //Assert.AreEqual(3510, p.Value, "#B:Value");
        }


#if NET_2_0
#if NeedsPorting
        [Test]
        public void XmlSchemaTest ()
        {
            NpgsqlParameter p1 = new NpgsqlParameter ();

            //Testing default values
            Assert.AreEqual (String.Empty, p1.XmlSchemaCollectionDatabase,
                     "#1 Default value for XmlSchemaCollectionDatabase is an empty string");
            Assert.AreEqual (String.Empty, p1.XmlSchemaCollectionName,
                     "#2 Default value for XmlSchemaCollectionName is an empty string");
            Assert.AreEqual (String.Empty, p1.XmlSchemaCollectionOwningSchema,
                     "#3 Default value for XmlSchemaCollectionOwningSchema is an empty string");

            //Changing one property should not affect the remaining two properties
            p1.XmlSchemaCollectionDatabase = "database";
            Assert.AreEqual ("database", p1.XmlSchemaCollectionDatabase,
                     "#4 Default value for XmlSchemaCollectionDatabase is an empty string");
            Assert.AreEqual (String.Empty, p1.XmlSchemaCollectionName,
                     "#5 Default value for XmlSchemaCollectionName is an empty string");
            Assert.AreEqual (String.Empty, p1.XmlSchemaCollectionOwningSchema,
                     "#6 Default value for XmlSchemaCollectionOwningSchema is an empty string");

            p1.XmlSchemaCollectionName = "name";
            Assert.AreEqual ("database", p1.XmlSchemaCollectionDatabase,
                     "#7 Default value for XmlSchemaCollectionDatabase is an empty string");
            Assert.AreEqual ("name", p1.XmlSchemaCollectionName,
                     "#8 Default value for XmlSchemaCollectionName is an empty string");
            Assert.AreEqual (String.Empty, p1.XmlSchemaCollectionOwningSchema,
                     "#9 Default value for XmlSchemaCollectionOwningSchema is an empty string");

            p1.XmlSchemaCollectionOwningSchema = "schema";
            Assert.AreEqual ("database", p1.XmlSchemaCollectionDatabase,
                     "#10 Default value for XmlSchemaCollectionDatabase is an empty string");
            Assert.AreEqual ("name", p1.XmlSchemaCollectionName,
                     "#11 Default value for XmlSchemaCollectionName is an empty string");
            Assert.AreEqual ("schema", p1.XmlSchemaCollectionOwningSchema,
                     "#12 Default value for XmlSchemaCollectionOwningSchema is an empty string");

            //assigning null value stores default empty string
            p1.XmlSchemaCollectionDatabase = null;
            Assert.AreEqual (String.Empty, p1.XmlSchemaCollectionDatabase,
                     "#13 Default value for XmlSchemaCollectionDatabase is an empty string");
            Assert.AreEqual ("name", p1.XmlSchemaCollectionName,
                     "#14 Default value for XmlSchemaCollectionName is an empty string");
            Assert.AreEqual ("schema", p1.XmlSchemaCollectionOwningSchema,
                     "#15 Default value for XmlSchemaCollectionOwningSchema is an empty string");

            p1.XmlSchemaCollectionName = "";
            Assert.AreEqual (String.Empty, p1.XmlSchemaCollectionDatabase,
                     "#16 Default value for XmlSchemaCollectionDatabase is an empty string");
            Assert.AreEqual ("", p1.XmlSchemaCollectionName,
                     "#17 Default value for XmlSchemaCollectionName is an empty string");
            Assert.AreEqual ("schema", p1.XmlSchemaCollectionOwningSchema,
                     "#18 Default value for XmlSchemaCollectionOwningSchema is an empty string");

            //values are not trimmed
            p1.XmlSchemaCollectionOwningSchema = "  a  ";
            Assert.AreEqual (String.Empty, p1.XmlSchemaCollectionDatabase,
                     "#19 Default value for XmlSchemaCollectionDatabase is an empty string");
            Assert.AreEqual ("", p1.XmlSchemaCollectionName,
                     "#20 Default value for XmlSchemaCollectionName is an empty string");
            Assert.AreEqual ("  a  ", p1.XmlSchemaCollectionOwningSchema,
                     "#21 Default value for XmlSchemaCollectionOwningSchema is an empty string");
        }
#endif
#endif

        [Test]
        public void ParameterCollectionHashLookupParameterRenameBug()
        {
            using (var command = new NpgsqlCommand())
            {
                // Put plenty of parameters in the collection to turn on hash lookup functionality.
                for (int i = 0 ; i < 10 ; i++)
                {
                    command.Parameters.AddWithValue(string.Format("p{0:00}", i + 1), NpgsqlDbType.Text, string.Format("String parameter value {0}", i + 1));
                }

                // Make sure both hash lookups have been generated.
                Assert.AreEqual(command.Parameters["p03"].ParameterName, "p03");
                Assert.AreEqual(command.Parameters["P03"].ParameterName, "p03");

                // Rename the target parameter.
                command.Parameters["p03"].ParameterName = "a_new_name";

                try
                {
                    // Try to exploit the hash lookup bug.
                    // If the bug exists, the hash lookups will be out of sync with the list, and be unable
                    // to find the parameter by its new name.
                    Assert.IsTrue(command.Parameters.IndexOf("a_new_name") >= 0);
                }
                catch (Exception e)
                {
                    throw new Exception("NpgsqlParameterCollection hash lookup/parameter rename bug detected", e);
                }
            }
        }

        [Test]
        public void NpgsqlParameterCloneTest()
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

            Assert.AreEqual(param.Value, newParam.Value);
            Assert.AreEqual(param.Precision, newParam.Precision);
            Assert.AreEqual(param.Scale, newParam.Scale);
            Assert.AreEqual(param.Size, newParam.Size);
            Assert.AreEqual(param.Direction, newParam.Direction);
            Assert.AreEqual(param.IsNullable, newParam.IsNullable);
            Assert.AreEqual(param.ParameterName, newParam.ParameterName);
            Assert.AreEqual(param.SourceColumn, newParam.SourceColumn);
            Assert.AreEqual(param.SourceVersion, newParam.SourceVersion);
            Assert.AreEqual(param.NpgsqlValue, newParam.NpgsqlValue);
            Assert.AreEqual(param.SourceColumnNullMapping, newParam.SourceColumnNullMapping);
            Assert.AreEqual(param.NpgsqlValue, newParam.NpgsqlValue);

        }
    }
}
