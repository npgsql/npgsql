// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Entity.Storage;

namespace EntityFramework7.Npgsql
{
    // TODO: Implementation is very partial at the moment...
    // TODO: Maybe it's worth finding a way to dynamically map types based on the TypeHandlerRegistry?
    // this would entail having access to an open connection here.
    // TODO: Complete types - network, geometric...
    // TODO: Provider-specific types?
    // TODO: Arrays?
    // TODO: Enums? Ranges?
    public class NpgsqlTypeMapper : RelationalTypeMapper
    {
        // No tinyint in PostgreSQL
        readonly RelationalTypeMapping _smallint    = new RelationalTypeMapping("smallint", DbType.Int16);
        readonly RelationalTypeMapping _int         = new RelationalTypeMapping("int", DbType.Int32);
        readonly RelationalTypeMapping _bigint      = new RelationalTypeMapping("bigint", DbType.Int64);
        readonly RelationalTypeMapping _real        = new RelationalTypeMapping("real", DbType.Single);
        readonly RelationalTypeMapping _double      = new RelationalTypeMapping("double precision", DbType.Double);
        readonly RelationalTypeMapping _decimal     = new RelationalTypeMapping("numeric", DbType.Decimal);

        // TODO: Look at the SqlServerMaxLengthMapping optimization, it may be relevant for us too
        readonly RelationalTypeMapping _text        = new RelationalTypeMapping("text", DbType.String);
        // TODO: The other text types, char
        readonly RelationalTypeMapping _bytea       = new RelationalTypeMapping("bytea", DbType.Binary);

        readonly RelationalTypeMapping _timestamp   = new RelationalTypeMapping("timestamp", DbType.DateTime);
        readonly RelationalTypeMapping _timestamptz = new RelationalTypeMapping("timestamptz", DbType.DateTimeOffset);
        readonly RelationalTypeMapping _date        = new RelationalTypeMapping("date", DbType.Date);
        readonly RelationalTypeMapping _time        = new RelationalTypeMapping("time", DbType.Time);
        // TODO: DbType?
        readonly RelationalTypeMapping _timetz      = new RelationalTypeMapping("timetz");
        readonly RelationalTypeMapping _interval    = new RelationalTypeMapping("interval");

        readonly RelationalTypeMapping _uuid        = new RelationalTypeMapping("uuid", DbType.Guid);
        readonly RelationalTypeMapping _bit         = new RelationalTypeMapping("bit");
        readonly RelationalTypeMapping _bool        = new RelationalTypeMapping("bool", DbType.Boolean);

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

        protected override IReadOnlyDictionary<Type, RelationalTypeMapping> SimpleMappings
            => _simpleMappings;

        protected override IReadOnlyDictionary<string, RelationalTypeMapping> SimpleNameMappings
            => _simpleNameMappings;
    }
}
