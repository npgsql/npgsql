using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using NpgsqlTypes;

namespace Npgsql
{
    abstract class CachedValue
    {
        internal bool IsSet { get; set; }
        internal bool IsProviderSpecificValue { get; set; }
    }

    internal class CachedValue<T> : CachedValue
    {
        T _value;

        internal T Value
        {
            get
            {
                Contract.Requires(IsSet);
                return _value;
            }
            set
            {
                IsSet = true;
                _value = value;
            }
        }
    }

    // TODO: CachedValue instances need to be pooled
    internal class RowCache
    {
        readonly List<CachedValue> _values = new List<CachedValue>(10);

        internal CachedValue<T> Get<T>(int column)
        {
            CachedValue<T> result;
            if (column < _values.Count)
            {
                var c = _values[column];
                if (c == null)
                {
                    result = new CachedValue<T>();
                    _values[column] = result;
                    return result;
                }
                result = c as CachedValue<T>;
                if (result == null)
                {
                    // TODO: Return c to pool
                    result = new CachedValue<T>();
                    _values[column] = result;
                }
                return result;
            }

            for (var i = _values.Count; i < column; i++) {
                _values.Add(null);
            }
            result = new CachedValue<T>();
            _values.Add(result);
            return result;
        }

        internal void Clear()
        {
            foreach (var c in _values)
            {
                if (c != null) {
                    c.IsSet = false;
                }
            }
        }
    }
}
