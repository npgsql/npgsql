// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using System.Resources;
using Microsoft.Data.Entity.Infrastructure;

[assembly: AssemblyTitle("EntityFramework7.Npgsql")]
[assembly: AssemblyDescription("PostgreSQL provider for Entity Framework 7")]
[assembly: DesignTimeProviderServices(
    fullyQualifiedTypeName: "Microsoft.Data.Entity.Scaffolding.NpgsqlDesignTimeServices, EntityFramework7.Npgsql.Design",
    packageName: "EntityFramework7.Npgsql.Design")]
