using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnitLite;

// Exists as a temporary test runner for dotnet cli
// (see https://github.com/nunit/nunit/issues/1371)
namespace Npgsql.Tests
{
    public class Program
    {
        public static int Main(string[] args)
        {
#if DNXCORE50
            return new AutoRun().Execute(typeof(Program).GetTypeInfo().Assembly, Console.Out, Console.In, args);
#else
            return new AutoRun().Execute(args);
#endif
        }
    }
}
