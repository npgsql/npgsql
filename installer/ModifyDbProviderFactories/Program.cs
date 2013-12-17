using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
#if NET40
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
#endif

namespace ModifyDbProviderFactories {
    class Program {
        static void Main(string[] args) {
            if (args.Length >= 6 && args[0] == "/add-or-replace" && File.Exists(args[1])) {
#if NET40
                XDocument xApp = XDocument.Load(args[1]);
                var DbProviderFactories = xApp.Element("configuration").Element("system.data").Element("DbProviderFactories");
                foreach (var el in DbProviderFactories
                    .Elements("add")
                    .Where(p => p.Attribute("invariant") != null && p.Attribute("invariant").Value == args[3])
                    ) {
                    el.AddAfterSelf(
                        new XComment(el.ToString())
                        );
                    el.Remove();
                }
                XElement xAdd;
                DbProviderFactories.Add(xAdd = new XElement("add"
                    , new XAttribute("name", args[2])
                    , new XAttribute("invariant", args[3])
                    , new XAttribute("description", args[4])
                    , new XAttribute("type", args[5])
                    ));
                for (int x = 6; x + 1 < args.Length; x += 2) {
                    xAdd.SetAttributeValue(args[x], args[x + 1]);
                }
                xApp.Save(args[1]);
#else
                XmlDocument xApp = new XmlDocument();
                xApp.Load(args[1]);
                var DbProviderFactories = xApp.SelectSingleNode("/configuration/system.data/DbProviderFactories");
                foreach (XmlElement el in DbProviderFactories.SelectNodes("add[@invariant='" + args[3] + "']")) {
                    el.ParentNode.InsertAfter(xApp.CreateComment(el.OuterXml), el);
                    el.ParentNode.RemoveChild(el);
                }
                XmlElement xAdd = xApp.CreateElement("add");
                DbProviderFactories.AppendChild(xAdd);
                xAdd.SetAttribute("name", args[2]);
                xAdd.SetAttribute("invariant", args[3]);
                xAdd.SetAttribute("description", args[4]);
                xAdd.SetAttribute("type", args[5]);
                for (int x = 6; x + 1 < args.Length; x += 2) {
                    xAdd.SetAttribute(args[x], args[x + 1]);
                }
                xApp.Save(args[1]);
#endif
                return;
            }
            if (args.Length == 3 && args[0] == "/remove" && File.Exists(args[1])) {
#if NET40
                XDocument xApp = XDocument.Load(args[1]);
                var DbProviderFactories = xApp.Element("configuration").Element("system.data").Element("DbProviderFactories");
                foreach (var el in DbProviderFactories
                    .Elements("add")
                    .Where(p => p.Attribute("invariant") != null && p.Attribute("invariant").Value == args[2])
                    ) {
                    el.Remove();
                }
                xApp.Save(args[1]);
#else
                XmlDocument xApp = new XmlDocument();
                xApp.Load(args[1]);
                var DbProviderFactories = xApp.SelectSingleNode("/configuration/system.data/DbProviderFactories");
                foreach (XmlElement el in DbProviderFactories.SelectNodes("add[@invariant='" + args[2] + "']")) {
                    el.ParentNode.RemoveChild(el);
                }
                xApp.Save(args[1]);
#endif
                return;
            }
            Console.Error.WriteLine("ModifyDbProviderFactories");
            Console.Error.WriteLine(" /add-or-replace <app.config> <name> <invariant> <description> <type> support FF ... ");
            Console.Error.WriteLine(" /remove <app.config> <invariant> ");
            Environment.ExitCode = 1;
        }
    }
}
