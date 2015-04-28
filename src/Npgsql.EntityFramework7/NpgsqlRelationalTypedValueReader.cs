using System;
using System.Data.Common;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Relational;

namespace Npgsql.EntityFramework7
{
    public class NpgsqlRelationalTypedValueReader : RelationalTypedValueReader
    {
        public NpgsqlRelationalTypedValueReader([NotNull] DbDataReader dataReader)
            : base(dataReader) {}

        public override T ReadValue<T>(int index)
        {
            Debug.Assert(index >= 0 && index < Count);

            // No byte type in PostgreSQL, the standard is to use smallint (2-byte) instead.
            // Go through some hoops to convert the database-provided short into a byte
            if (typeof(T) == typeof(byte))
            {
                return (T)(object)(byte)base.ReadValue<short>(index);
            }

            // Npgsql by default returns timestamptz as DateTime with Kind=Utc, not DateTimeOffset.
            // (SqlServer has an actual DateTimeOffset type).
            if (typeof(T) == typeof(DateTimeOffset))
            {
                var dateTime = base.ReadValue<DateTime>(index);
                Debug.Assert(dateTime.Kind == DateTimeKind.Utc || dateTime.Kind == DateTimeKind.Local);
                return (T)(object)new DateTimeOffset(dateTime);
            }

            return base.ReadValue<T>(index);
        }
    }
}