//
// NpgsqlParameterTest.cs - NUnit Test Cases for testing the
//                          NpgsqlParameter class
// Author:
//	Senganal T (tsenganal@novell.com)
//	Amit Biswas (amit@amitbiswas.com)
//	Gert Driesen (drieseng@users.sourceforge.net)
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
using System.Xml;
using Npgsql;
using NpgsqlTypes;

using NUnit.Framework;

namespace NpgsqlTests
{
    [TestFixture]
    public class NpgsqlParameterTest
    {
        [Test]
        public void Constructor1()
        {
            NpgsqlParameter p = new NpgsqlParameter();
            Assert.AreEqual(DbType.String, p.DbType, "DbType");
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
			Assert.IsFalse (p.SourceColumnNullMapping, "SourceColumnNullMapping");
#endif
            Assert.AreEqual(DataRowVersion.Current, p.SourceVersion, "SourceVersion");
            Assert.AreEqual(NpgsqlDbType.Text, p.NpgsqlDbType, "NpgsqlDbType");
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
            DateTime value = new DateTime(2004, 8, 24);

            NpgsqlParameter p = new NpgsqlParameter("address", value);
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
			Assert.IsFalse (p.SourceColumnNullMapping, "B:SourceColumnNullMapping");
#endif
            Assert.AreEqual(DataRowVersion.Current, p.SourceVersion, "B:SourceVersion");
            Assert.AreEqual(NpgsqlDbType.Timestamp, p.NpgsqlDbType, "B:NpgsqlDbType");
#if NET_2_0
			// FIXME
			//Assert.AreEqual (new SqlDateTime (value), p.SqlValue, "B:SqlValue");
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
            NpgsqlParameter p = new NpgsqlParameter("address", DBNull.Value);
            Assert.AreEqual(DbType.String, p.DbType, "B:DbType");
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
			Assert.IsFalse (p.SourceColumnNullMapping, "B:SourceColumnNullMapping");
#endif
            Assert.AreEqual(DataRowVersion.Current, p.SourceVersion, "B:SourceVersion");
            Assert.AreEqual(NpgsqlDbType.Text, p.NpgsqlDbType, "B:NpgsqlDbType");
#if NET_2_0
			// FIXME
			//Assert.AreEqual (SqlString.Null, p.SqlValue, "B:SqlValue");
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
            NpgsqlParameter p = new NpgsqlParameter("address", (Object)null);
            Assert.AreEqual(DbType.String, p.DbType, "A:DbType");
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
			Assert.IsFalse (p.SourceColumnNullMapping, "A:SourceColumnNullMapping");
#endif
            Assert.AreEqual(DataRowVersion.Current, p.SourceVersion, "A:SourceVersion");
            Assert.AreEqual(NpgsqlDbType.Text, p.NpgsqlDbType, "A:NpgsqlDbType");
#if NET_2_0
			Assert.IsNull (p.NpgsqlValue, "A:NpgsqlValue");
#endif
            Assert.IsNull(p.Value, "A:Value");
#if NET_2_0
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionDatabase, "A:XmlSchemaCollectionDatabase");
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionName, "A:XmlSchemaCollectionName");
            //Assert.AreEqual (string.Empty, p.XmlSchemaCollectionOwningSchema, "A:XmlSchemaCollectionOwningSchema");
#endif
        }

#if NET_2_0
		[Test] //.ctor (String, NpgsqlDbType, Int32, String, ParameterDirection, bool, byte, byte, DataRowVersion, object)
		public void Constructor7 ()
		{
			NpgsqlParameter p1 = new NpgsqlParameter ("p1Name", NpgsqlDbType.Varchar, 20,
							    "srcCol", ParameterDirection.InputOutput, false, 0, 0,
							    DataRowVersion.Original, "foo");
			Assert.AreEqual (DbType.String, p1.DbType, "DbType");
			Assert.AreEqual (ParameterDirection.InputOutput, p1.Direction, "Direction");
			Assert.AreEqual (false, p1.IsNullable, "IsNullable");
			//Assert.AreEqual (999, p1.LocaleId, "#");
			Assert.AreEqual ("p1Name", p1.ParameterName, "ParameterName");
			Assert.AreEqual (0, p1.Precision, "Precision");
			Assert.AreEqual (0, p1.Scale, "Scale");
			Assert.AreEqual (20, p1.Size, "Size");
			Assert.AreEqual ("srcCol", p1.SourceColumn, "SourceColumn");
			Assert.AreEqual (false, p1.SourceColumnNullMapping, "SourceColumnNullMapping");
			Assert.AreEqual (DataRowVersion.Original, p1.SourceVersion, "SourceVersion");
            Assert.AreEqual(NpgsqlDbType.Varchar, p1.NpgsqlDbType, "NpgsqlDbType");
			//Assert.AreEqual (3210, p1.SqlValue, "#");
			Assert.AreEqual ("foo", p1.Value, "Value");
            //Assert.AreEqual ("database", p1.XmlSchemaCollectionDatabase, "XmlSchemaCollectionDatabase");
            //Assert.AreEqual ("name", p1.XmlSchemaCollectionName, "XmlSchemaCollectionName");
            //Assert.AreEqual ("schema", p1.XmlSchemaCollectionOwningSchema, "XmlSchemaCollectionOwningSchema");
		}

        //[Test]
        //public void CompareInfo ()
        //{
        //    NpgsqlParameter parameter = new NpgsqlParameter ();
        //    Assert.AreEqual (SqlCompareOptions.None, parameter.CompareInfo, "#1");
        //    parameter.CompareInfo = SqlCompareOptions.IgnoreNonSpace;
        //    Assert.AreEqual (SqlCompareOptions.IgnoreNonSpace, parameter.CompareInfo, "#2");
        //}
