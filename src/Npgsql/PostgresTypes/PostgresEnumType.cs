#region License
// The PostgreSQL License
//
// Copyright (C) 2017 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using Npgsql.TypeHandlers;

namespace Npgsql.PostgresTypes
{
    /// <summary>
    /// Represents a PostgreSQL enum data type.
    /// </summary>
    /// <remarks>
    /// See https://www.postgresql.org/docs/current/static/datatype-enum.html.
    /// </remarks>
    public class PostgresEnumType : PostgresType
    {
        /// <summary>
        /// Constructs a representation of a PostgreSQL enum data type.
        /// </summary>
        protected internal PostgresEnumType(string ns, string name, uint oid) : base(ns, name, oid)
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
