// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Entity.Metadata;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Storage.Internal
{
    // TODO: Implementation is very partial at the moment...
    // TODO: Maybe it's worth finding a way to dynamically map types based on the TypeHandlerRegistry?
    // TODO: Complete types - network, geometric...
    // TODO: Provider-specific types?
    // TODO: Arrays?
    // TODO: Enums? Ranges?
    public class NpgsqlTypeMapper : RelationalTypeMapper
    {
        // No tinyint in PostgreSQL
        readonly RelationalTypeMapping _tinyint     = new RelationalTypeMapping("smallint", typeof(byte), DbType.Byte);
        readonly RelationalTypeMapping _smallint    = new RelationalTypeMapping("smallint", typeof(byte), DbType.Int16);
        readonly RelationalTypeMapping _int         = new RelationalTypeMapping("int", typeof(int), DbType.Int32);
        readonly RelationalTypeMapping _bigint      = new RelationalTypeMapping("bigint", typeof(long), DbType.Int64);
        readonly RelationalTypeMapping _real        = new RelationalTypeMapping("real", typeof(float), DbType.Single);
        readonly RelationalTypeMapping _double      = new RelationalTypeMapping("double precision", typeof(double), DbType.Double);
        readonly RelationalTypeMapping _decimal     = new RelationalTypeMapping("numeric", typeof(decimal), DbType.Decimal);

        // TODO: Look at the SqlServerMaxLengthMapping optimization, it may be relevant for us too
        readonly RelationalTypeMapping _text        = new RelationalTypeMapping("text", typeof(string), DbType.String);
        // TODO: The other text types, char
        readonly RelationalTypeMapping _bytea       = new RelationalTypeMapping("bytea", typeof(byte[]), DbType.Binary);

        readonly RelationalTypeMapping _timestamp   = new RelationalTypeMapping("timestamp", typeof(DateTime), DbType.DateTime);
        readonly RelationalTypeMapping _timestamptz = new RelationalTypeMapping("timestamptz", typeof(DateTime), DbType.DateTimeOffset);
        readonly RelationalTypeMapping _date        = new RelationalTypeMapping("date", typeof(DateTime), DbType.Date);
        readonly RelationalTypeMapping _time        = new RelationalTypeMapping("time", typeof(TimeSpan), DbType.Time);
        // TODO: DbType?
        readonly RelationalTypeMapping _timetz      = new RelationalTypeMapping("timetz", typeof(DateTimeOffset));
        readonly RelationalTypeMapping _interval    = new RelationalTypeMapping("interval", typeof(TimeSpan));

        readonly RelationalTypeMapping _uuid        = new RelationalTypeMapping("uuid", typeof(Guid), DbType.Guid);
        // TODO: BIT(1) vs. BIT(N)
        readonly RelationalTypeMapping _bit         = new RelationalTypeMapping("bit", typeof(BitArray));
        readonly RelationalTypeMapping _bool        = new RelationalTypeMapping("bool", typeof(bool), DbType.Boolean);

        private readonly Dictionary<string, RelationalTypeMapping> _simpleNameMappings;
        private readonly Dictionary<Type, RelationalTypeMapping> _simpleMappings;

        public NpgsqlTypeMapper()
        {
            _simpleNameMappings
                = new Dictionary<string, RelationalTypeMapping>(StringComparer.OrdinalIgnoreCase)
                {
                    { "smallint",         _smallint    },
                    { "integer",          _int         },
                    { "bigint",           _bigint      },
                    { "real",             _real        },
                    { "double precision", _double      },
                    { "decimal",          _decimal     },
                    { "numeric",          _decimal     },
                    { "text",             _text        },
                    { "bytea",            _bytea       },
                    { "timestamp",        _timestamp   },
                    { "timestamptz",      _timestamptz },
                    { "date",             _date        },
                    { "time",             _time        },
                    { "timetz",           _timetz      },
                    { "interval",         _interval    },
                    { "uuid",             _uuid        },
                    { "bit",              _bit         },
                    { "bool",             _bool        },
                };

            _simpleMappings
                = new Dictionary<Type, RelationalTypeMapping>
                {
                    { typeof(byte),           _tinyint     },
                    { typeof(short),          _smallint    },
                    { typeof(int),            _int         },
                    { typeof(long),           _bigint      },
                    { typeof(float),          _real        },
                    { typeof(double),         _double      },
                    { typeof(decimal),        _decimal     },
                    { typeof(string),         _text        },
                    { typeof(byte[]),         _bytea       },
                    { typeof(DateTime),       _timestamp   },
                    { typeof(DateTimeOffset), _timestamptz },
                    { typeof(TimeSpan),       _time        },
                    { typeof(Guid),           _uuid        },
                    { typeof(BitArray),       _bit         },
                    { typeof(bool),           _bool        },

                    //{ typeof(char), _int },
                    //{ typeof(sbyte), new RelationalTypeMapping("smallint") },
                    //{ typeof(ushort), new RelationalTypeMapping("int") },
                    //{ typeof(uint), new RelationalTypeMapping("bigint") },
                    //{ typeof(ulong), new RelationalTypeMapping("numeric(20, 0)") },
                };
        }

        protected override string GetColumnType(IProperty property) => property.Npgsql().ColumnType;

        protected override IReadOnlyDictionary<Type, RelationalTypeMapping> SimpleMappings
            => _simpleMappings;

        protected override IReadOnlyDictionary<string, RelationalTypeMapping> SimpleNameMappings
            => _simpleNameMappings;
    }
}