#endif

        [Test]
        public void InferType_Byte()
        {
            Byte value = 0x0a;

            NpgsqlParameter param = new NpgsqlParameter();
            param.Value = value;
            // no support for byte
            Assert.AreEqual(NpgsqlDbType.Smallint, param.NpgsqlDbType, "#1");
            Assert.AreEqual(DbType.Int16, param.DbType, "#2");
        }

        [Test]
        public void InferType_ByteArray()
        {
            Byte[] value = new Byte[] { 0x0a, 0x0d };

            NpgsqlParameter param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(NpgsqlDbType.Bytea, param.NpgsqlDbType, "#1");
            Assert.AreEqual(DbType.Binary, param.DbType, "#2");
        }

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
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (DbType.String, p.DbType, "#A:DbType");
			Assert.AreEqual (string_value, p.Value, "#A:Value");

			p = new NpgsqlParameter ();
			p.Value = value;
			Assert.AreEqual (value, p.Value, "#B:Value1");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (string_value, p.Value, "#B:Value2");

			p = new NpgsqlParameter ();
			p.Value = value;
			Assert.AreEqual (value, p.Value, "#C:Value1");
			Assert.AreEqual (DbType.String, p.DbType, "#C:DbType");
			Assert.AreEqual (string_value, p.Value, "#C:Value2");

			p = new NpgsqlParameter ("name", value);
			Assert.AreEqual (value, p.Value, "#D:Value1");
			Assert.AreEqual (DbType.String, p.DbType, "#D:DbType");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#D:SqlDbType");
			Assert.AreEqual (string_value, p.Value, "#D:Value2");

			p = new NpgsqlParameter ("name", 5);
			p.Value = value;
			Assert.AreEqual (value, p.Value, "#E:Value1");
			Assert.AreEqual (DbType.String, p.DbType, "#E:DbType");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#E:SqlDbType");
			Assert.AreEqual (string_value, p.Value, "#E:Value2");

			p = new NpgsqlParameter ("name", SqlDbType.NVarChar);
			p.Value = value;
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#F:SqlDbType");
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
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (DbType.String, p.DbType, "#A:DbType");
			Assert.AreEqual (string_value, p.Value, "#A:Value2");

			p = new NpgsqlParameter ();
			p.Value = value;
			Assert.AreEqual (value, p.Value, "#B:Value1");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (string_value, p.Value, "#B:Value2");

			p = new NpgsqlParameter ();
			p.Value = value;
			Assert.AreEqual (value, p.Value, "#C:Value1");
			Assert.AreEqual (DbType.String, p.DbType, "#C:DbType");
			Assert.AreEqual (string_value, p.Value, "#C:Value2");

			p = new NpgsqlParameter ("name", value);
			Assert.AreEqual (value, p.Value, "#D:Value1");
			Assert.AreEqual (DbType.String, p.DbType, "#D:DbType");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#D:SqlDbType");
			Assert.AreEqual (string_value, p.Value, "#D:Value2");

			p = new NpgsqlParameter ("name", 5);
			p.Value = value;
			Assert.AreEqual (value, p.Value, "#E:Value1");
			Assert.AreEqual (DbType.String, p.DbType, "#E:DbType");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#E:SqlDbType");
			Assert.AreEqual (string_value, p.Value, "#E:Value2");

			p = new NpgsqlParameter ("name", SqlDbType.NVarChar);
			p.Value = value;
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#F:SqlDbType");
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

        [Test]
        public void InferType_DateTime()
        {
            DateTime value;
            NpgsqlParameter param;

            value = DateTime.Now;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.DateTime, param.SqlDbType, "#A1");
            Assert.AreEqual(DbType.DateTime, param.DbType, "#A2");

            value = DateTime.Now;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.DateTime, param.SqlDbType, "#B1");
            Assert.AreEqual(DbType.DateTime, param.DbType, "#B2");

            value = new DateTime(1973, 8, 13);
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.DateTime, param.SqlDbType, "#C1");
            Assert.AreEqual(DbType.DateTime, param.DbType, "#C2");
        }

        [Test]
        public void InferType_Decimal()
        {
            Decimal value;
            NpgsqlParameter param;

            value = Decimal.MaxValue;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Decimal, param.SqlDbType, "#A1");
            Assert.AreEqual(DbType.Decimal, param.DbType, "#A2");

            value = Decimal.MinValue;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Decimal, param.SqlDbType, "#B1");
            Assert.AreEqual(DbType.Decimal, param.DbType, "#B2");

            value = 214748.364m;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Decimal, param.SqlDbType, "#C1");
            Assert.AreEqual(DbType.Decimal, param.DbType, "#C2");
        }

        [Test]
        public void InferType_Double()
        {
            Double value;
            NpgsqlParameter param;

            value = Double.MaxValue;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Float, param.SqlDbType, "#A1");
            Assert.AreEqual(DbType.Double, param.DbType, "#A2");

            value = Double.MinValue;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Float, param.SqlDbType, "#B1");
            Assert.AreEqual(DbType.Double, param.DbType, "#B2");

            value = 0d;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Float, param.SqlDbType, "#C1");
            Assert.AreEqual(DbType.Double, param.DbType, "#C2");
        }

        [Test]
        public void InferType_Enum()
        {
            NpgsqlParameter param;

            param = new NpgsqlParameter();
            param.Value = ByteEnum.A;
            Assert.AreEqual(SqlDbType.TinyInt, param.SqlDbType, "#A1");
            Assert.AreEqual(DbType.Byte, param.DbType, "#A2");

            param = new NpgsqlParameter();
            param.Value = Int64Enum.A;
            Assert.AreEqual(SqlDbType.BigInt, param.SqlDbType, "#B1");
            Assert.AreEqual(DbType.Int64, param.DbType, "#B2");
        }

        [Test]
        public void InferType_Guid()
        {
            Guid value = Guid.NewGuid();

            NpgsqlParameter param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.UniqueIdentifier, param.SqlDbType, "#1");
            Assert.AreEqual(DbType.Guid, param.DbType, "#2");
        }

        [Test]
        public void InferType_Int16()
        {
            Int16 value;
            NpgsqlParameter param;

            value = Int16.MaxValue;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.SmallInt, param.SqlDbType, "#A1");
            Assert.AreEqual(DbType.Int16, param.DbType, "#A2");

            value = Int16.MinValue;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.SmallInt, param.SqlDbType, "#B1");
            Assert.AreEqual(DbType.Int16, param.DbType, "#B2");

            value = (Int16)0;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.SmallInt, param.SqlDbType, "#C1");
            Assert.AreEqual(DbType.Int16, param.DbType, "#C2");
        }

        [Test]
        public void InferType_Int32()
        {
            Int32 value;
            NpgsqlParameter param;

            value = Int32.MaxValue;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Int, param.SqlDbType, "#A1");
            Assert.AreEqual(DbType.Int32, param.DbType, "#A2");

            value = Int32.MinValue;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Int, param.SqlDbType, "#B1");
            Assert.AreEqual(DbType.Int32, param.DbType, "#B2");

            value = 0;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Int, param.SqlDbType, "#C1");
            Assert.AreEqual(DbType.Int32, param.DbType, "#C2");
        }

        [Test]
        public void InferType_Int64()
        {
            Int64 value;
            NpgsqlParameter param;

            value = Int64.MaxValue;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.BigInt, param.SqlDbType, "#A1");
            Assert.AreEqual(DbType.Int64, param.DbType, "#A2");

            value = Int64.MinValue;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.BigInt, param.SqlDbType, "#B1");
            Assert.AreEqual(DbType.Int64, param.DbType, "#B2");

            value = 0L;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.BigInt, param.SqlDbType, "#C1");
            Assert.AreEqual(DbType.Int64, param.DbType, "#C2");
        }

        [Test]
