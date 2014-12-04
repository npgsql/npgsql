using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NpgsqlTypes;

namespace Npgsql
{
    internal class NpgsqlValue
    {
        PrimitiveUnion _primitives;
        object _object;
        string _string;
        byte[] _binary;

        #region Setters

        internal void SetToNull()
        {
            Type = StorageType.Null;
        }

        internal void SetTo(bool value)
        {
            _primitives.Boolean = value;
            Type = StorageType.Boolean;
        }

        internal void SetTo(byte value)
        {
            _primitives.Byte = value;
            Type = StorageType.Byte;
        }

        internal void SetTo(short value)
        {
            _primitives.Int16 = value;
            Type = StorageType.Int16;
        }

        internal void SetTo(int value)
        {
            _primitives.Int32 = value;
            Type = StorageType.Int32;
        }

        internal void SetTo(long value)
        {
            _primitives.Int64 = value;
            Type = StorageType.Int64;
        }

        internal void SetTo(float value)
        {
            _primitives.Single = value;
            Type = StorageType.Single;
        }

        internal void SetTo(double value)
        {
            _primitives.Double = value;
            Type = StorageType.Double;
        }

        internal void SetTo(decimal value)
        {
            _primitives.Decimal = value;
            Type = StorageType.Decimal;
        }

        internal void SetTo(string value)
        {
            _string = value;
            Type = StorageType.String;
        }

        internal void SetTo(byte[] value)
        {
            _binary = value;
            Type = StorageType.Binary;
        }

        internal void SetTo(NpgsqlDate value)
        {
            _primitives.Date = value;
            Type = StorageType.Date;
        }

        internal void SetTo(NpgsqlTime value)
        {
            _primitives.Time = value;
            Type = StorageType.Time;
        }

        internal void SetTo(NpgsqlTimeTZ value)
        {
            _primitives.TimeTz = value;
            Type = StorageType.TimeTz;
        }

        internal void SetTo(NpgsqlTimeStamp value)
        {
            _primitives.TimeStamp = value;
            Type = StorageType.TimeStamp;
        }

        internal void SetTo(NpgsqlTimeStampTZ value)
        {
            _primitives.TimeStampTz = value;
            Type = StorageType.TimeStampTz;
        }

        internal void SetTo(NpgsqlInterval value)
        {
            _primitives.Interval = value;
            Type = StorageType.Interval;
        }

        #endregion

        #region Getters

        internal string String
        {
            get
            {
                switch (Type)
                {
                    case StorageType.String:
                        return _string;
                    case StorageType.Object:
                        return _object.ToString();
                    case StorageType.Boolean:
                        return _primitives.Boolean.ToString();
                    case StorageType.Byte:
                        return _primitives.Byte.ToString();
                    case StorageType.Int16:
                        return _primitives.Int16.ToString();
                    case StorageType.Int32:
                        return _primitives.Int32.ToString();
                    case StorageType.Int64:
                        return _primitives.Int64.ToString();
                    case StorageType.Single:
                        return _primitives.Single.ToString();
                    case StorageType.Double:
                        return _primitives.Double.ToString();
                    case StorageType.Decimal:
                        return _primitives.Decimal.ToString();
                    case StorageType.Date:
                        return _primitives.Date.ToString();
                    case StorageType.Time:
                        return _primitives.Time.ToString();
                    case StorageType.TimeTz:
                        return _primitives.TimeTz.ToString();
                    case StorageType.TimeStamp:
                        return _primitives.TimeStamp.ToString();
                    case StorageType.TimeStampTz:
                        return _primitives.TimeStampTz.ToString();
                    case StorageType.Interval:
                        return _primitives.Interval.ToString();
                    /*
            case StorageType.Char:
                return throw new NotImplementedException();
             */
                    default:
                        throw CreateConversionException();
                }
            }
        }

        internal byte Byte
        {
            get
            {
                switch (Type)
                {
                    case StorageType.Byte:
                        return _primitives.Byte;
                    case StorageType.Int16:
                        return (byte)_primitives.Int16;
                    case StorageType.Int32:
                        return (byte)_primitives.Int32;
                    case StorageType.Int64:
                        return (byte)_primitives.Int64;
                    case StorageType.Single:
                        return (byte)_primitives.Single;
                    case StorageType.Double:
                        return (byte)_primitives.Double;
                    case StorageType.Decimal:
                        return (byte)_primitives.Decimal;
                    case StorageType.String:
                        return Byte.Parse(_string);
                    default:
                        throw CreateConversionException();
                }
            }
        }

