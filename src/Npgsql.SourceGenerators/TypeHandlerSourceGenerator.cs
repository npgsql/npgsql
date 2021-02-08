using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Npgsql.SourceGenerators
{
    [Generator]
    class TypeHandlerSourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(() => new MySyntaxReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            var compilation = context.Compilation;

            var (simpleTypeHandlerInterfaceSymbol, typeHandlerInterfaceSymbol) = (
                compilation.GetTypeByMetadataName("Npgsql.Internal.TypeHandling.INpgsqlSimpleTypeHandler`1"),
                compilation.GetTypeByMetadataName("Npgsql.Internal.TypeHandling.INpgsqlTypeHandler`1"));

            if (simpleTypeHandlerInterfaceSymbol is null || typeHandlerInterfaceSymbol is null)
                throw new Exception("Could not find INpgsqlSimpleTypeHandler or INpgsqlTypeHandler");

            foreach (var cds in ((MySyntaxReceiver)context.SyntaxReceiver!).TypeHandlerCandidates)
            {
                var semanticModel = compilation.GetSemanticModel(cds.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(cds) is not INamedTypeSymbol typeSymbol)
                    continue;

                if (typeSymbol.AllInterfaces.Any(i =>
                    i.OriginalDefinition.Equals(simpleTypeHandlerInterfaceSymbol, SymbolEqualityComparer.Default)))
                {
                    AugmentTypeHandler(typeSymbol, cds, isSimple: true);
                    continue;
                }

                if (typeSymbol.AllInterfaces.Any(i =>
                    i.OriginalDefinition.Equals(typeHandlerInterfaceSymbol, SymbolEqualityComparer.Default)))
                {
                    AugmentTypeHandler(typeSymbol, cds, isSimple: false);
                }
            }

            void AugmentTypeHandler(INamedTypeSymbol typeSymbol, ClassDeclarationSyntax classDeclarationSyntax, bool isSimple)
            {
                var typeName = FormatTypeName(typeSymbol);

                var usings = new HashSet<string>(
                    new[]
                    {
                        "System",
                        "System.Threading",
                        "System.Threading.Tasks",
                        "Npgsql.Internal"
                    }.Concat(classDeclarationSyntax.SyntaxTree.GetCompilationUnitRoot().Usings
                        .Where(u => u.Alias is null)
                        .Select(u => u.Name.ToString())));

                var ns = typeSymbol.ContainingNamespace.ToDisplayString();

                var interfaces = typeSymbol.AllInterfaces
                    .Where(i => i.OriginalDefinition.Equals(isSimple ? simpleTypeHandlerInterfaceSymbol : typeHandlerInterfaceSymbol,
                                    SymbolEqualityComparer.Default) &&
                                !i.TypeArguments[0].IsAbstract);

                var validateAccess = isSimple ? "protected" : "public";
                var validationDispatchLines = interfaces.Select(i =>
                    $"{FormatTypeName(i.TypeArguments[0])} converted => (({FormatTypeName(i)})this).ValidateAndGetLength(converted, {(isSimple ? "" : "ref lengthCache, ")}parameter),");

                var writeDispatchLines = interfaces.Select(i =>
                    $"{FormatTypeName(i.TypeArguments[0])} converted => WriteWithLengthInternal(converted, buf, lengthCache, parameter, async, cancellationToken),");

                var sourceBuilder = new StringBuilder();

                foreach (var usingNamespace in usings.OrderBy(n => n))
                    sourceBuilder.Append("using ").Append(usingNamespace).AppendLine(";");

                sourceBuilder.Append($@"

#nullable enable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace {ns}
{{
    partial class {typeName}
    {{
        {validateAccess} override int ValidateObjectAndGetLength(object value, {(isSimple ? "" : "ref NpgsqlLengthCache? lengthCache, ")}NpgsqlParameter? parameter)
            => value switch
            {{");
                foreach (var line in validationDispatchLines)
                    sourceBuilder.Append(@$"
                {line}");

                sourceBuilder.Append($@"

                DBNull => -1,
                null => -1,
                _ => throw new InvalidCastException($""Can't write CLR type {{value.GetType()}} with handler type {typeName}"")
            }};

        public override Task WriteObjectWithLength(object value, NpgsqlWriteBuffer buf, NpgsqlLengthCache? lengthCache, NpgsqlParameter? parameter, bool async, CancellationToken cancellationToken = default)
            => value switch
            {{");
                foreach (var line in writeDispatchLines)
                    sourceBuilder.Append(@$"
                {line}");

                sourceBuilder.Append($@"

                DBNull => WriteWithLengthInternal(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                null => WriteWithLengthInternal(DBNull.Value, buf, lengthCache, parameter, async, cancellationToken),
                _ => throw new InvalidCastException($""Can't write CLR type {{value.GetType()}} with handler type {typeName}"")
            }};
    }}
}}");

                context.AddSource(typeSymbol.Name + ".Generated.cs", SourceText.From(sourceBuilder.ToString(), Encoding.UTF8));
            }

            static string FormatTypeName(ITypeSymbol typeSymbol)
            {
                if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
                {
                    return namedTypeSymbol.IsGenericType
                        ? new StringBuilder(namedTypeSymbol.Name)
                            .Append('<')
                            .Append(string.Join(",", namedTypeSymbol.TypeArguments.Select(FormatTypeName)))
                            .Append('>')
                            .ToString()
                        : namedTypeSymbol.Name;
                }

                return typeSymbol.ToString();
            }
        }

        class MySyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> TypeHandlerCandidates { get; } = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax cds &&
                    cds.BaseList is not null &&
                    cds.Modifiers.Any(SyntaxKind.PartialKeyword))
                {
                    TypeHandlerCandidates.Add(cds);
                }
            }
        }
    }
}