#if NET_2_0
		[Category ("NotWorking")]
#endif
        public void InferType_Invalid()
        {
            object[] notsupported = new object[] {
				UInt16.MaxValue,
				UInt32.MaxValue,
				UInt64.MaxValue,
				SByte.MaxValue,
				new NpgsqlParameter ()
			};

            NpgsqlParameter param = new NpgsqlParameter();

            for (int i = 0; i < notsupported.Length; i++)
            {
#if NET_2_0
				param.Value = notsupported [i];
				try {
					SqlDbType type = param.SqlDbType;
					Assert.Fail ("#A1:" + i + " (" + type + ")");
				} catch (ArgumentException ex) {
					// The parameter data type of ... is invalid
					Assert.AreEqual (typeof (ArgumentException), ex.GetType (), "#A2");
					Assert.IsNull (ex.InnerException, "#A3");
					Assert.IsNotNull (ex.Message, "#A4");
					Assert.IsNull (ex.ParamName, "#A5");
				}

				try {
					DbType type = param.DbType;
					Assert.Fail ("#B1:" + i + " (" + type + ")");
				} catch (ArgumentException ex) {
					// The parameter data type of ... is invalid
					Assert.AreEqual (typeof (ArgumentException), ex.GetType (), "#B2");
					Assert.IsNull (ex.InnerException, "#B3");
					Assert.IsNotNull (ex.Message, "#B4");
					Assert.IsNull (ex.ParamName, "#B5");
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

        [Test]
        public void InferType_Object()
        {
            Object value = new Object();

            NpgsqlParameter param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Variant, param.SqlDbType, "#1");
            Assert.AreEqual(DbType.Object, param.DbType, "#2");
        }

        [Test]
        public void InferType_Single()
        {
            Single value = Single.MaxValue;

            NpgsqlParameter param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Real, param.SqlDbType, "#1");
            Assert.AreEqual(DbType.Single, param.DbType, "#2");
        }

        [Test]
        public void InferType_String()
        {
            String value = "some text";

            NpgsqlParameter param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.NVarChar, param.SqlDbType, "#1");
            Assert.AreEqual(DbType.String, param.DbType, "#2");
        }

        [Test]
#if NET_2_0
		[Category ("NotWorking")]
