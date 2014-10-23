using System;
using System.Data ;

namespace NpgsqlTypes
{
    internal class PostgisNativeTypeInfo : NpgsqlNativeTypeInfo
    {
        public readonly string _trueCastName;
        
        internal PostgisNativeTypeInfo(ConvertNativeToBackendTextHandler ConvertNativeToBackendText = null,
                                       ConvertNativeToBackendBinaryHandler ConvertNativeToBackendBinary = null
        ) : base ("geometry",NpgsqlDbType.Geometry ,DbType.Object,false ,ConvertNativeToBackendText, ConvertNativeToBackendBinary)            
        {
            _trueCastName = "bytea::geometry";
            //We need to cast the hexstring to bytea before casting it finally to geometry.
        }

        public override string CastName
        {
            get { return _trueCastName; }
        }
    }
}

