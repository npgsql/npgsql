using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using EntityFramework7.Npgsql.FunctionalTests;
using Microsoft.CodeAnalysis;
using Microsoft.Data.Entity.FunctionalTests.TestUtilities.Xunit;
using Microsoft.Data.Entity.Relational.Design.FunctionalTests.ReverseEngineering;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Data.Entity.Internal;
using Microsoft.Data.Entity.Scaffolding;
using Microsoft.Data.Entity.Scaffolding.Internal;

namespace EntityFramework7.Npgsql.Design.FunctionalTests.ReverseEngineering
{
    public class NpgsqlE2ETests : E2ETestBase, IClassFixture<NpgsqlE2EFixture>
    {
        protected override string ProviderName => "EntityFramework7.Npgsql.Design";

        protected override void ConfigureDesignTimeServices(IServiceCollection services)
        {
            base.ConfigureDesignTimeServices(services);
            new NpgsqlDesignTimeServices().ConfigureDesignTimeServices(services);
        }

        public virtual string TestNamespace => "E2ETest.Namespace";
        public virtual string TestProjectDir => Path.Combine("E2ETest", "Output");
        public virtual string TestSubDir => "SubDir";
        public virtual string CustomizedTemplateDir => Path.Combine("E2ETest", "CustomizedTemplate", "Dir");

        public static TableSelectionSet Filter
            => new TableSelectionSet(new List<string>{
                "AllDataTypes",
                "PropertyConfiguration",
                "Test Spaces Keywords Table",
                "OneToManyDependent",
                "OneToManyPrincipal",
                "OneToOneDependent",
                "OneToOnePrincipal",
                "OneToOneSeparateFKDependent",
                "OneToOneSeparateFKPrincipal",
                "OneToOneFKToUniqueKeyDependent",
                "OneToOneFKToUniqueKeyPrincipal",
                /*
                "ReferredToByTableWithUnmappablePrimaryKeyColumn",
                "TableWithUnmappablePrimaryKeyColumn",
                */
                "SelfReferencing",
            });

        public NpgsqlE2ETests(NpgsqlE2EFixture fixture, ITestOutputHelper output)
            : base(output)
        {
        }

        protected override E2ECompiler GetCompiler() => new E2ECompiler
        {
            NamedReferences =
                    {
                        "EntityFramework.Core",
                        "EntityFramework.Relational",
                        "EntityFramework7.Npgsql",
#if DNXCORE50 || NETCORE50
                        "System.Data.Common",
                        "System.Linq.Expressions",
                        "System.Reflection",
                        "System.ComponentModel.Annotations",
#else
                    },
            References =
                    {
                        MetadataReference.CreateFromFile(
                            Assembly.Load(new AssemblyName(
                                "System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")).Location),
                        MetadataReference.CreateFromFile(
                            Assembly.Load(new AssemblyName(
                                "System.ComponentModel.DataAnnotations, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35")).Location),
#endif
                    }
        };

        string _connectionString = NpgsqlTestStore.CreateConnectionString("NpgsqlReverseEngineerTestE2E");

        private static readonly List<string> _expectedEntityTypeFiles = new List<string>
            {
                "AllDataTypes.expected",
                "OneToManyDependent.expected",
                "OneToManyPrincipal.expected",
                "OneToOneDependent.expected",
                "OneToOneFKToUniqueKeyDependent.expected",
                "OneToOneFKToUniqueKeyPrincipal.expected",
                "OneToOnePrincipal.expected",
                "OneToOneSeparateFKDependent.expected",
                "OneToOneSeparateFKPrincipal.expected",
                "PropertyConfiguration.expected",
                /*
                "ReferredToByTableWithUnmappablePrimaryKeyColumn.expected",
                */
                "SelfReferencing.expected",
                "Test_Spaces_Keywords_Table.expected"
            };

