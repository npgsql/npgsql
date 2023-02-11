using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace MStatDumper
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                throw new Exception("Must provide the path to mstat file. It's in {project}/obj/Release/{TFM}/{os}/native/{project}.mstat");
            }

            var markDownStyleOutput = args.Length > 1 && args[1] == "md";
            var methodsFilePath = args.Length > 2 ? args[2] : string.Empty;

            var asm = AssemblyDefinition.ReadAssembly(args[0]);
            var globalType = (TypeDefinition)asm.MainModule.LookupToken(0x02000001);

            var types = globalType.Methods.First(x => x.Name == "Types");
            var typeStats = GetTypes(types).ToList();
            var typeSize = typeStats.Sum(x => x.Size);
            var typesByModules = typeStats.GroupBy(x => x.Type.Scope).Select(x => new { x.Key.Name, Sum = x.Sum(x => x.Size) }).ToList();
            if (markDownStyleOutput)
            {
                Console.WriteLine("<details>");
                Console.WriteLine($"<summary>Types Total Size {typeSize:n0}</summary>");
                Console.WriteLine();
                Console.WriteLine("<br>");
                Console.WriteLine();
                Console.WriteLine("| Name | Size |");
                Console.WriteLine("| --- | --- |");
                foreach (var m in typesByModules.OrderByDescending(x => x.Sum))
                {
                    Console.WriteLine($"| {m.Name.Replace("`", "\\`")} | {m.Sum:n0} |");
                }
                Console.WriteLine();
                Console.WriteLine("</details>");
            }
            else
            {
                Console.WriteLine($"// ********** Types Total Size {typeSize:n0}");
                foreach (var m in typesByModules.OrderByDescending(x => x.Sum))
                {
                    Console.WriteLine($"{m.Name,-70} {m.Sum,9:n0}");
                }
                Console.WriteLine("// **********");
            }

            Console.WriteLine();

            var methods = globalType.Methods.First(x => x.Name == "Methods");
            var methodStats = GetMethods(methods).ToList();
            var methodSize = methodStats.Sum(x => x.Size + x.GcInfoSize + x.EhInfoSize);
            var methodsByModules = methodStats.GroupBy(x => x.Method.DeclaringType.Scope).Select(x => new { x.Key.Name, Sum = x.Sum(x => x.Size + x.GcInfoSize + x.EhInfoSize) }).ToList();
            if (markDownStyleOutput)
            {
                Console.WriteLine("<details>");
                Console.WriteLine($"<summary>Methods Total Size {methodSize:n0}</summary>");
                Console.WriteLine();
                Console.WriteLine("<br>");
                Console.WriteLine();
                Console.WriteLine("| Name | Size |");
                Console.WriteLine("| --- | --- |");
                foreach (var m in methodsByModules.OrderByDescending(x => x.Sum))
                {
                    Console.WriteLine($"| {m.Name.Replace("`", "\\`")} | {m.Sum:n0} |");
                }
                Console.WriteLine();
                Console.WriteLine("</details>");
            }
            else
            {
                Console.WriteLine($"// ********** Methods Total Size {methodSize:n0}");
                foreach (var m in methodsByModules.OrderByDescending(x => x.Sum))
                {
                    Console.WriteLine($"{m.Name,-70} {m.Sum,9:n0}");
                }
                Console.WriteLine("// **********");
            }

            Console.WriteLine();

            string FindNamespace(TypeReference type)
            {
                var current = type;
                while (true)
                {
                    if (!string.IsNullOrEmpty(current.Namespace))
                    {
                        return current.Namespace;
                    }

                    if (current.DeclaringType == null)
                    {
                        return current.Name;
                    }

                    current = current.DeclaringType;
                }
            }

            var methodsByNamespace = methodStats.Select(x => new TypeStats { Type = x.Method.DeclaringType, Size = x.Size + x.GcInfoSize + x.EhInfoSize }).Concat(typeStats).GroupBy(x => FindNamespace(x.Type)).Select(x => new { x.Key, Sum = x.Sum(x => x.Size) }).ToList();
            if (markDownStyleOutput)
            {
                Console.WriteLine("<details>");
                Console.WriteLine("<summary>Size By Namespace</summary>");
                Console.WriteLine();
                Console.WriteLine("<br>");
                Console.WriteLine();
                Console.WriteLine("| Name | Size |");
                Console.WriteLine("| --- | --- |");
                foreach (var m in methodsByNamespace.OrderByDescending(x => x.Sum))
                {
                    Console.WriteLine($"| {m.Key.Replace("`", "\\`")} | {m.Sum:n0} |");
                }
                Console.WriteLine();
                Console.WriteLine("</details>");
            }
            else
            {
                Console.WriteLine("// ********** Size By Namespace");
                foreach (var m in methodsByNamespace.OrderByDescending(x => x.Sum))
                {
                    Console.WriteLine($"{m.Key,-70} {m.Sum,9:n0}");
                }
                Console.WriteLine("// **********");
            }

            Console.WriteLine();

            var blobs = globalType.Methods.First(x => x.Name == "Blobs");
            var blobStats = GetBlobs(blobs).ToList();
            var blobSize = blobStats.Sum(x => x.Size);
            if (markDownStyleOutput)
            {
                Console.WriteLine("<details>");
                Console.WriteLine($"<summary>Blobs Total Size {blobSize:n0}</summary>");
                Console.WriteLine();
                Console.WriteLine("<br>");
                Console.WriteLine();
                Console.WriteLine("| Name | Size |");
                Console.WriteLine("| --- | --- |");
                foreach (var m in blobStats.OrderByDescending(x => x.Size))
                {
                    Console.WriteLine($"| {m.Name.Replace("`", "\\`")} | {m.Size:n0} |");
                }
                Console.WriteLine();
                Console.WriteLine("</details>");
            }
            else
            {
                Console.WriteLine($"// ********** Blobs Total Size {blobSize:n0}");
                foreach (var m in blobStats.OrderByDescending(x => x.Size))
                {
                    Console.WriteLine($"{m.Name,-70} {m.Size,9:n0}");
                }
                Console.WriteLine("// **********");
            }

            if (!string.IsNullOrEmpty(methodsFilePath))
            {
                var methodsByClass = methodStats
                    .Where(x => x.Method.DeclaringType.Scope.Name == "Npgsql")
                    .GroupBy(x => (x.Method.DeclaringType.DeclaringType ?? x.Method.DeclaringType).Name)
                    .ToList();

                using var file = File.Create(methodsFilePath);
                using var sw = new StreamWriter(file);

                sw.WriteLine("<details>");
                sw.WriteLine("<summary>Methods Size By Class</summary>");
                sw.WriteLine();
                sw.WriteLine("<br>");
                sw.WriteLine();
                sw.WriteLine("| Name | Size |");
                sw.WriteLine("| --- | --- |");
                foreach (var m in methodsByClass
                             .Select(x => new { Name = x.Key, Sum = x.Sum(x => x.Size + x.GcInfoSize + x.EhInfoSize) })
                             .OrderByDescending(x => x.Sum))
                {
                    sw.WriteLine($"| {m.Name.Replace("`", "\\`")} | {m.Sum:n0} |");
                }

                foreach (var g in methodsByClass)
                {
                    sw.WriteLine();
                    sw.WriteLine("<details>");
                    sw.WriteLine($"<summary>\"{g.Key}\" Methods</summary>");
                    sw.WriteLine();
                    sw.WriteLine("<br>");
                    sw.WriteLine();
                    sw.WriteLine("| Name | Size |");
                    sw.WriteLine("| --- | --- |");
                    foreach (var m in g
                                 .Select(x => new { x.Method.Name, Size = x.Size + x.GcInfoSize + x.EhInfoSize})
                                 .OrderByDescending(x => x.Size))
                    {
                        sw.WriteLine($"| {m.Name.Replace("`", "\\`")} | {m.Size:n0} |");
                    }
                    sw.WriteLine();
                    sw.WriteLine("</details>");
                }

                sw.WriteLine();
                sw.WriteLine("</details>");
            }
        }

        public static IEnumerable<TypeStats> GetTypes(MethodDefinition types)
        {
            types.Body.SimplifyMacros();
            var il = types.Body.Instructions;
            for (var i = 0; i + 2 < il.Count; i += 2)
            {
                var type = (TypeReference)il[i + 0].Operand;
                var size = (int)il[i + 1].Operand;
                yield return new TypeStats
                {
                    Type = type,
                    Size = size
                };
            }
        }

        public static IEnumerable<MethodStats> GetMethods(MethodDefinition methods)
        {
            methods.Body.SimplifyMacros();
            var il = methods.Body.Instructions;
            for (var i = 0; i + 4 < il.Count; i += 4)
            {
                var method = (MethodReference)il[i + 0].Operand;
                var size = (int)il[i + 1].Operand;
                var gcInfoSize = (int)il[i + 2].Operand;
                var ehInfoSize = (int)il[i + 3].Operand;
                yield return new MethodStats
                {
                    Method = method,
                    Size = size,
                    GcInfoSize = gcInfoSize,
                    EhInfoSize = ehInfoSize
                };
            }
        }

        public static IEnumerable<BlobStats> GetBlobs(MethodDefinition blobs)
        {
            blobs.Body.SimplifyMacros();
            var il = blobs.Body.Instructions;
            for (var i = 0; i + 2 < il.Count; i += 2)
            {
                var name = (string)il[i + 0].Operand;
                var size = (int)il[i + 1].Operand;
                yield return new BlobStats
                {
                    Name = name,
                    Size = size
                };
            }
        }
    }

    public class TypeStats
    {
        public string MethodName { get; set; }
        public TypeReference Type { get; set; }
        public int Size { get; set; }
    }

    public class MethodStats
    {
        public MethodReference Method { get; set; }
        public int Size { get; set; }
        public int GcInfoSize { get; set; }
        public int EhInfoSize { get; set; }
    }

    public class BlobStats
    {
        public string Name { get; set; }
        public int Size { get; set; }
    }
}
