using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Npgsql
{
    internal class NpgsqlValue
    {
        PrimitiveUnion _primitives;
        object _object;
        string _string;

        #region Setters

        internal void SetToNull()
        {
            _type = TypeCode.DBNull;
        }

        internal void SetTo(bool value)
        {
            _primitives.Boolean = value;
            _type = TypeCode.Boolean;
        }

        internal void SetTo(byte value)
        {
            _primitives.Byte = value;
            _type = TypeCode.Byte;
        }

        internal void SetTo(short value)
        {
            _primitives.Int16 = value;
            _type = TypeCode.Int16;
        }

        internal void SetTo(int value)
        {
            _primitives.Int32 = value;
            _type = TypeCode.Int32;
        }

        internal void SetTo(long value)
        {
            _primitives.Int64 = value;
            _type = TypeCode.Int64;
        }

        internal void SetTo(string value)
        {
            _string = value;
            _type = TypeCode.String;
        }

        #endregion

        #region Getters

        internal string String
        {
            get
            {
                switch (_type)
                {
                    case TypeCode.String:
                        return _string;
                    case TypeCode.DBNull:
                        return null;
                    case TypeCode.Object:
                        return _object.ToString();
                    case TypeCode.Boolean:
                        return _primitives.Boolean.ToString();
                    case TypeCode.Byte:
                        return _primitives.Byte.ToString();
                    case TypeCode.Int16:
                        return _primitives.Int16.ToString();
                    case TypeCode.Int32:
                        return _primitives.Int32.ToString();
                    case TypeCode.Int64:
                        return _primitives.Int64.ToString();
                    case TypeCode.Single:
                        return _primitives.Single.ToString();
                    case TypeCode.Double:
                        return _primitives.Double.ToString();
                        /*
                    case TypeCode.DateTime:
                        return throw new NotImplementedException();
                    case TypeCode.Decimal:
                        return throw new NotImplementedException();
                    case TypeCode.Char:
                        return throw new NotImplementedException();
                     */
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal byte Byte
        {
            get
            {
                switch (_type)
                {
                    case TypeCode.Byte:
                        return _primitives.Byte;
                    case TypeCode.Int16:
                        return (byte)_primitives.Int16;
                    case TypeCode.Int32:
                        return (byte)_primitives.Int32;
                    case TypeCode.Int64:
                        return (byte)_primitives.Int64;
                    case TypeCode.Single:
                        return (byte)_primitives.Single;
                    case TypeCode.Double:
                        return (byte)_primitives.Double;
                    /*
                case TypeCode.Decimal:
                    return throw new NotImplementedException();
                 */
                    case TypeCode.String:
                        return Byte.Parse(_string);
                    case TypeCode.DBNull:
                        throw new NotImplementedException("Which exception?");
                    default:
                        throw new InvalidCastException(String.Format("Can't cast database type {0} to Byte", _type));
                }
            }
        }

        internal short Int16
        {
            get
            {
                switch (_type)
                {
                    case TypeCode.Byte:
                        return _primitives.Byte;
                    case TypeCode.Int16:
                        return _primitives.Int16;
                    case TypeCode.Int32:
                        return (short)_primitives.Int32;
                    case TypeCode.Int64:
                        return (short)_primitives.Int64;
                    case TypeCode.Single:
                        return (short)_primitives.Single;
                    case TypeCode.Double:
                        return (short)_primitives.Double;
                    /*
                case TypeCode.Decimal:
                    return throw new NotImplementedException();
                 */
                    case TypeCode.String:
                        return Int16.Parse(_string);
                    case TypeCode.DBNull:
                        throw new NotImplementedException("Which exception?");
                    default:
                        throw new InvalidCastException(String.Format("Can't cast database type {0} to Int16", _type));
                }
            }
        }

        internal int Int32
        {
            get
            {
                switch (_type)
                {
                    case TypeCode.Int32:
                        return _primitives.Int32;
                    case TypeCode.Byte:
                        return _primitives.Byte;
                    case TypeCode.Int16:
                        return _primitives.Int16;
                    case TypeCode.Int64:
                        return (int) _primitives.Int64;
                    case TypeCode.Single:
                        return (int) _primitives.Single;
                    case TypeCode.Double:
                        return (int) _primitives.Double;
                        /*
                    case TypeCode.Decimal:
                        return throw new NotImplementedException();
                     */
                    case TypeCode.String:
                        return Int32.Parse(_string);
                    case TypeCode.DBNull:
                        throw new NotImplementedException("Which exception?");
                    default:
                        throw new InvalidCastException(String.Format("Can't cast database type {0} to Int32", _type));
                }
            }
        }

        internal long Int64
        {
            get
            {
                switch (_type)
                {
                    case TypeCode.Byte:
                        return _primitives.Byte;
                    case TypeCode.Int16:
                        return _primitives.Int16;
                    case TypeCode.Int32:
                        return _primitives.Int32;
                    case TypeCode.Int64:
                        return _primitives.Int64;
                    case TypeCode.Single:
                        return (long)_primitives.Single;
                    case TypeCode.Double:
                        return (long)_primitives.Double;
                    /*
                case TypeCode.Decimal:
                    return throw new NotImplementedException();
                 */
                    case TypeCode.String:
                        return Int64.Parse(_string);
                    case TypeCode.DBNull:
                        throw new NotImplementedException("Which exception?");
                    default:
                        throw new InvalidCastException(String.Format("Can't cast database type {0} to Int64", _type));
                }
            }
        }

        internal bool Boolean
        {
            get
            {
                switch (_type)
                {
                    case TypeCode.Boolean:
                        return _primitives.Boolean;
                    case TypeCode.DBNull:
                        throw new NotImplementedException("Which exception?");
                    default:
                        throw new InvalidCastException(String.Format("Can't cast database type {0} to bool", _type));
                }
            }
        }

        internal object Object
        {
            get
            {
                switch (_type)
                {
                    case TypeCode.Object:
                        return _object;
                    case TypeCode.DBNull:
                        return null;
                    case TypeCode.String:
                        return _string;
                    case TypeCode.Boolean:
                        return _primitives.Boolean;
                    case TypeCode.Byte:
                        return _primitives.Byte;
                    case TypeCode.Int16:
                        return _primitives.Int16;
                    case TypeCode.Int32:
                        return _primitives.Int32;
                    case TypeCode.Int64:
                        return _primitives.Int64;
                    case TypeCode.Single:
                        return _primitives.Single;
                    case TypeCode.Double:
                        return _primitives.Double;
                    /*
                    case TypeCode.Decimal:
                        break;
                    case TypeCode.DateTime:
                        break;
                    case TypeCode.Char:
                        return _primitives.Boolean;
                        */
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        #endregion

        internal bool IsNull
        {
            get { return _type == TypeCode.DBNull; }
        }

        public override string ToString()
        {
            return Object.ToString();
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
        }

        TypeCode _type;
    }
}
