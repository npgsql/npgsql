﻿using System;
using BenchmarkDotNet.Attributes;
using Npgsql.Internal.TypeHandlers;

namespace Npgsql.Benchmarks.TypeHandlers
{
    [Config(typeof(Config))]
    public class Uuid : TypeHandlerBenchmarks<Guid>
    {
        public Uuid() : base(new UuidHandler(GetPostgresType("uuid"))) { }
    }
}
