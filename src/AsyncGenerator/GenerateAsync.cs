using System;
using System.Collections.Generic;
using System.Data;
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

            // Creation of the assembly reference should be done this way:
            // var mscorlib = new MetadataFileReference(typeof(object).Assembly.Location);
            // But this creates a "path not absolute" exception on mono in GetAssemblyOrModuleSymbol() below.
            // See http://stackoverflow.com/questions/26355922/creating-roslyn-metadatafilereference-bombs-on-mono-linux

            var mscorlibMetadata = AssemblyMetadata.CreateFromImageStream(new FileStream(typeof(object).Assembly.Location, FileMode.Open, FileAccess.Read));
            var mscorlib = new MetadataImageReference (mscorlibMetadata);

            var datalibMetadata = AssemblyMetadata.CreateFromImageStream(new FileStream(typeof(CommandBehavior).Assembly.Location, FileMode.Open, FileAccess.Read));
            var datalib = new MetadataImageReference (datalibMetadata);

            var compilation = CSharpCompilation.Create(
                "Temp",
                files.Select(f => f.SyntaxTree),
                new[] { mscorlib, datalib }
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
                    // Syntactically filter out any method without [GenerateAsync] (for performance)
                    if (m.AttributeLists.SelectMany(al => al.Attributes).All(a => a.Name.ToString() != "GenerateAsync")) {
                        continue;
                    }

                    var methodSymbol = file.SemanticModel.GetDeclaredSymbol(m);

                    var cls = m.FirstAncestorOrSelf<ClassDeclarationSyntax>();
                    var ns = cls.FirstAncestorOrSelf<NamespaceDeclarationSyntax>();

                    Dictionary<ClassDeclarationSyntax, HashSet<MethodInfo>> classes;
                    if (!file.NamespaceToClasses.TryGetValue(ns, out classes))
                        classes = file.NamespaceToClasses[ns] = new Dictionary<ClassDeclarationSyntax, HashSet<MethodInfo>>();

                    HashSet<MethodInfo> methods;
                    if (!classes.TryGetValue(cls, out methods))
                        methods = classes[cls] = new HashSet<MethodInfo>();

                    var methodInfo = new MethodInfo
                    {
                        DeclarationSyntax = m,
                        Symbol = methodSymbol,
                        Transformed = m.Identifier.Text + "Async",
                        WithOverride = false
                    };

                    var attr = methodSymbol.GetAttributes().Single(a => a.AttributeClass.Name == "GenerateAsync");
                    if (attr.ConstructorArguments.Length > 0 && attr.ConstructorArguments[0].Value != null)
                        methodInfo.Transformed = (string)attr.ConstructorArguments[0].Value;
                    if (attr.ConstructorArguments.Length > 1 && ((bool) attr.ConstructorArguments[1].Value))
                        methodInfo.WithOverride = true;
                    methods.Add(methodInfo);
                }
            }

            Log.LogMessage("Found {0} methods marked for async rewriting",
                           files.SelectMany(f => f.NamespaceToClasses.Values).SelectMany(ctm => ctm.Values).SelectMany(m => m).Count());

            // Second pass: transform
            foreach (var f in files)
            {
                Log.LogMessage("Writing out {0}", f.TransformedName);
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
            var inMethodSyntax = inMethodInfo.DeclarationSyntax;
            //Log.LogMessage("Method {0}: {1}", inMethodInfo.Symbol.Name, inMethodInfo.Symbol.);

            Log.LogMessage(MessageImportance.Low, "  Rewriting method {0} to {1}", inMethodInfo.Symbol.Name, inMethodInfo.Transformed);

            // Visit all method invocations inside the method, rewrite them to async if needed
            var rewriter = new MethodInvocationRewriter(Log, file.SemanticModel, _excludedTypes);
            var outMethod = (MethodDeclarationSyntax)rewriter.Visit(inMethodSyntax);

            // Method signature
            outMethod = outMethod
                .WithIdentifier(SyntaxFactory.Identifier(inMethodInfo.Transformed))
                .WithAttributeLists(new SyntaxList<AttributeListSyntax>())
                .WithModifiers(inMethodSyntax.Modifiers
                  .Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword))
                  //.Remove(SyntaxFactory.Token(SyntaxKind.OverrideKeyword))
                  //.Remove(SyntaxFactory.Token(SyntaxKind.NewKeyword))
                );

            // Transform return type adding Task<>
            var returnType = inMethodSyntax.ReturnType.ToString();
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

            if (inMethodInfo.WithOverride) {
                outMethod = outMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
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
            var symbol = (IMethodSymbol)_model.GetSymbolInfo(node).Symbol;
            if (symbol == null)
                return node;

            // Skip invocations of methods that don't have [GenerateAsync], or an Async
            // counterpart to them
            if (!symbol.GetAttributes().Any(a => a.AttributeClass.Name == "GenerateAsync") && (
                  _excludeTypes.Contains(symbol.ContainingType) ||
                  !symbol.ContainingType.GetMembers(symbol.Name + "Async").Any()
               ))
            {
                return node;
            }

            Log.LogMessage(MessageImportance.Low, "    Found rewritable invocation: " + symbol);

            // Rewrite the method name and prefix the invocation with await
            var asIdentifierName = node.Expression as IdentifierNameSyntax;
            if (asIdentifierName != null)
            {
                ExpressionSyntax rewritten = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.AwaitExpression,
                    node.WithExpression(asIdentifierName.WithIdentifier(
                        SyntaxFactory.Identifier(asIdentifierName.Identifier.Text + "Async")
                    ))
                );
                if (!(node.Parent is StatementSyntax))
                    rewritten = SyntaxFactory.ParenthesizedExpression(rewritten);
                return rewritten;
            }

            var memberAccessExp = node.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExp != null)
            {
                // Roslyn apparently doesn't visit MethodInvocationSyntax recursively, so:
                // Stream.Read().Flush() gets rewritten to await Stream.Read().FlushAsync()
                // and Read() is still sync. Opened question in the MSDN forum, for now
                // manually recurse here.
                var nestedInvocation = memberAccessExp.Expression as InvocationExpressionSyntax;
                if (nestedInvocation != null)
                    memberAccessExp = memberAccessExp.WithExpression((ExpressionSyntax)VisitInvocationExpression(nestedInvocation));

                ExpressionSyntax rewritten = SyntaxFactory.PrefixUnaryExpression(SyntaxKind.AwaitExpression,
                    node.WithExpression(memberAccessExp.WithName(
                        memberAccessExp.Name.WithIdentifier(SyntaxFactory.Identifier(memberAccessExp.Name.Identifier.Text + "Async"))
                    ))
                );
                if (!(node.Parent is StatementSyntax))
                    rewritten = SyntaxFactory.ParenthesizedExpression(rewritten);
                return rewritten;
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
        public MethodDeclarationSyntax DeclarationSyntax;
        public IMethodSymbol Symbol;
        public string Transformed;
        public bool WithOverride;
    }
}
