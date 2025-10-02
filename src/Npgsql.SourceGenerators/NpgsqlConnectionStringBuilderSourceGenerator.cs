using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Scriban;

namespace Npgsql.SourceGenerators;

[Generator]
public class NpgsqlConnectionStringBuilderSourceGenerator : IIncrementalGenerator
{
    static readonly DiagnosticDescriptor InternalError = new DiagnosticDescriptor(
        id: "PGXXXX",
        title: "Internal issue when source-generating NpgsqlConnectionStringBuilder",
        messageFormat: "{0}",
        category: "Internal",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var compilationProvider = context.CompilationProvider;
        context.RegisterSourceOutput(compilationProvider, (spc, compilation) =>
        {
            var type = compilation.Assembly.GetTypeByMetadataName("Npgsql.NpgsqlConnectionStringBuilder");
            if (type is null)
                return;

            var connectionStringPropertyAttribute = compilation.Assembly.GetTypeByMetadataName("Npgsql.NpgsqlConnectionStringPropertyAttribute");
            if (connectionStringPropertyAttribute is null)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    InternalError,
                    location: null,
                    "Could not find Npgsql.NpgsqlConnectionStringPropertyAttribute"));
                return;
            }

            var obsoleteAttribute = compilation.GetTypeByMetadataName("System.ObsoleteAttribute");
            var displayNameAttribute = compilation.GetTypeByMetadataName("System.ComponentModel.DisplayNameAttribute");
            var defaultValueAttribute = compilation.GetTypeByMetadataName("System.ComponentModel.DefaultValueAttribute");

            if (obsoleteAttribute is null || displayNameAttribute is null || defaultValueAttribute is null)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    InternalError,
                    location: null,
                    "Could not find ObsoleteAttribute, DisplayNameAttribute or DefaultValueAttribute"));
                return;
            }

            var properties = new List<PropertyDetails>();
            var propertiesByKeyword = new Dictionary<string, PropertyDetails>();
            foreach (var member in type.GetMembers())
            {
                if (member is not IPropertySymbol property ||
                    property.GetAttributes().FirstOrDefault(a => connectionStringPropertyAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default)) is not { } propertyAttribute ||
                    property.GetAttributes()
                        .FirstOrDefault(a => displayNameAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default))
                        ?.ConstructorArguments[0].Value is not string displayName)
                {
                    continue;
                }

                var explicitDefaultValue = property.GetAttributes()
                    .FirstOrDefault(a => defaultValueAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default))
                    ?.ConstructorArguments[0].Value;

                if (explicitDefaultValue is string s)
                    explicitDefaultValue = '"' + s.Replace("\"", "\"\"") + '"';

                if (explicitDefaultValue is not null && property.Type.TypeKind == TypeKind.Enum)
                {
                    explicitDefaultValue = $"({property.Type.Name}){explicitDefaultValue}";
                }

                var propertyDetails = new PropertyDetails
                {
                    Name = property.Name,
                    CanonicalName = displayName,
                    TypeName = property.Type.Name,
                    IsEnum = property.Type.TypeKind == TypeKind.Enum,
                    IsObsolete = property.GetAttributes().Any(a => obsoleteAttribute.Equals(a.AttributeClass, SymbolEqualityComparer.Default)),
                    DefaultValue = explicitDefaultValue
                };

                properties.Add(propertyDetails);

                propertiesByKeyword[displayName.ToUpperInvariant()] = propertyDetails;
                if (property.Name != displayName)
                {
                    var propertyName = property.Name.ToUpperInvariant();
                    if (!propertiesByKeyword.ContainsKey(propertyName))
                        propertyDetails.Alternatives.Add(propertyName);
                }

                if (propertyAttribute.ConstructorArguments.Length == 1)
                {
                    foreach (var synonymArg in propertyAttribute.ConstructorArguments[0].Values)
                    {
                        if (synonymArg.Value is string synonym)
                        {
                            var synonymName = synonym.ToUpperInvariant();
                            if (!propertiesByKeyword.ContainsKey(synonymName))
                                propertyDetails.Alternatives.Add(synonymName);
                        }
                    }
                }
            }

            var template = Template.Parse(EmbeddedResource.GetContent("NpgsqlConnectionStringBuilder.snbtxt"), "NpgsqlConnectionStringBuilder.snbtxt");

            var output = template.Render(new
            {
                Properties = properties,
                PropertiesByKeyword = propertiesByKeyword
            });

            spc.AddSource(type.Name + ".Generated.cs", SourceText.From(output, Encoding.UTF8));
        });
    }

    sealed class PropertyDetails
    {
        public string Name { get; set; } = null!;
        public string CanonicalName { get; set; } = null!;
        public string TypeName { get; set; } = null!;
        public bool IsEnum { get; set; }
        public bool IsObsolete { get; set; }
        public object? DefaultValue { get; set; }

        public HashSet<string> Alternatives { get; } = new(StringComparer.Ordinal);

        public PropertyDetails Clone()
            => new()
            {
                Name = Name,
                CanonicalName = CanonicalName,
                TypeName = TypeName,
                IsEnum = IsEnum,
                IsObsolete = IsObsolete,
                DefaultValue = DefaultValue
            };
    }
}