        internal short Int16
        {
            get
            {
                switch (Type)
                {
                    case StorageType.Byte:
                        return _primitives.Byte;
                    case StorageType.Int16:
                        return _primitives.Int16;
                    case StorageType.Int32:
                        return (short)_primitives.Int32;
                    case StorageType.Int64:
                        return (short)_primitives.Int64;
                    case StorageType.Single:
                        return (short)_primitives.Single;
                    case StorageType.Double:
                        return (short)_primitives.Double;
                    case StorageType.Decimal:
                        return (short)_primitives.Decimal;
                    case StorageType.String:
                        return Int16.Parse(_string);
                    default:
                        throw CreateConversionException();
                }
            }
        }

        internal int Int32
        {
            get
            {
                switch (Type)
                {
                    case StorageType.Int32:
                        return _primitives.Int32;
                    case StorageType.Byte:
                        return _primitives.Byte;
                    case StorageType.Int16:
                        return _primitives.Int16;
                    case StorageType.Int64:
                        return (int) _primitives.Int64;
                    case StorageType.Single:
                        return (int) _primitives.Single;
                    case StorageType.Double:
                        return (int) _primitives.Double;
                    case StorageType.Decimal:
                        return (int)_primitives.Decimal;
                    case StorageType.String:
                        return Int32.Parse(_string);
                    default:
                        throw CreateConversionException();
                }
            }
        }

        internal long Int64
        {
            get
            {
                switch (Type)
                {
                    case StorageType.Byte:
                        return _primitives.Byte;
                    case StorageType.Int16:
                        return _primitives.Int16;
                    case StorageType.Int32:
                        return _primitives.Int32;
                    case StorageType.Int64:
                        return _primitives.Int64;
                    case StorageType.Single:
                        return (long)_primitives.Single;
                    case StorageType.Double:
                        return (long)_primitives.Double;
                    case StorageType.Decimal:
                        return (long)_primitives.Decimal;
                    case StorageType.String:
                        return Int64.Parse(_string);
                    default:
                        throw CreateConversionException();
                }
            }
        }

        internal float Float
        {
            get
            {
                switch (Type)
                {
                    case StorageType.Single:
                        return _primitives.Single;
                    case StorageType.Double:
                        return (float)_primitives.Double;
                    case StorageType.Byte:
                        return _primitives.Byte;
                    case StorageType.Int16:
                        return _primitives.Int16;
                    case StorageType.Int32:
                        return _primitives.Int32;
                    case StorageType.Int64:
                        return _primitives.Int64;
                    case StorageType.Decimal:
                        return (float)_primitives.Decimal;
                    case StorageType.String:
                        return Single.Parse(_string);
                    default:
                        throw CreateConversionException();
                }
            }
        }

        internal double Double
        {
            get
            {
                switch (Type)
                {
                    case StorageType.Double:
                        return _primitives.Double;
                    case StorageType.Single:
                        return _primitives.Single;
                    case StorageType.Byte:
                        return _primitives.Byte;
                    case StorageType.Int16:
                        return _primitives.Int16;
                    case StorageType.Int32:
                        return _primitives.Int32;
                    case StorageType.Int64:
                        return _primitives.Int64;
                    case StorageType.Decimal:
                        return (double)_primitives.Decimal;
                    case StorageType.String:
                        return Double.Parse(_string);
                    default:
                        throw CreateConversionException();
                }
            }
        }

        internal decimal Decimal
        {
            get
            {
                switch (Type)
                {
                    case StorageType.Decimal:
                        return _primitives.Decimal;
                    case StorageType.Byte:
                        return _primitives.Byte;
                    case StorageType.Int16:
                        return _primitives.Int16;
                    case StorageType.Int32:
                        return _primitives.Int32;
                    case StorageType.Int64:
                        return _primitives.Int64;
                    case StorageType.Single:
                        return (decimal)_primitives.Single;
                    case StorageType.Double:
                        return (decimal)_primitives.Double;
                    case StorageType.String:
                        return Decimal.Parse(_string);
                    default:
                        throw CreateConversionException();
                }
            }
        }

        internal bool Boolean
        {
            get
            {
                switch (Type)
                {
                    case StorageType.Boolean:
                        return _primitives.Boolean;
                    default:
                        throw CreateConversionException();
                }
            }
        }

        internal DateTime DateTime
        {
            get
            {
                switch (Type)
                {
                    case StorageType.Date:
                        return (DateTime)_primitives.Date;
                    case StorageType.Time:
                        return (DateTime)_primitives.Time;
                    case StorageType.TimeTz:
                        return (DateTime)_primitives.TimeTz;
                    case StorageType.TimeStamp:
                        return (DateTime)_primitives.TimeStamp;
                    case StorageType.TimeStampTz:
                        return ((DateTime)_primitives.TimeStampTz).ToLocalTime();
                    default:
                        throw CreateConversionException();
                }
            }
        }

        internal NpgsqlDate Date
        {
            get
            {
                if (Type == StorageType.Date)
                    return _primitives.Date;
                throw CreateConversionException();
            }
        }

