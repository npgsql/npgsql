using System;
using System.Data;

#if ENTITIES6
using System.Data.Entity.Spatial;
#else
using System.Data.Spatial;
#endif

namespace Npgsql.Spatial
{
    /// <summary>
    /// A postgis geometry service API.
    /// </summary>
    public class PostgisDataReader
        : DbSpatialDataReader, System.Data.IDataReader
    {
        private PostgisServices _svcs;
        private NpgsqlDataReader _rdr;

        /// <summary>
        /// Creates a new instance of postgis data reader using a specific instance of PostgisService.
        /// </summary>
        /// <param name="svcs">The service provider that DbGeometry instances will use.</param>
        /// <param name="rdr">The underlying data reader.</param>
        public PostgisDataReader(PostgisServices svcs, NpgsqlDataReader rdr)
            :base()
        {
            _svcs = svcs;
            _rdr = rdr;
        }

        /// <summary>
        /// Creates a new instance of postgis data reader.
        /// </summary>
        /// <param name="rdr">The underlying data reader.</param>
        public PostgisDataReader(NpgsqlDataReader rdr)
            :this(new PostgisServices(),rdr)
        {

        }

        /// <summary>
        /// Get the DbGeography value of a column, given its zero-based ordinal.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override DbGeography GetGeography(int ordinal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the DbGeometry value of a column, given its zero-based ordinal.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public override DbGeometry GetGeometry(int ordinal)
        {
            return _svcs.GeometryFromProviderValue(_rdr[ordinal]);
        }

        /// <summary>
        /// Get the value indicating wether a column is a Geometry value, given its zero-based ordinal.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
#if ENTITIES6
        public override bool IsGeometryColumn(int ordinal)
#else
        public bool IsGeometryColumn(int ordinal)
#endif
        {
            return _rdr[ordinal] is NpgsqlTypes.PostgisGeometry;
        }

        /// <summary>
        /// Get the value indicating wether a column is a Geography value, given its zero-based ordinal.
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
#if ENTITIES6
        public override bool IsGeographyColumn(int ordinal)
#else
        public bool IsGeographyColumn(int ordinal)
#endif
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a value indicating the depth of nesting of the current row. Always Zero.
        /// </summary>
        public int Depth
        {
            get
            {
                return _rdr.Depth;
            }
        }

        /// <summary>
        /// Gets a value indicating wether the data reader is closed.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return _rdr.IsClosed;
            }
        }

        /// <summary>
        /// Gets the number of row affected by the SQL statement.
        /// </summary>
        public int RecordsAffected
        {
            get
            {
                return _rdr.RecordsAffected;
            }
        }


        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        public int FieldCount
        {
            get
            {
                return _rdr.FieldCount;
            }
        }

        /// <summary>
        /// Gets the value of the specified column name of the current row.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get
            {
                return _rdr[name];
            }
        }

        /// <summary>
        /// Gets the value of the specified column index of the current row.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object this[int i]
        {
            get
            {
                return _rdr[i];
            }
        }

        /// <summary>
        /// Close the underlying datareader object.
        /// </summary>
        public void Close()
        {
            _rdr.Close();
        }

        /// <summary>
        /// Returns a DataTable which contains metadata about the current row.
        /// </summary>
        /// <returns></returns>
        public DataTable GetSchemaTable()
        {
            return _rdr.GetSchemaTable();
        }

        /// <summary>
        /// Advances the reader to the next result when reading data from a batch of statements.
        /// </summary>
        /// <returns></returns>
        public bool NextResult()
        {
            return _rdr.NextResult();
        }

        /// <summary>
        /// Advances the reader to the next record in a result set.
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            return _rdr.Read();
        }

        /// <summary>
        /// Frees the resources hold by the data reader.
        /// </summary>
        public void Dispose()
        {
            _rdr.Dispose();
        }

        /// <summary>
        /// Gets the name of a column, given a zero based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public string GetName(int i)
        {
            return _rdr.GetName(i);
        }

        /// <summary>
        /// Gets the data type name of a column, given a zero based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public string GetDataTypeName(int i)
        {
            return _rdr.GetDataTypeName(i);
        }


        /// <summary>
        /// Gets the System.Type of a column, given a zero based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Type GetFieldType(int i)
        {
            return _rdr.GetFieldType(i);
        }


        /// <summary>
        /// Gets the value of a column, given a zero based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public object GetValue(int i)
        {
            return _rdr.GetValue(i);
        }

        /// <summary>
        /// Populates an array of objects with the values of the current row.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int GetValues(object[] values)
        {
            return _rdr.GetValues(values);
        }

        /// <summary>
        /// Gets the column ordinal given the column name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetOrdinal(string name)
        {
            return _rdr.GetOrdinal(name);
        }


        /// <summary>
        /// Get the value of a column as a boolean, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool GetBoolean(int i)
        {
            return _rdr.GetBoolean(i);
        }

        /// <summary>
        /// Get the value of a column as a byte, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public byte GetByte(int i)
        {
            return _rdr.GetByte(i);
        }

        /// <summary>
        /// Populates a byte array with the value of a column, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return _rdr.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// Get the value of a column as a char, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public char GetChar(int i)
        {
            return _rdr.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return _rdr.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        /// <summary>
        /// Get the value of a column as a GUID, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Guid GetGuid(int i)
        {
            return _rdr.GetGuid(i);
        }

        /// <summary>
        /// Get the value of a column as an int16, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public short GetInt16(int i)
        {
            return _rdr.GetInt16(i);
        }

        /// <summary>
        /// Get the value of a column as an int32, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public int GetInt32(int i)
        {
            return _rdr.GetInt32(i);
        }

        /// <summary>
        /// Get the value of a column as an int64, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public long GetInt64(int i)
        {
            return _rdr.GetInt64(i);
        }

        /// <summary>
        /// Get the value of a column as a float, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public float GetFloat(int i)
        {
            return _rdr.GetFloat(i);
        }

        /// <summary>
        /// Get the value of a column as a double, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double GetDouble(int i)
        {
            return _rdr.GetDouble(i);
        }

        /// <summary>
        /// Get the value of a column as a string, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public string GetString(int i)
        {
            return _rdr.GetString(i);
        }

        /// <summary>
        /// Get the value of a column as a decimal, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public decimal GetDecimal(int i)
        {
            return _rdr.GetDecimal(i);
        }

        /// <summary>
        /// Get the value of a column as a datetime, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public DateTime GetDateTime(int i)
        {
            return _rdr.GetDateTime(i);
        }

        /// <summary>
        /// Returns a DbDataReader of a column, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public IDataReader GetData(int i)
        {
            return _rdr.GetData(i);
        }

        /// <summary>
        /// Get the value indicating wether the column contains non-existent or missing value, given its zero-based ordinal.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool IsDBNull(int i)
        {
            return _rdr.IsDBNull(i);
        }
    }
}
