using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban;

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

            var template = Template.Parse(EmbeddedResource.GetContent("TypeHandler.snbtxt"), "TypeHandler.snbtxt");

            foreach (var cds in ((MySyntaxReceiver)context.SyntaxReceiver!).TypeHandlerCandidates)
            {
                var semanticModel = compilation.GetSemanticModel(cds.SyntaxTree);
                if (semanticModel.GetDeclaredSymbol(cds) is not INamedTypeSymbol typeSymbol)
                    continue;

                if (typeSymbol.AllInterfaces.Any(i =>
                    i.OriginalDefinition.Equals(simpleTypeHandlerInterfaceSymbol, SymbolEqualityComparer.Default)))
                {
                    AugmentTypeHandler(template, typeSymbol, cds, isSimple: true);
                    continue;
                }

                if (typeSymbol.AllInterfaces.Any(i =>
                    i.OriginalDefinition.Equals(typeHandlerInterfaceSymbol, SymbolEqualityComparer.Default)))
                {
                    AugmentTypeHandler(template, typeSymbol, cds, isSimple: false);
                }
            }

            void AugmentTypeHandler(
                Template template,
                INamedTypeSymbol typeSymbol,
                ClassDeclarationSyntax classDeclarationSyntax,
                bool isSimple)
            {
                var usings = new HashSet<string>(
                    new[]
                    {
                        "System",
                        "System.Threading",
                        "System.Threading.Tasks",
                        "Npgsql.Internal"
                    }.Concat(classDeclarationSyntax.SyntaxTree.GetCompilationUnitRoot().Usings
                        .Where(u => u.Alias is null && u.StaticKeyword.IsKind(SyntaxKind.None))
                        .Select(u => u.Name.ToString())));

                var interfaces = typeSymbol.AllInterfaces
                    .Where(i => i.OriginalDefinition.Equals(isSimple ? simpleTypeHandlerInterfaceSymbol : typeHandlerInterfaceSymbol,
                                    SymbolEqualityComparer.Default) &&
                                !i.TypeArguments[0].IsAbstract);

                var output = template.Render(new
                {
                    Usings = usings,
                    TypeName = FormatTypeName(typeSymbol),
                    Namespace = typeSymbol.ContainingNamespace.ToDisplayString(),
                    IsSimple = isSimple,
                    Interfaces = interfaces.Select(i => new
                    {
                        Name = FormatTypeName(i),
                        HandledType = FormatTypeName(i.TypeArguments[0]),
                    })
                });

                context.AddSource(typeSymbol.Name + ".Generated.cs", SourceText.From(output, Encoding.UTF8));
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

                if (typeSymbol.TypeKind == TypeKind.Array)
                {
                    return $"{FormatTypeName(((IArrayTypeSymbol)typeSymbol).ElementType)}[]";
                    // return "int";
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
