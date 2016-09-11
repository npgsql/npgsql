using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Npgsql.PostgresTypes
{
    class PostgresRangeType : PostgresType
    {
        readonly PostgresType _subtype;

        internal PostgresRangeType(string ns, string name, uint oid, PostgresType subtypePostgresType)
            : base(ns, name, oid)
        {
            _subtype = subtypePostgresType;
            if (subtypePostgresType.NpgsqlDbType.HasValue)
                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Range | subtypePostgresType.NpgsqlDbType;
            _subtype.Range = this;
        }

        internal override TypeHandler Activate(TypeHandlerRegistry registry)
        {
            TypeHandler subtypeHandler;
            if (!registry.TryGetByOID(_subtype.OID, out subtypeHandler))
            {
                // Subtype hasn't been set up yet, do it now
                subtypeHandler = _subtype.Activate(registry);
            }

            var handler = subtypeHandler.CreateRangeHandler(this);
            registry.ByOID[OID] = handler;
            if (NpgsqlDbType.HasValue)
                registry.ByNpgsqlDbType.Add(NpgsqlDbType.Value, handler);
            registry.ByType[handler.GetFieldType()] = handler;
            return handler;
        }
    }
}