        [Fact]
        [UseCulture("en-US")]
        public void E2ETest_UseAttributesInsteadOfFluentApi()
        {
            var configuration = new ReverseEngineeringConfiguration
            {
                ConnectionString = _connectionString,
                ContextClassName = "AttributesContext",
                ProjectPath = TestProjectDir + Path.DirectorySeparatorChar, // tests that ending DirectorySeparatorChar does not affect namespace
                ProjectRootNamespace = TestNamespace,
                OutputPath = TestSubDir,
                TableSelectionSet = Filter,
            };

            var filePaths = Generator.GenerateAsync(configuration).GetAwaiter().GetResult();

            var actualFileSet = new FileSet(InMemoryFiles, Path.Combine(TestProjectDir, TestSubDir))
            {
                Files = Enumerable.Repeat(filePaths.ContextFile, 1).Concat(filePaths.EntityTypeFiles).Select(Path.GetFileName).ToList()
            };

            var expectedFileSet = new FileSet(new FileSystemFileService(),
                Path.Combine("ReverseEngineering", "ExpectedResults", "E2E_UseAttributesInsteadOfFluentApi"),
                contents => contents.Replace("namespace " + TestNamespace, "namespace " + TestNamespace + "." + TestSubDir)
                    .Replace("{{connectionString}}", _connectionString))
            {
                Files = (new List<string> { "AttributesContext.expected" })
                    .Concat(_expectedEntityTypeFiles).ToList()
            };

            /*
            AssertLog(new LoggerMessages
            {
                Warn =
                        {
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.AllDataTypes.geographyColumn", "geography"),
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.AllDataTypes.geometryColumn", "geometry"),
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.AllDataTypes.hierarchyidColumn", "hierarchyid"),
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.AllDataTypes.sql_variantColumn", "sql_variant"),
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.AllDataTypes.xmlColumn", "xml"),
                            NpgsqlDesignStrings.DataTypeDoesNotAllowNpgsqlIdentityStrategy("dbo.PropertyConfiguration.PropertyConfigurationID","tinyint"),
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.TableWithUnmappablePrimaryKeyColumn.TableWithUnmappablePrimaryKeyColumnID", "hierarchyid"),
                            RelationalDesignStrings.PrimaryKeyErrorPropertyNotFound("dbo.TableWithUnmappablePrimaryKeyColumn"),
                            RelationalDesignStrings.UnableToGenerateEntityType("dbo.TableWithUnmappablePrimaryKeyColumn"),
                        }
            });
            */
            AssertEqualFileContents(expectedFileSet, actualFileSet);
            AssertCompile(actualFileSet);
        }

        [Fact]
        [UseCulture("en-US")]
        public void E2ETest_AllFluentApi()
        {
            var configuration = new ReverseEngineeringConfiguration
            {
                ConnectionString = _connectionString,
                ProjectPath = TestProjectDir,
                ProjectRootNamespace = TestNamespace,
                OutputPath = null, // not used for this test
                UseFluentApiOnly = true,
                TableSelectionSet = Filter,
            };

            var filePaths = Generator.GenerateAsync(configuration).GetAwaiter().GetResult();

            var actualFileSet = new FileSet(InMemoryFiles, TestProjectDir)
            {
                Files = Enumerable.Repeat(filePaths.ContextFile, 1).Concat(filePaths.EntityTypeFiles).Select(Path.GetFileName).ToList()
            };

            var expectedFileSet = new FileSet(new FileSystemFileService(),
                Path.Combine("ReverseEngineering", "ExpectedResults", "E2E_AllFluentApi"),
                inputFile => inputFile.Replace("{{connectionString}}", _connectionString))
            {
                Files = (new List<string> { "NpgsqlReverseEngineerTestE2EContext.expected" })
                    .Concat(_expectedEntityTypeFiles).ToList()
            };

            /*
            AssertLog(new LoggerMessages
            {
                Warn =
                        {
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.AllDataTypes.geographyColumn", "geography"),
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.AllDataTypes.geometryColumn", "geometry"),
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.AllDataTypes.hierarchyidColumn", "hierarchyid"),
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.AllDataTypes.sql_variantColumn", "sql_variant"),
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.AllDataTypes.xmlColumn", "xml"),
                            NpgsqlDesignStrings.DataTypeDoesNotAllowNpgsqlIdentityStrategy("dbo.PropertyConfiguration.PropertyConfigurationID","tinyint"),
                            RelationalDesignStrings.CannotFindTypeMappingForColumn("dbo.TableWithUnmappablePrimaryKeyColumn.TableWithUnmappablePrimaryKeyColumnID", "hierarchyid"),
                            RelationalDesignStrings.PrimaryKeyErrorPropertyNotFound("dbo.TableWithUnmappablePrimaryKeyColumn"),
                            RelationalDesignStrings.UnableToGenerateEntityType("dbo.TableWithUnmappablePrimaryKeyColumn"),
                        }
            });
            */

            AssertEqualFileContents(expectedFileSet, actualFileSet);
            AssertCompile(actualFileSet);
        }
    }
}
