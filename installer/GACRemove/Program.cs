using System;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Reflection;

namespace GACRemove
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (String a in args)
            {
                try
                {
                    Assembly dll = Assembly.Load(a);
                    Console.WriteLine(dll.FullName);
                    new Publish().GacRemove(new Uri(dll.CodeBase).LocalPath);
                }
                catch (FileNotFoundException)
                {

                }
            }
        }
    }
}