        internal NpgsqlTime Time
        {
            get
            {
                if (Type == StorageType.Time)
                    return _primitives.Time;
                throw CreateConversionException();
            }
        }

        internal NpgsqlTimeTZ TimeTZ
        {
            get
            {
                if (Type == StorageType.TimeTz)
                    return _primitives.TimeTz;
                throw CreateConversionException();
            }
        }

        internal NpgsqlTimeStamp TimeStamp
        {
            get
            {
                if (Type == StorageType.TimeStamp)
                    return _primitives.TimeStamp;
                throw CreateConversionException();
            }
        }

        internal NpgsqlTimeStampTZ TimeStampTz
        {
            get
            {
                if (Type == StorageType.TimeStampTz)
                    return _primitives.TimeStampTz;
                throw CreateConversionException();
            }
        }

        internal NpgsqlInterval Interval
        {
            get
            {
                if (Type == StorageType.Interval)
                    return _primitives.Interval;
                throw CreateConversionException();
            }
        }

        internal TimeSpan TimeSpan
        {
            get
            {
                if (Type == StorageType.Interval)
                    return (TimeSpan)_primitives.Interval;
                throw CreateConversionException();
            }
        }

        internal object Value
        {
            get
            {
                switch (Type)
                {
                    case StorageType.Object:
                        return _object;
                    case StorageType.Null:
                        return DBNull.Value;
                    case StorageType.String:
                        return _string;
                    case StorageType.Boolean:
                        return _primitives.Boolean;
                    case StorageType.Byte:
                        return _primitives.Byte;
                    case StorageType.Int16:
                        return _primitives.Int16;
                    case StorageType.Int32:
                        return _primitives.Int32;
                    case StorageType.Int64:
                        return _primitives.Int64;
                    case StorageType.Single:
                        return _primitives.Single;
                    case StorageType.Double:
                        return _primitives.Double;
                    case StorageType.Decimal:
                        return _primitives.Decimal;
                    case StorageType.Date:
                    case StorageType.Time:
                    case StorageType.TimeTz:
                    case StorageType.TimeStamp:
                    case StorageType.TimeStampTz:
                        return DateTime;
                    case StorageType.Interval:
                        return (TimeSpan)_primitives.Interval;
                    /*
                    case StorageType.Char:
                        return _primitives.Boolean;
                        */
                    case StorageType.Binary:
                        return _binary;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal object ProviderSpecificValue
        {
            get
            {
                switch (Type)
                {
                    case StorageType.Date:
                        return _primitives.Date;
                    case StorageType.Time:
                        return _primitives.Time;
                    case StorageType.TimeTz:
                        return _primitives.TimeTz;
                    case StorageType.TimeStamp:
                        return _primitives.TimeStamp;
                    case StorageType.TimeStampTz:
                        return _primitives.TimeStampTz;
                    case StorageType.Interval:
                        return _primitives.Interval;
                    default:
                        return Value;
                }
            }
        }

        #endregion

        internal bool IsNull
        {
            get { return Type == StorageType.Null; }
        }

        Exception CreateConversionException()
        {
            // TODO: Is InvalidCastException the best choice for NULL?
            throw Type == StorageType.Null
                ? new InvalidCastException("Column contains NULL")
                : new InvalidCastException(String.Format("Can't cast database type {0} to String", Type));
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct PrimitiveUnion
        {
            [FieldOffset(0)]
            internal bool Boolean;
            [FieldOffset(0)]
            internal byte Byte;
            [FieldOffset(0)]
            internal short Int16;
            [FieldOffset(0)]
            internal int Int32;
            [FieldOffset(0)]
            internal long Int64;
            [FieldOffset(0)]
            internal float Single;
            [FieldOffset(0)]
            internal double Double;
            [FieldOffset(0)]
            internal decimal Decimal;
            [FieldOffset(0)]
            internal NpgsqlDate Date;
            [FieldOffset(0)]
            internal NpgsqlTime Time;
            [FieldOffset(0)]
            internal NpgsqlTimeTZ TimeTz;
            [FieldOffset(0)]
            internal NpgsqlTimeStamp TimeStamp;
            [FieldOffset(0)]
            internal NpgsqlTimeStampTZ TimeStampTz;
            [FieldOffset(0)]
            internal NpgsqlInterval Interval;
        }

        internal StorageType Type { get; private set; }

        internal enum StorageType
        {
            Null,
            Object,
            String,
            Boolean,
            Byte,
            Int16,
            Int32,
            Int64,
            Single,
            Double,
            Decimal,
            Date,
            Time,
            TimeTz,
            TimeStamp,
            TimeStampTz,
            Interval,
            Binary,
        }
    }
}
