using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Npgsql.Internal.TypeHandling;
using NpgsqlTypes;

namespace Npgsql.TypeMapping
{
    abstract class TypeMapperBase : INpgsqlTypeMapper
    {
        public INpgsqlNameTranslator DefaultNameTranslator { get; }

        protected TypeMapperBase(INpgsqlNameTranslator defaultNameTranslator)
        {
            if (defaultNameTranslator == null)
                throw new ArgumentNullException(nameof(defaultNameTranslator));

            DefaultNameTranslator = defaultNameTranslator;
        }

        #region Mapping management

        /// <inheritdoc />
        public abstract INpgsqlTypeMapper MapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            where TEnum : struct, Enum;

        /// <inheritdoc />
        public abstract bool UnmapEnum<TEnum>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null)
            where TEnum : struct, Enum;

        /// <inheritdoc />
        [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
        public abstract INpgsqlTypeMapper MapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null);

        /// <inheritdoc />
        [RequiresUnreferencedCode("Composite type mapping currently isn't trimming-safe.")]
        public abstract INpgsqlTypeMapper MapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null);

        /// <inheritdoc />
        public abstract bool UnmapComposite<T>(string? pgName = null, INpgsqlNameTranslator? nameTranslator = null);

        /// <inheritdoc />
        public abstract bool UnmapComposite(Type clrType, string? pgName = null, INpgsqlNameTranslator? nameTranslator = null);

        /// <inheritdoc />
        public abstract void AddTypeResolverFactory(TypeHandlerResolverFactory resolverFactory);

        public abstract void Reset();

        #endregion Composite mapping

        #region Misc

        private protected static string GetPgName(Type clrType, INpgsqlNameTranslator nameTranslator)
            => clrType.GetCustomAttribute<PgNameAttribute>()?.PgName
               ?? nameTranslator.TranslateTypeName(clrType.Name);

        #endregion Misc
    }
}
