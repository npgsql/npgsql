#if DNX46
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnitLite;

// Exists as a temporary test runner for DNX
// (see http://www.alteridem.net/2015/11/04/testing-net-core-using-nunit-3/)
namespace Npgsql.Tests
{
    public class Program
    {
        public int Main(string[] args)
        {
#if DNX46
            return new AutoRun().Execute(args);
#else
            return new AutoRun().Execute(typeof(Program).GetTypeInfo().Assembly, Console.Out, Console.In, args);
#endif
        }
    }
}
#endif
