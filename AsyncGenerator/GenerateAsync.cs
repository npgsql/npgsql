using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AsyncGenerator
{
    // Map namespaces to classes to methods, for methods that are marked
    using NamespaceToClasses = Dictionary<NamespaceDeclarationSyntax, Dictionary<ClassDeclarationSyntax, HashSet<MethodInfo>>>;

    /// <summary>
    /// </summary>
    /// <remarks>
    /// http://stackoverflow.com/questions/2961753/how-to-hide-files-generated-by-custom-tool-in-visual-studio
    /// </remarks>
    public class GenerateAsync : Microsoft.Build.Utilities.Task
    {
        [Required]
        public ITaskItem[] InputFiles { get; set; }
        [Output]
        public ITaskItem[] OutputFiles { get; set; }

        /// <summary>
        /// Invocations of methods on these types never get rewritten to async
        /// </summary>
        HashSet<ITypeSymbol> _excludedTypes;

        /// <summary>
        /// Using directives required for async, not expected to be in the source (sync) files
        /// </summary>
        static readonly UsingDirectiveSyntax[] ExtraUsingDirectives = {
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Threading")),
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Threading.Tasks")),
        };

        public override bool Execute()
        {
            var files = InputFiles.Select(f => new SourceFile {
                Name = f.ItemSpec,
                SyntaxTree = SyntaxFactory.ParseSyntaxTree(File.ReadAllText(f.ItemSpec))
            }).ToList();

            var mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create(
                "Temp",
                files.Select(f => f.SyntaxTree),
                new[] { mscorlib }
            );
            foreach (var file in files) {
                file.SemanticModel = compilation.GetSemanticModel(file.SyntaxTree);
            }

            var corlibSymbol = (IAssemblySymbol)compilation.GetAssemblyOrModuleSymbol(mscorlib);
            _excludedTypes = new HashSet<ITypeSymbol> {
                corlibSymbol.GetTypeByMetadataName("System.IO.TextWriter"),
                corlibSymbol.GetTypeByMetadataName("System.IO.MemoryStream") 
            };

            // First pass: find methods with the [GenerateAsync] attribute
            foreach (var file in files)
            {
                foreach (var m in file.SyntaxTree.GetRoot()
                    .DescendantNodes()
                    .OfType<MethodDeclarationSyntax>()
                )
                {
                    var attr = m.AttributeLists.SelectMany(al => al.Attributes).FirstOrDefault(a => a.Name.ToString() == "GenerateAsync");
                    if (attr == null) {
                        continue;
                    }

                    var cls = m.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                    var ns = cls.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();

                    Dictionary<ClassDeclarationSyntax, HashSet<MethodInfo>> classes;
                    if (!file.NamespaceToClasses.TryGetValue(ns, out classes))
                        classes = file.NamespaceToClasses[ns] = new Dictionary<ClassDeclarationSyntax, HashSet<MethodInfo>>();

                    HashSet<MethodInfo> methods;
                    if (!classes.TryGetValue(cls, out methods))
                        methods = classes[cls] = new HashSet<MethodInfo>();

                    var transformedName = attr.ArgumentList == null
                        ? m.Identifier.Text + "Async"
                        : attr.ArgumentList.Arguments[0].ToString().Trim(new[] { '"' });

                    methods.Add(new MethodInfo
                    {
                        Method = m,
                        Transformed = transformedName
                    });
                }
            }

            Log.LogMessage(MessageImportance.Normal, "Found {0} methods marked for async rewriting",
                           files.SelectMany(f => f.NamespaceToClasses.Values).SelectMany(ctm => ctm.Values).SelectMany(m => m).Count());

            // Second pass: transform
            foreach (var f in files)
            {
                Log.LogMessage(MessageImportance.Normal, "Writing out {0}", f.TransformedName);
                File.WriteAllText(f.TransformedName, RewriteFile(f).ToString());
            }

            OutputFiles = files.Select(f => new TaskItem(f.TransformedName)).ToArray();

            return true;
        }

        CompilationUnitSyntax RewriteFile(SourceFile file)
        {
            return SyntaxFactory.CompilationUnit()
              .WithUsings(SyntaxFactory.List(
                  file.SyntaxTree.GetRoot().DescendantNodes().OfType<UsingDirectiveSyntax>().Concat(ExtraUsingDirectives)
              ))
              .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(file.NamespaceToClasses.Select(ntc =>
                  SyntaxFactory.NamespaceDeclaration(ntc.Key.Name)
                  .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(ntc.Value.Select(mbc =>
                      SyntaxFactory.ClassDeclaration(mbc.Key.Identifier)
                      .WithModifiers(mbc.Key.Modifiers)
                      .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>(mbc.Value.Select(m => RewriteMethod(file, m))))
                  ).ToArray()))
              )))
              .WithEndOfFileToken(SyntaxFactory.Token(SyntaxKind.EndOfFileToken))
              .NormalizeWhitespace();
        }

        MethodDeclarationSyntax RewriteMethod(SourceFile file, MethodInfo inMethodInfo)
        {
            var inMethod = inMethodInfo.Method;

            Log.LogMessage(MessageImportance.Low, "Rewriting invocations inside method " + inMethod.Identifier.Text);
            var rewriter = new MethodInvocationRewriter(Log, file.SemanticModel, _excludedTypes);
            var outMethod = (MethodDeclarationSyntax)rewriter.Visit(inMethod);

            // Method signature
            outMethod = outMethod
                .WithIdentifier(SyntaxFactory.Identifier(inMethodInfo.Transformed))
                .WithAttributeLists(new SyntaxList<AttributeListSyntax>())
                .WithModifiers(inMethod.Modifiers
                  .Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword))
                  .Remove(SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
                  .Remove(SyntaxFactory.Token(SyntaxKind.NewKeyword))
                );
                //.WithReturnType(SyntaxFactory.ParseTypeName("Task<" + inMethod.ReturnType + ">"));  // TODO: Structure
            var returnType = inMethod.ReturnType.ToString();
            outMethod = outMethod.WithReturnType(SyntaxFactory.ParseTypeName(
                returnType == "void" ? "Task" : String.Format("Task<{0}>", returnType))
            );

            // Remove the override and new attributes. Seems like the clean .Remove above doesn't work...
            for (var i = 0; i < outMethod.Modifiers.Count;)
            {
                var text = outMethod.Modifiers[i].Text;
                if (text == "override" || text == "new") {
                    outMethod = outMethod.WithModifiers(outMethod.Modifiers.RemoveAt(i));
                    continue;
                }
                i++;
            }

            return outMethod;
        }
    }

    internal class MethodInvocationRewriter : CSharpSyntaxRewriter
    {
        readonly SemanticModel _model;
        readonly HashSet<ITypeSymbol> _excludeTypes;
        readonly TaskLoggingHelper Log;

        public MethodInvocationRewriter(TaskLoggingHelper log, SemanticModel model, HashSet<ITypeSymbol> excludeTypes)
        {
            Log = log;
            _model = model;
            _excludeTypes = excludeTypes;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var symbol = _model.GetSymbolInfo(node).Symbol;
            if (symbol == null)
                return node;

            if (!symbol.GetAttributes().Any(a => a.AttributeClass.Name == "GenerateAsync") && (
                  _excludeTypes.Contains(symbol.ContainingType) ||
                  !symbol.ContainingType.GetMembers(symbol.Name + "Async").Any()
               ))
            {
                return node;
            }

            Log.LogMessage(MessageImportance.Low, "Found rewritable invocation: " + symbol);

            var asIdentifierName = node.Expression as IdentifierNameSyntax;
            if (asIdentifierName != null)
            {
                return SyntaxFactory.PrefixUnaryExpression(SyntaxKind.AwaitExpression,
                    node.WithExpression(asIdentifierName.WithIdentifier(
                        SyntaxFactory.Identifier(asIdentifierName.Identifier.Text + "Async")
                    ))
                );
            }

            var memberAccessExp = node.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExp != null)
            {
                return SyntaxFactory.PrefixUnaryExpression(SyntaxKind.AwaitExpression,
                    node.WithExpression(memberAccessExp.WithName(
                        memberAccessExp.Name.WithIdentifier(SyntaxFactory.Identifier(memberAccessExp.Name.Identifier.Text + "Async"))
                    ))
                );
            }

            throw new NotSupportedException(String.Format("It seems there's an expression type ({0}) not yet supported by the AsyncGenerator", node.Expression.GetType()));
        }
    }

    class SourceFile
    {
        public string Name;
        public SyntaxTree SyntaxTree;
        public SemanticModel SemanticModel;
        public NamespaceToClasses NamespaceToClasses = new NamespaceToClasses();
        public string TransformedName
        {
            get { return Name.Replace(".cs", ".Async.cs"); }
        }
    }

    class MethodInfo
    {
        public MethodDeclarationSyntax Method;
        public string Transformed;
    }
}
