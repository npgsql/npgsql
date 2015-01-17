using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace NpgsqlTypes
{
    /// <summary>
    /// Handles user types, such as enums and composite types.
    /// </summary>
    public static class NpgsqlUserTypes
    {
        /// <summary>
        /// Registers an enum type by setting up a mapping between a .NET enum type and a PosgreSQL type.
        /// TEnum must be a .NET enum type. The enum labels should have the same values in both the .NET enum and the database.
        /// If another label is used in the database, this can be specified for each label with a EnumLabelAttribute.
        /// This method must be called before connecting the first time to a PostgreSQL database in your application, preferably at application start.
        /// </summary>
        /// <typeparam name="TEnum">A System.Enum type.</typeparam>
        /// <param name="typeName">The name of the enum type in the PostgreSQL database.</param>
        public static void RegisterEnum<TEnum>(string typeName)
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("The enumType is not an enum type", "enumType");
            if (string.IsNullOrEmpty(typeName))
                throw new ArgumentException("typeName cannot be empty", "typeName");
            Contract.EndContractBlock();

            TypeHandlerRegistry.AddEnumType(typeof(TEnum), typeName);
        }
    }

    /// <summary>
    /// Indicates that the PostgreSQL enum value differs from the .NET value.
    /// </summary>
    public class EnumLabelAttribute : Attribute
    {
        public string Label { get; private set; }

        /// <summary>
        /// Indicates that the PostgreSQL enum value differs from the .NET value.
        /// </summary>
        /// <param name="label">What label to use instead.</param>
        public EnumLabelAttribute(string label)
        {
            Label = label;
        }
    }
}
