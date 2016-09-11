using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.TypeHandlers;

namespace Npgsql.PostgresTypes
{
    class PostgresArrayType : PostgresType
    {
        readonly PostgresType _element;

        internal PostgresArrayType(string ns, string name, uint oid, PostgresType elementPostgresType)
            : base(ns, name, oid)
        {
            _element = elementPostgresType;
            if (elementPostgresType.NpgsqlDbType.HasValue)
                NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Array | elementPostgresType.NpgsqlDbType;
            _element.Array = this;
        }

        internal override TypeHandler Activate(TypeHandlerRegistry registry)
        {
            TypeHandler elementHandler;
            if (!registry.TryGetByOID(_element.OID, out elementHandler))
            {
                // Element type hasn't been set up yet, do it now
                elementHandler = _element.Activate(registry);
            }

            var arrayHandler = elementHandler.CreateArrayHandler(this);
            registry.ByOID[OID] = arrayHandler;

            var asEnumHandler = elementHandler as IEnumHandler;
            if (asEnumHandler != null)
            {
                if (registry.ArrayHandlerByType == null)
                    registry.ArrayHandlerByType = new Dictionary<Type, TypeHandler>();
                registry.ArrayHandlerByType[asEnumHandler.EnumType] = arrayHandler;
                return arrayHandler;
            }

            var asCompositeHandler = elementHandler as ICompositeHandler;
            if (asCompositeHandler != null)
            {
                if (registry.ArrayHandlerByType == null)
                    registry.ArrayHandlerByType = new Dictionary<Type, TypeHandler>();
                registry.ArrayHandlerByType[asCompositeHandler.CompositeType] = arrayHandler;
                return arrayHandler;
            }

            if (NpgsqlDbType.HasValue)
                registry.ByNpgsqlDbType[NpgsqlDbType.Value] = arrayHandler;

            // Note that array handlers aren't registered in _byType, because they handle all dimension types and not just one CLR type
            // (e.g. int[], int[,], int[,,]). So the by-type lookup is special, see this[Type type]
            // TODO: register single-dimensional in _byType as a specific optimization? But do PSV as well...

            return arrayHandler;
        }
    }
}
