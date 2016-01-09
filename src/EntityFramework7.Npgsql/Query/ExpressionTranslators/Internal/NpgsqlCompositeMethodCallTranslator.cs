// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Microsoft.Data.Entity.Query.ExpressionTranslators.Internal
{
    public class NpgsqlCompositeMethodCallTranslator : RelationalCompositeMethodCallTranslator
    {
        private static readonly IMethodCallTranslator[] _methodCallTranslators =
        {
            new NpgsqlStringSubstringTranslator(),
            new NpgsqlMathAbsTranslator(),
            new NpgsqlMathCeilingTranslator(),
            new NpgsqlMathFloorTranslator(),
            new NpgsqlMathPowerTranslator(),
            new NpgsqlMathRoundTranslator(),
            new NpgsqlMathTruncateTranslator(),
            new NpgsqlStringReplaceTranslator(),
            new NpgsqlStringToLowerTranslator(),
            new NpgsqlStringToUpperTranslator(),
            new NpgsqlRegexIsMatchTranslator(),
        };

        public NpgsqlCompositeMethodCallTranslator([NotNull] ILogger<NpgsqlCompositeMethodCallTranslator> logger)
            : base(logger)
        {
            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            AddTranslators(_methodCallTranslators);
        }
    }
}
