using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Common;
using NUnitLite;
using System.Reflection;

// Exists as a temporary test runner for dotnet cli
// (see https://github.com/nunit/nunit/issues/1371)
namespace Npgsql.Tests
{
    public class Program
    {
        public static int Main(string[] args)
        {
#if NET451
            return new AutoRun().Execute(args);
#else
            var writer = new ExtendedTextWrapper(Console.Out);
            return new AutoRun(typeof(Program).GetTypeInfo().Assembly).Execute(args, writer, Console.In);
#endif
        }
    }
}
