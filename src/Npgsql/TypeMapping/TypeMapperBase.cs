using System;
using System.Collections.Generic;
using System.Reflection;
using Npgsql.TypeHandlers;
using Npgsql.TypeHandlers.CompositeHandlers;
using Npgsql.TypeHandling;
using NpgsqlTypes;

namespace Npgsql.TypeMapping
{
    abstract class TypeMapperBase : INpgsqlTypeMapper
    {
        internal Dictionary<string, NpgsqlTypeMapping> Mappings { get; } = new Dictionary<string, NpgsqlTypeMapping>();

        public INpgsqlNameTranslator DefaultNameTranslator { get; }

        protected TypeMapperBase(INpgsqlNameTranslator defaultNameTranslator)
        {
            if (defaultNameTranslator == null)
                throw new ArgumentNullException(nameof(defaultNameTranslator));

            DefaultNameTranslator = defaultNameTranslator;
        }

        #region Mapping management

        public virtual INpgsqlTypeMapper AddMapping(NpgsqlTypeMapping mapping)
        {
            if (Mappings.ContainsKey(mapping.PgTypeName))
                RemoveMapping(mapping.PgTypeName);
            Mappings[mapping.PgTypeName] = mapping;
            return this;
        }

        public virtual bool RemoveMapping(string pgTypeName) => Mappings.Remove(pgTypeName);

        IEnumerable<NpgsqlTypeMapping> INpgsqlTypeMapper.Mappings => Mappings.Values;

        public abstract void Reset();

        #endregion Mapping management

        #region Enum mapping

        public INpgsqlTypeMapper MapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            where TEnum : struct, Enum
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName(typeof(TEnum), nameTranslator);

            return AddMapping(new NpgsqlTypeMappingBuilder
            {
                PgTypeName = pgName,
                ClrTypes = new[] { typeof(TEnum) },
                TypeHandlerFactory = new EnumTypeHandlerFactory<TEnum>(nameTranslator)
            }.Build());
        }

        public bool UnmapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            where TEnum : struct, Enum
        {
            if (pgName != null && pgName.Trim() == "")
                throw new ArgumentException("pgName can't be empty", nameof(pgName));

            if (nameTranslator == null)
                nameTranslator = DefaultNameTranslator;
            if (pgName == null)
                pgName = GetPgName(typeof(TEnum), nameTranslator);

            return RemoveMapping(pgName);
        }

        #endregion Enum mapping

        #region Composite mapping

        public INpgsqlTypeMapper MapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            => MapComposite(pgName, nameTranslator, typeof(T), t => new CompositeTypeHandlerFactory<T>(t));

        public INpgsqlTypeMapper MapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            => MapComposite(pgName, nameTranslator, clrType, t => (NpgsqlTypeHandlerFactory)
                Activator.CreateInstance(typeof(CompositeTypeHandlerFactory<>).MakeGenericType(clrType), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new object[] { t }, null)!);

        INpgsqlTypeMapper MapComposite(string? pgName, INpgsqlNameTranslator? nameTranslator, Type type, Func<INpgsqlNameTranslator, NpgsqlTypeHandlerFactory> factory)
        {
            if (pgName != null && string.IsNullOrWhiteSpace(pgName))
                throw new ArgumentException("pgName can't be empty.", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(type, nameTranslator);

            return AddMapping(
                new NpgsqlTypeMappingBuilder
                {
                    PgTypeName = pgName,
                    ClrTypes = new[] { type },
                    TypeHandlerFactory = factory(nameTranslator),
                }
                .Build());
        }

        public bool UnmapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            => UnmapComposite(typeof(T), pgName, nameTranslator);

        public bool UnmapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
        {
            if (pgName != null && string.IsNullOrWhiteSpace(pgName))
                throw new ArgumentException("pgName can't be empty.", nameof(pgName));

            nameTranslator ??= DefaultNameTranslator;
            pgName ??= GetPgName(clrType, nameTranslator);

            return RemoveMapping(pgName);
        }

        #endregion Composite mapping

        #region Misc

        // TODO: why does ReSharper think `GetCustomAttribute<T>` is non-nullable?
        // ReSharper disable once ConstantConditionalAccessQualifier ConstantNullCoalescingCondition
        private protected static string GetPgName(Type clrType, INpgsqlNameTranslator nameTranslator)
            => clrType.GetCustomAttribute<PgNameAttribute>()?.PgName
               ?? nameTranslator.TranslateTypeName(clrType.Name);

        #endregion Misc
    }
}
