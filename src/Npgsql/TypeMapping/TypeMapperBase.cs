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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Npgsql.Logging;
using Npgsql.NameTranslation;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandlers;
using NpgsqlTypes;

namespace Npgsql.TypeMapping
{
    abstract class TypeMapperBase : INpgsqlTypeMapper
    {
        internal Dictionary<string, NpgsqlTypeMapping> Mappings { get; set; }

        internal static readonly INpgsqlNameTranslator DefaultNameTranslator = new NpgsqlSnakeCaseNameTranslator();

        #region Mapping management

        public virtual INpgsqlTypeMapper AddMapping(NpgsqlTypeMapping mapping)
        {
            if (Mappings.ContainsKey(mapping.PgTypeName))
                RemoveMapping(mapping.PgTypeName);
            Mappings[mapping.PgTypeName] = mapping;
            return this;
        }

        public virtual bool RemoveMapping(string pgTypeName) => Mappings.Remove(pgTypeName);
        public abstract void Reset();

        #endregion Mapping management

        #region Enum mapping

        public INpgsqlTypeMapper MapEnum<TEnum>(string pgName = null, INpgsqlNameTranslator nameTranslator = null)
            where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
                throw new ArgumentException("An enum type must be provided");
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName<TEnum>(nameTranslator);

            return AddMapping(new NpgsqlTypeMappingBuilder
            {
                PgTypeName = pgName,
                ClrTypes = new[] { typeof(TEnum) },
                TypeHandlerFactory = new EnumTypeHandlerFactory<TEnum>(nameTranslator)
            }.Build());
        }

        public bool UnmapEnum<TEnum>(string pgName = null, INpgsqlNameTranslator nameTranslator = null) where TEnum : struct
        {
            if (!typeof(TEnum).GetTypeInfo().IsEnum)
                throw new ArgumentException("An enum type must be provided");
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName<TEnum>(nameTranslator);

            return RemoveMapping(pgName);
        }

        #endregion Enum mapping

        #region Composite mapping

        public INpgsqlTypeMapper MapComposite<T>(string pgName = null, INpgsqlNameTranslator nameTranslator = null)
            where T : new()
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName<T>(nameTranslator);

            return AddMapping(new NpgsqlTypeMappingBuilder
            {
                PgTypeName = pgName,
                ClrTypes = new[] { typeof(T) },
                TypeHandlerFactory = new CompositeTypeHandlerFactory<T>(nameTranslator)
            }.Build());
        }

        public bool UnmapComposite<T>(string pgName = null, INpgsqlNameTranslator nameTranslator = null)
            where T: new()
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName<T>(nameTranslator);

            return RemoveMapping(pgName);
        }

        #endregion Composite mapping

        #region Misc

        static string GetPgName<T>(INpgsqlNameTranslator nameTranslator)
        {
            var attr = typeof(T).GetTypeInfo().GetCustomAttribute<PgNameAttribute>();
            return attr == null
                ? nameTranslator.TranslateTypeName(typeof(T).Name)
                : attr.PgName;
        }

        #endregion Misc
    }
}
