using System;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Reflection;

namespace GACInstall
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (String a in args)
            {
                if (File.Exists(a))
                {
                    String fp = Path.GetFullPath(a);
                    Console.WriteLine(Assembly.LoadFile(fp).FullName);
                    new Publish().GacInstall(fp);
                }
            }
        }
    }
}