#endif
        public void InferType_TimeSpan()
        {
            TimeSpan value = new TimeSpan(4, 6, 23);

            NpgsqlParameter param = new NpgsqlParameter();
#if NET_2_0
			param.Value = value;
			Assert.AreEqual (SqlDbType.Time, param.SqlDbType, "#1");
			Assert.AreEqual (DbType.Time, param.DbType, "#2");
#else
            try
            {
                param.Value = value;
                Assert.Fail("#1");
            }
            catch (FormatException)
            {
                // appears to be bug in .NET 1.1 while constructing
                // exception message
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(typeof(ArgumentException), ex.GetType(), "#2");
                Assert.IsNull(ex.InnerException, "#3");
                Assert.IsNotNull(ex.Message, "#4");
                Assert.IsNull(ex.ParamName, "#5");
            }
#endif
        }

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

        [Test] // bug #320196
        public void ParameterNullTest()
        {
            NpgsqlParameter param = new NpgsqlParameter("param", SqlDbType.Decimal);
            Assert.AreEqual(0, param.Scale, "#A1");
            param.Value = DBNull.Value;
            Assert.AreEqual(0, param.Scale, "#A2");

            param = new NpgsqlParameter("param", SqlDbType.Int);
            Assert.AreEqual(0, param.Scale, "#B1");
            param.Value = DBNull.Value;
            Assert.AreEqual(0, param.Scale, "#B2");
        }

        [Test]
        public void ParameterType()
        {
            NpgsqlParameter p;

            // If Type is not set, then type is inferred from the value
            // assigned. The Type should be inferred everytime Value is assigned
            // If value is null or DBNull, then the current Type should be reset to NVarChar.
            p = new NpgsqlParameter();
            Assert.AreEqual(DbType.String, p.DbType, "#A1");
            Assert.AreEqual(SqlDbType.NVarChar, p.SqlDbType, "#A2");
            p.Value = DBNull.Value;
            Assert.AreEqual(DbType.String, p.DbType, "#B1");
            Assert.AreEqual(SqlDbType.NVarChar, p.SqlDbType, "#B2");
            p.Value = 1;
            Assert.AreEqual(DbType.Int32, p.DbType, "#C1");
            Assert.AreEqual(SqlDbType.Int, p.SqlDbType, "#C2");
            p.Value = DBNull.Value;
#if NET_2_0
			Assert.AreEqual (DbType.String, p.DbType, "#D1");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#D2");
#else
            Assert.AreEqual(DbType.Int32, p.DbType, "#D1");
            Assert.AreEqual(SqlDbType.Int, p.SqlDbType, "#D2");
#endif
            p.Value = new byte[] { 0x0a };
            Assert.AreEqual(DbType.Binary, p.DbType, "#E1");
            Assert.AreEqual(SqlDbType.VarBinary, p.SqlDbType, "#E2");
            p.Value = null;
#if NET_2_0
			Assert.AreEqual (DbType.String, p.DbType, "#F1");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#F2");
#else
            Assert.AreEqual(DbType.Binary, p.DbType, "#F1");
            Assert.AreEqual(SqlDbType.VarBinary, p.SqlDbType, "#F2");
#endif
            p.Value = DateTime.Now;
            Assert.AreEqual(DbType.DateTime, p.DbType, "#G1");
            Assert.AreEqual(SqlDbType.DateTime, p.SqlDbType, "#G2");
            p.Value = null;
#if NET_2_0
			Assert.AreEqual (DbType.String, p.DbType, "#H1");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#H2");
#else
            Assert.AreEqual(DbType.DateTime, p.DbType, "#H1");
            Assert.AreEqual(SqlDbType.DateTime, p.SqlDbType, "#H2");
#endif

            // If DbType is set, then the SqlDbType should not be
            // inferred from the value assigned.
            p = new NpgsqlParameter();
            p.DbType = DbType.DateTime;
            Assert.AreEqual(SqlDbType.DateTime, p.SqlDbType, "#I1");
            p.Value = 1;
            Assert.AreEqual(SqlDbType.DateTime, p.SqlDbType, "#I2");
            p.Value = null;
            Assert.AreEqual(SqlDbType.DateTime, p.SqlDbType, "#I3");
            p.Value = DBNull.Value;
            Assert.AreEqual(SqlDbType.DateTime, p.SqlDbType, "#I4");

            // If SqlDbType is set, then the DbType should not be
            // inferred from the value assigned.
            p = new NpgsqlParameter();
            p.SqlDbType = SqlDbType.VarBinary;
            Assert.AreEqual(SqlDbType.VarBinary, p.SqlDbType, "#J1");
            p.Value = 1;
            Assert.AreEqual(SqlDbType.VarBinary, p.SqlDbType, "#J2");
            p.Value = null;
            Assert.AreEqual(SqlDbType.VarBinary, p.SqlDbType, "#J3");
            p.Value = DBNull.Value;
            Assert.AreEqual(SqlDbType.VarBinary, p.SqlDbType, "#J4");
        }

        [Test]
        public void InferType_Boolean()
        {
            Boolean value;
            NpgsqlParameter param;

            value = false;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Bit, param.SqlDbType, "#A1");
            Assert.AreEqual(DbType.Boolean, param.DbType, "#A2");

            value = true;
            param = new NpgsqlParameter();
            param.Value = value;
            Assert.AreEqual(SqlDbType.Bit, param.SqlDbType, "#B1");
            Assert.AreEqual(DbType.Boolean, param.DbType, "#B2");
        }

        [Test]
        public void ParameterName()
        {
            NpgsqlParameter p = new NpgsqlParameter();
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
		public void ResetDbType ()
		{
			NpgsqlParameter p;

			//Parameter with an assigned value but no DbType specified
			p = new NpgsqlParameter ("foo", 42);
			p.ResetDbType ();
			Assert.AreEqual (DbType.Int32, p.DbType, "#A:DbType");
			Assert.AreEqual (SqlDbType.Int, p.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (42, p.Value, "#A:Value");

			p.DbType = DbType.DateTime; //assigning a DbType
			Assert.AreEqual (DbType.DateTime, p.DbType, "#B:DbType1");
			Assert.AreEqual (SqlDbType.DateTime, p.SqlDbType, "#B:SqlDbType1");
			p.ResetDbType ();
			Assert.AreEqual (DbType.Int32, p.DbType, "#B:DbType2");
			Assert.AreEqual (SqlDbType.Int, p.SqlDbType, "#B:SqlDbtype2");

			//Parameter with an assigned SqlDbType but no specified value
			p = new NpgsqlParameter ("foo", SqlDbType.Int);
			p.ResetDbType ();
			Assert.AreEqual (DbType.String, p.DbType, "#C:DbType");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#C:SqlDbType");

			p.DbType = DbType.DateTime; //assigning a SqlDbType
			Assert.AreEqual (DbType.DateTime, p.DbType, "#D:DbType1");
			Assert.AreEqual (SqlDbType.DateTime, p.SqlDbType, "#D:SqlDbType1");
			p.ResetDbType ();
			Assert.AreEqual (DbType.String, p.DbType, "#D:DbType2");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#D:SqlDbType2");

			p = new NpgsqlParameter ();
			p.Value = DateTime.MaxValue;
			Assert.AreEqual (DbType.DateTime, p.DbType, "#E:DbType1");
			Assert.AreEqual (SqlDbType.DateTime, p.SqlDbType, "#E:SqlDbType1");
			p.Value = null;
			p.ResetDbType ();
			Assert.AreEqual (DbType.String, p.DbType, "#E:DbType2");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#E:SqlDbType2");

			p = new NpgsqlParameter ("foo", SqlDbType.VarChar);
			p.Value = DateTime.MaxValue;
			p.ResetDbType ();
			Assert.AreEqual (DbType.DateTime, p.DbType, "#F:DbType");
			Assert.AreEqual (SqlDbType.DateTime, p.SqlDbType, "#F:SqlDbType");
			Assert.AreEqual (DateTime.MaxValue, p.Value, "#F:Value");

			p = new NpgsqlParameter ("foo", SqlDbType.VarChar);
			p.Value = DBNull.Value;
			p.ResetDbType ();
			Assert.AreEqual (DbType.String, p.DbType, "#G:DbType");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#G:SqlDbType");
			Assert.AreEqual (DBNull.Value, p.Value, "#G:Value");

			p = new NpgsqlParameter ("foo", SqlDbType.VarChar);
			p.Value = null;
			p.ResetDbType ();
			Assert.AreEqual (DbType.String, p.DbType, "#G:DbType");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#G:SqlDbType");
			Assert.IsNull (p.Value, "#G:Value");
		}

		[Test]
		public void ResetSqlDbType ()
		{
			//Parameter with an assigned value but no SqlDbType specified
			NpgsqlParameter p1 = new NpgsqlParameter ("foo", 42);
			Assert.AreEqual (42, p1.Value, "#1");
			Assert.AreEqual (DbType.Int32, p1.DbType, "#2");
			Assert.AreEqual (SqlDbType.Int, p1.SqlDbType, "#3");

			p1.ResetSqlDbType ();
			Assert.AreEqual (DbType.Int32, p1.DbType, "#4 The parameter with value 42 must have DbType as Int32");
			Assert.AreEqual (SqlDbType.Int, p1.SqlDbType, "#5 The parameter with value 42 must have SqlDbType as Int");

			p1.SqlDbType = SqlDbType.DateTime; //assigning a SqlDbType
			Assert.AreEqual (DbType.DateTime, p1.DbType, "#6");
			Assert.AreEqual (SqlDbType.DateTime, p1.SqlDbType, "#7");
			p1.ResetSqlDbType (); //Resetting SqlDbType
			Assert.AreEqual (DbType.Int32, p1.DbType, "#8 Resetting SqlDbType must infer the type from the value");
			Assert.AreEqual (SqlDbType.Int, p1.SqlDbType, "#9 Resetting SqlDbType must infer the type from the value");

			//Parameter with an assigned SqlDbType but no specified value
			NpgsqlParameter p2 = new NpgsqlParameter ("foo", SqlDbType.Int);
			Assert.AreEqual (null, p2.Value, "#10");
			Assert.AreEqual (DbType.Int32, p2.DbType, "#11");
			Assert.AreEqual (SqlDbType.Int, p2.SqlDbType, "#12");

			//Although a SqlDbType is specified, calling ResetSqlDbType resets 
			//the SqlDbType and DbType properties to default values
			p2.ResetSqlDbType ();
			Assert.AreEqual (DbType.String, p2.DbType, "#13 Resetting SqlDbType must infer the type from the value");
			Assert.AreEqual (SqlDbType.NVarChar, p2.SqlDbType, "#14 Resetting SqlDbType must infer the type from the value");

			p2.SqlDbType = SqlDbType.DateTime; //assigning a SqlDbType
			Assert.AreEqual (DbType.DateTime, p2.DbType, "#15");
			Assert.AreEqual (SqlDbType.DateTime, p2.SqlDbType, "#16");
			p2.ResetSqlDbType (); //Resetting SqlDbType
			Assert.AreEqual (DbType.String, p2.DbType, "#17 Resetting SqlDbType must infer the type from the value");
			Assert.AreEqual (SqlDbType.NVarChar, p2.SqlDbType, "#18 Resetting SqlDbType must infer the type from the value");
		}
#endif

        [Test]
        public void SourceColumn()
        {
            NpgsqlParameter p = new NpgsqlParameter();
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
		public void SourceColumnNullMapping ()
		{
			NpgsqlParameter p = new NpgsqlParameter ();
			Assert.IsFalse (p.SourceColumnNullMapping, "#1");
			p.SourceColumnNullMapping = true;
			Assert.IsTrue (p.SourceColumnNullMapping, "#2");
			p.SourceColumnNullMapping = false;
			Assert.IsFalse (p.SourceColumnNullMapping, "#3");
		}
#endif

        [Test]
        public void SqlDbTypeTest()
        {
            NpgsqlParameter p = new NpgsqlParameter("zipcode", 3510);
            p.SqlDbType = SqlDbType.DateTime;
            Assert.AreEqual(DbType.DateTime, p.DbType, "#A:DbType");
            Assert.AreEqual(SqlDbType.DateTime, p.SqlDbType, "#A:SqlDbType");
            Assert.AreEqual(3510, p.Value, "#A:Value");
            p.SqlDbType = SqlDbType.VarChar;
            Assert.AreEqual(DbType.AnsiString, p.DbType, "#B:DbType");
            Assert.AreEqual(SqlDbType.VarChar, p.SqlDbType, "#B:SqlDbType");
            Assert.AreEqual(3510, p.Value, "#B:Value");
        }

        [Test]
        public void SqlDbTypeTest_Value_Invalid()
        {
            NpgsqlParameter p = new NpgsqlParameter("zipcode", 3510);
            try
            {
                p.SqlDbType = (SqlDbType)666;
                Assert.Fail("#1");
            }
            catch (ArgumentOutOfRangeException ex)
            {
#if NET_2_0
				// The SqlDbType enumeration value, 666, is
				// invalid
				Assert.AreEqual (typeof (ArgumentOutOfRangeException), ex.GetType (), "#2");
				Assert.IsNull (ex.InnerException, "#3");
				Assert.IsNotNull (ex.Message, "#4");
				Assert.IsTrue (ex.Message.IndexOf ("666") != -1, "#5:" + ex.Message);
				Assert.AreEqual ("SqlDbType", ex.ParamName, "#6");
#else
                // Specified argument was out of the range of
                // valid values
                Assert.AreEqual(typeof(ArgumentOutOfRangeException), ex.GetType(), "#2");
                Assert.IsNull(ex.InnerException, "#3");
                Assert.IsNotNull(ex.Message, "#4");
                Assert.IsNotNull(ex.ParamName, "#5");
#endif
            }
        }

#if NET_2_0
		[Test]
		public void SqlValue ()
		{
			NpgsqlParameter parameter = new NpgsqlParameter ();
			Assert.IsNull (parameter.SqlValue, "#A1");

			object value;

			value = "Char";
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.NVarChar, parameter.SqlDbType, "String:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "String:SqlValue1");
			Assert.AreEqual (typeof (SqlString), parameter.SqlValue.GetType (), "String:SqlValue2");
			Assert.AreEqual (value, ((SqlString) parameter.SqlValue).Value, "String:SqlValue3");
			Assert.AreEqual (value, parameter.Value, "String:Value");

			value = true;
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.Bit, parameter.SqlDbType, "Boolean:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Boolean:SqlValue1");
			Assert.AreEqual (typeof (SqlBoolean), parameter.SqlValue.GetType (), "Boolean:SqlValue2");
			Assert.AreEqual (value, ((SqlBoolean) parameter.SqlValue).Value, "Boolean:SqlValue3");
			Assert.AreEqual (value, parameter.Value, "Boolean:Value");

			value = (byte) 0x0a;
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.TinyInt, parameter.SqlDbType, "Boolean:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Boolean:SqlValue1");
			Assert.AreEqual (typeof (SqlByte), parameter.SqlValue.GetType (), "Boolean:SqlValue2");
			Assert.AreEqual (value, ((SqlByte) parameter.SqlValue).Value, "Boolean:SqlValue3");
			Assert.AreEqual (value, parameter.Value, "Boolean:Value");

			value = new DateTime (2008, 6, 4);
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.DateTime, parameter.SqlDbType, "DateTime:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "DateTime:SqlValue1");
			Assert.AreEqual (typeof (SqlDateTime), parameter.SqlValue.GetType (), "DateTime:SqlValue2");
			Assert.AreEqual (value, ((SqlDateTime) parameter.SqlValue).Value, "DateTime:SqlValue3");
			Assert.AreEqual (value, parameter.Value, "DateTime:Value");

			value = Guid.NewGuid ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.UniqueIdentifier, parameter.SqlDbType, "Guid:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Guid:SqlValue1");
			Assert.AreEqual (typeof (SqlGuid), parameter.SqlValue.GetType (), "Guid:SqlValue2");
			Assert.AreEqual (value, ((SqlGuid) parameter.SqlValue).Value, "Guid:SqlValue3");
			Assert.AreEqual (value, parameter.Value, "Guid:Value");

			value = (short) 5;
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.SmallInt, parameter.SqlDbType, "Int16:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Int16:SqlValue1");
			Assert.AreEqual (typeof (SqlInt16), parameter.SqlValue.GetType (), "Int16:SqlValue2");
			Assert.AreEqual (value, ((SqlInt16) parameter.SqlValue).Value, "Int16:SqlValue3");
			Assert.AreEqual (value, parameter.Value, "Int16:Value");

			value = 10;
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.Int, parameter.SqlDbType, "Int32:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Int32:SqlValue1");
			Assert.AreEqual (typeof (SqlInt32), parameter.SqlValue.GetType (), "Int32:SqlValue2");
			Assert.AreEqual (value, ((SqlInt32) parameter.SqlValue).Value, "Int32:SqlValue3");
			Assert.AreEqual (value, parameter.Value, "Int32:Value");

			value = 56L;
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.BigInt, parameter.SqlDbType, "Int64:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Int64:SqlValue1");
			Assert.AreEqual (typeof (SqlInt64), parameter.SqlValue.GetType (), "Int64:SqlValue2");
			Assert.AreEqual (value, ((SqlInt64) parameter.SqlValue).Value, "Int64:SqlValue3");
			Assert.AreEqual (value, parameter.Value, "Int64:Value");

			parameter.SqlValue = 45.5D;
			Assert.AreEqual (SqlDbType.Float, parameter.SqlDbType, "Double:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Double:SqlValue1");
			Assert.AreEqual (typeof (SqlDouble), parameter.SqlValue.GetType (), "Double:SqlValue2");
			Assert.AreEqual (45.5D, ((SqlDouble) parameter.SqlValue).Value, "Double:SqlValue3");
			Assert.AreEqual (45.5D, parameter.Value, "Double:Value");

			value = 45m;
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.Decimal, parameter.SqlDbType, "Decimal:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Decimal:SqlValue1");
			Assert.AreEqual (typeof (SqlDecimal), parameter.SqlValue.GetType (), "Decimal:SqlValue2");
			Assert.AreEqual (value, ((SqlDecimal) parameter.SqlValue).Value, "Decimal:SqlValue3");
			Assert.AreEqual (value, parameter.Value, "Decimal:Value");

			value = 45f;
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.Real, parameter.SqlDbType, "Decimal:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Decimal:SqlValue1");
			Assert.AreEqual (typeof (SqlSingle), parameter.SqlValue.GetType (), "Decimal:SqlValue2");
			Assert.AreEqual (value, ((SqlSingle) parameter.SqlValue).Value, "Decimal:SqlValue3");
			Assert.AreEqual (value, parameter.Value, "Decimal:Value");

			value = new byte [] { 0x0d, 0x0a };
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.VarBinary, parameter.SqlDbType, "Bytes:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Bytes:SqlValue1");
			Assert.AreEqual (typeof (SqlBinary), parameter.SqlValue.GetType (), "Bytes:SqlValue2");
			Assert.AreEqual (value, ((SqlBinary) parameter.SqlValue).Value, "Bytes:SqlValue3");
			Assert.AreEqual (value, parameter.Value, "Bytes:Value");

			parameter = new NpgsqlParameter ();
			value = 'X';
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.NVarChar, parameter.SqlDbType, "Chars:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Chars:SqlValue1");
			Assert.AreEqual (typeof (SqlString), parameter.SqlValue.GetType (), "Chars:SqlValue2");
			Assert.AreEqual ("X", ((SqlString) parameter.SqlValue).Value, "Chars:SqlValue3");
			// FIXME bug #525321
			//Assert.AreEqual ("X", parameter.Value, "Chars:Value");

			value = new char [] { 'X', 'A' };
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.NVarChar, parameter.SqlDbType, "Chars:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "Chars:SqlValue1");
			Assert.AreEqual (typeof (SqlString), parameter.SqlValue.GetType (), "Chars:SqlValue2");
			Assert.AreEqual ("XA", ((SqlString) parameter.SqlValue).Value, "Chars:SqlValue3");
			// FIXME bug #525321
			//Assert.AreEqual ("XA", parameter.Value, "Chars:Value");
		}
