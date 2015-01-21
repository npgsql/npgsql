using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace NpgsqlTypes
{
    /// <summary>
    /// Indicates that the PostgreSQL enum value differs from the .NET value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumLabelAttribute : Attribute
    {
        /// <summary>
        /// The database label that corresponds to the .NET enum value on which this attribute is placed.
        /// </summary>
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
