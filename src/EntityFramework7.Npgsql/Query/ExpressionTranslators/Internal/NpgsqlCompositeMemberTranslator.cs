// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Query.ExpressionTranslators.Internal
{
    public class NpgsqlCompositeMemberTranslator : RelationalCompositeMemberTranslator
    {
        public NpgsqlCompositeMemberTranslator()
        {
            var npgsqlTranslators = new List<IMemberTranslator>
            {
                new NpgsqlStringLengthTranslator(),
                new NpgsqlDateTimeNowTranslator()
            };

            AddTranslators(npgsqlTranslators);
        }
    }
}