#endif

        [Test]
        public void SqlTypes_SqlBinary()
        {
            NpgsqlParameter parameter;
            SqlBinary value = new SqlBinary(new byte[] { 0x0d, 0x0a });

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.VarBinary, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlBinary.Null;
			Assert.AreEqual (SqlDbType.VarBinary, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlBinary.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlBinary.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.VarBinary, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

        [Test]
        public void SqlTypes_SqlBoolean()
        {
            NpgsqlParameter parameter;
            SqlBoolean value = new SqlBoolean(false);

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.Bit, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlBoolean.Null;
			Assert.AreEqual (SqlDbType.Bit, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlBoolean.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlBoolean.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.Bit, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

        [Test]
        public void SqlTypes_SqlByte()
        {
            NpgsqlParameter parameter;
            SqlByte value = new SqlByte(0x0d);

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.TinyInt, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlByte.Null;
			Assert.AreEqual (SqlDbType.TinyInt, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlByte.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlByte.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.TinyInt, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

#if NET_2_0
		[Test]
		[Category ("NotWorking")]
	        // This doesn't work as SqlBytes are represented as SqlBinary
		public void SqlTypes_SqlBytes ()
		{
			NpgsqlParameter parameter;
			SqlBytes value = new SqlBytes (new byte [] { 0x0d, 0x0a });

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.VarBinary, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreSame (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreSame (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlBytes.Null;
			Assert.AreEqual (SqlDbType.VarBinary, parameter.SqlDbType, "#B:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "#B:SqlValue1");
			Assert.AreEqual (typeof (SqlBytes), parameter.SqlValue.GetType (), "#B:SqlValue2");
			Assert.IsTrue (((SqlBytes) parameter.SqlValue).IsNull, "#B:SqlValue3");
			Assert.IsNotNull (parameter.Value, "#B:Value1");
			Assert.AreEqual (typeof (SqlBytes), parameter.Value.GetType (), "#B:Value2");
			Assert.IsTrue (((SqlBytes) parameter.Value).IsNull, "#B:Value3");

			parameter = new NpgsqlParameter ();
			parameter.Value = value;
			Assert.AreEqual (SqlDbType.VarBinary, parameter.SqlDbType, "#C:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#C:Value");
		}

		[Test]
		[Category ("NotWorking")]
	        // This doesn't work as SqlChars are represented as SqlString
		public void SqlTypes_SqlChars ()
		{
			NpgsqlParameter parameter;
			SqlChars value = new SqlChars (new char [] { 'X', 'A' });

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.NVarChar, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreSame (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreSame (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlChars.Null;
			Assert.AreEqual (SqlDbType.NVarChar, parameter.SqlDbType, "#B:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "#B:SqlValue1");
			Assert.AreEqual (typeof (SqlChars), parameter.SqlValue.GetType (), "#B:SqlValue2");
			Assert.IsTrue (((SqlChars) parameter.SqlValue).IsNull, "#B:SqlValue3");
			Assert.IsNotNull (parameter.Value, "#B:Value1");
			Assert.AreEqual (typeof (SqlChars), parameter.Value.GetType (), "#B:Value2");
			Assert.IsTrue (((SqlChars) parameter.Value).IsNull, "#B:Value3");

			parameter = new NpgsqlParameter ();
			parameter.Value = value;
			Assert.AreEqual (SqlDbType.NVarChar, parameter.SqlDbType, "#C:SqlDbType");
			Assert.AreSame (value, parameter.SqlValue, "#C:SqlValue");
			Assert.AreSame (value, parameter.Value, "#C:Value");
		}
#endif

        [Test]
        public void SqlTypes_SqlDateTime()
        {
            NpgsqlParameter parameter;
            SqlDateTime value = new SqlDateTime(DateTime.Now);

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.DateTime, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlDateTime.Null;
			Assert.AreEqual (SqlDbType.DateTime, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlDateTime.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlDateTime.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.DateTime, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

        [Test]
        public void SqlTypes_SqlDecimal()
        {
            NpgsqlParameter parameter;
            SqlDecimal value = new SqlDecimal(45m);

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.Decimal, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlDecimal.Null;
			Assert.AreEqual (SqlDbType.Decimal, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlDecimal.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlDecimal.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.Decimal, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

        [Test]
        public void SqlTypes_SqlDouble()
        {
            NpgsqlParameter parameter;
            SqlDouble value = new SqlDouble(4.5D);

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.Float, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlDouble.Null;
			Assert.AreEqual (SqlDbType.Float, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlDouble.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlDouble.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.Float, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

        [Test]
        public void SqlTypes_SqlGuid()
        {
            NpgsqlParameter parameter;
            SqlGuid value = new SqlGuid(Guid.NewGuid());

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.UniqueIdentifier, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlGuid.Null;
			Assert.AreEqual (SqlDbType.UniqueIdentifier, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlGuid.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlGuid.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.UniqueIdentifier, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

        [Test]
        public void SqlTypes_SqlInt16()
        {
            NpgsqlParameter parameter;
            SqlInt16 value = new SqlInt16((short)5);

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.SmallInt, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlInt16.Null;
			Assert.AreEqual (SqlDbType.SmallInt, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlInt16.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlInt16.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.SmallInt, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

        [Test]
        public void SqlTypes_SqlInt32()
        {
            NpgsqlParameter parameter;
            SqlInt32 value = new SqlInt32(5);

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.Int, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlInt32.Null;
			Assert.AreEqual (SqlDbType.Int, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlInt32.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlInt32.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.Int, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

        [Test]
        public void SqlTypes_SqlInt64()
        {
            NpgsqlParameter parameter;
            SqlInt64 value = new SqlInt64(5L);

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.BigInt, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlInt64.Null;
			Assert.AreEqual (SqlDbType.BigInt, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlInt64.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlInt64.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.BigInt, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

        [Test]
        public void SqlTypes_SqlMoney()
        {
            NpgsqlParameter parameter;
            SqlMoney value = new SqlMoney(45m);

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.Money, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlMoney.Null;
			Assert.AreEqual (SqlDbType.Money, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlMoney.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlMoney.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.Money, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

        [Test]
        public void SqlTypes_SqlSingle()
        {
            NpgsqlParameter parameter;
            SqlSingle value = new SqlSingle(45f);

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.Real, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlSingle.Null;
			Assert.AreEqual (SqlDbType.Real, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlSingle.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlSingle.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.Real, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

        [Test]
        public void SqlTypes_SqlString()
        {
            NpgsqlParameter parameter;
            SqlString value = new SqlString("XA");

#if NET_2_0
			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.NVarChar, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreEqual (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreEqual (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlString.Null;
			Assert.AreEqual (SqlDbType.NVarChar, parameter.SqlDbType, "#B:SqlDbType");
			Assert.AreEqual (SqlString.Null, parameter.SqlValue, "#B:SqlValue");
			Assert.AreEqual (SqlString.Null, parameter.Value, "#B:Value");
#endif

            parameter = new NpgsqlParameter();
            parameter.Value = value;
            Assert.AreEqual(SqlDbType.NVarChar, parameter.SqlDbType, "#C:SqlDbType");
#if NET_2_0
			Assert.AreEqual (value, parameter.SqlValue, "#C:SqlValue");
#endif
            Assert.AreEqual(value, parameter.Value, "#C:Value");
        }

#if NET_2_0
		[Test]
		public void SqlTypes_SqlXml ()
		{
			NpgsqlParameter parameter;
			SqlXml value = new SqlXml (new XmlTextReader (new StringReader ("<test>Mono</test>")));

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = value;
			Assert.AreEqual (SqlDbType.Xml, parameter.SqlDbType, "#A:SqlDbType");
			Assert.AreSame (value, parameter.SqlValue, "#A:SqlValue");
			Assert.AreSame (value, parameter.Value, "#A:Value");

			parameter = new NpgsqlParameter ();
			parameter.SqlValue = SqlXml.Null;
			Assert.AreEqual (SqlDbType.Xml, parameter.SqlDbType, "#B:SqlDbType");
			Assert.IsNotNull (parameter.SqlValue, "#B:SqlValue1");
			Assert.AreEqual (typeof (SqlXml), parameter.SqlValue.GetType (), "#B:SqlValue2");
			Assert.IsTrue (((SqlXml) parameter.SqlValue).IsNull, "#B:SqlValue3");
			Assert.IsNotNull (parameter.Value, "#B:Value1");
			Assert.AreEqual (typeof (SqlXml), parameter.Value.GetType (), "#B:Value2");
			Assert.IsTrue (((SqlXml) parameter.Value).IsNull, "#B:Value3");

			parameter = new NpgsqlParameter ();
			parameter.Value = value;
			Assert.AreEqual (SqlDbType.Xml, parameter.SqlDbType, "#C:SqlDbType");
			Assert.AreSame (value, parameter.SqlValue, "#C:SqlValue");
			Assert.AreSame (value, parameter.Value, "#C:Value");
		}
#endif

        [Test]
        public void Value()
        {
            NpgsqlParameter p;

            p = new NpgsqlParameter("name", (Object)null);
            p.Value = 42;
            Assert.AreEqual(DbType.Int32, p.DbType, "#A:DbType");
            Assert.AreEqual(SqlDbType.Int, p.SqlDbType, "#A:SqlDbType");
            Assert.AreEqual(42, p.Value, "#A:Value");

            p.Value = DBNull.Value;
#if NET_2_0
			Assert.AreEqual (DbType.String, p.DbType, "#B:DbType");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#B:SqlDbType");
#else
            Assert.AreEqual(DbType.Int32, p.DbType, "#B:DbType");
            Assert.AreEqual(SqlDbType.Int, p.SqlDbType, "#B:SqlDbType");
#endif
            Assert.AreEqual(DBNull.Value, p.Value, "#B:Value");

            p.Value = DateTime.MaxValue;
            Assert.AreEqual(DbType.DateTime, p.DbType, "#C:DbType");
            Assert.AreEqual(SqlDbType.DateTime, p.SqlDbType, "#C:SqlDbType");
            Assert.AreEqual(DateTime.MaxValue, p.Value, "#C:Value");

            p.Value = null;
#if NET_2_0
			Assert.AreEqual (DbType.String, p.DbType, "#D:DbType");
			Assert.AreEqual (SqlDbType.NVarChar, p.SqlDbType, "#D:SqlDbType");
#else
            Assert.AreEqual(DbType.DateTime, p.DbType, "#D:DbType");
            Assert.AreEqual(SqlDbType.DateTime, p.SqlDbType, "#D:SqlDbType");
#endif
            Assert.IsNull(p.Value, "#D:Value");

            p = new NpgsqlParameter("zipcode", SqlDbType.Int);
            p.Value = DateTime.MaxValue;
            Assert.AreEqual(DbType.Int32, p.DbType, "#E:DbType");
            Assert.AreEqual(SqlDbType.Int, p.SqlDbType, "#E:SqlDbType");
            Assert.AreEqual(DateTime.MaxValue, p.Value, "#E:Value");

            p.Value = null;
            Assert.AreEqual(DbType.Int32, p.DbType, "#F:DbType");
            Assert.AreEqual(SqlDbType.Int, p.SqlDbType, "#F:SqlDbType");
            Assert.IsNull(p.Value, "#F:Value");

            p.Value = DBNull.Value;
            Assert.AreEqual(DbType.Int32, p.DbType, "#G:DbType");
            Assert.AreEqual(SqlDbType.Int, p.SqlDbType, "#G:SqlDbType");
            Assert.AreEqual(DBNull.Value, p.Value, "#G:Value");
        }

#if NET_2_0
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

        private enum ByteEnum : byte
        {
            A = 0x0a,
            B = 0x0d
        }

        private enum Int64Enum : long
        {
            A = long.MinValue,
            B = long.MaxValue
        }
    }
}
