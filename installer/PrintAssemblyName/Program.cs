using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintAssemblyName {
    class Program {
        static void Main(string[] args) {
            try {
                foreach (string fp in args) {
                    Console.Write(System.Reflection.Assembly.LoadFile(fp).FullName);
                    break;
                }
            }
            catch (Exception err) {
                Console.Error.WriteLine(err);
                Environment.ExitCode = 1;
            }
        }
    }
}
