using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.TypeHandlers;

namespace Npgsql.PostgresTypes
{
    class PostgresEnumType : PostgresType
    {
        internal PostgresEnumType(string ns, string name, uint oid) : base(ns, name, oid)
        {
            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Enum;
        }

        internal override void AddTo(TypeHandlerRegistry.AvailablePostgresTypes types)
        {
            base.AddTo(types);
            types.ByFullName[FullName] = this;
            types.ByName[Name] = types.ByName.ContainsKey(Name)
                ? null
                : this;
        }

        internal override TypeHandler Activate(TypeHandlerRegistry registry)
        {
            // Enums need to be mapped by the user with an explicit mapping call (MapComposite or MapCompositeGlobally).
            // If we're here the enum hasn't been mapped to a CLR type and we should activate it as text.
            return new PostgresBaseType(Namespace, Name, OID, typeof(TextHandler), new TypeMappingAttribute(Name))
                .Activate(registry);
        }

        internal void Activate(TypeHandlerRegistry registry, IEnumHandlerFactory handlerFactory)
            => Activate(registry, handlerFactory.Create(this));

        internal void Activate(TypeHandlerRegistry registry, IEnumHandler enumHandler)
        {
            var handler = (TypeHandler)enumHandler;
            registry.ByOID[OID] = handler;
            registry.ByType[enumHandler.EnumType] = handler;

            Array?.Activate(registry);
        }
    }
}
