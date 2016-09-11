using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql.TypeHandlers;

namespace Npgsql.PostgresTypes
{
    class PostgresCompositeType : PostgresType
    {
        /// <summary>
        /// Holds the name and OID for all fields.
        /// Populated on the first activation of the composite.
        /// </summary>
        internal List<RawCompositeField> RawFields { get; }

        internal PostgresCompositeType(string ns, string name, uint oid, List<RawCompositeField> rawFields)
            : base(ns, name, oid)
        {
            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Composite;
            RawFields = rawFields;
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
            // Composites need to be mapped by the user with an explicit mapping call (MapComposite or MapCompositeGlobally).
            // If we're here the enum hasn't been mapped to a CLR type and we should activate it as text.
            throw new Exception($"Composite PostgreSQL type {Name} must be mapped before use");
        }

        internal void Activate(TypeHandlerRegistry registry, ICompositeHandlerFactory factory)
            => Activate(registry, factory.Create(this, RawFields, registry));

        internal void Activate(TypeHandlerRegistry registry, ICompositeHandler compositeHandler)
        {
            var handler = (TypeHandler)compositeHandler;

            registry.ByOID[OID] = handler;
            registry.ByType[compositeHandler.CompositeType] = handler;
            Array?.Activate(registry);
        }
    }
}
