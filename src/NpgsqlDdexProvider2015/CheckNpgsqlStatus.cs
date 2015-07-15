using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Npgsql.VisualStudio.Provider
{
    class CheckNpgsqlStatus
    {
        public static bool NeedInst
        {
            get
            {
                String fpxml = Ut.HostConfig;
                if (File.Exists(fpxml))
                {
                    XmlDocument xmlo = new XmlDocument();
                    xmlo.Load(fpxml);

                    foreach (XmlElement elAdd in xmlo.SelectNodes("/configuration/system.data/DbProviderFactories/add[@invariant='Npgsql']"))
                    {
                        String itsType = Regex.Replace(elAdd.GetAttribute("type"), "\\s+", "").ToLowerInvariant();
                        String myType = Regex.Replace(typeof(Npgsql.NpgsqlFactory).AssemblyQualifiedName, "\\s+", "").ToLowerInvariant();
                        if (myType.Equals(itsType))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        public class Ut
        {
            public static string HostConfig
            {
                get
                {
                    return System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName + ".config";
                }
            }
        }

        internal static void DoInst()
        {
            String fpxml = Ut.HostConfig;
            if (File.Exists(fpxml))
            {
                XmlDocument xmlo = new XmlDocument();
                if (File.Exists(fpxml))
                    xmlo.Load(fpxml);

                // http://stackoverflow.com/a/722251
                foreach (XmlElement el in xmlo.SelectNodes("/configuration/system.data/DbProviderFactories/*[self::add[@invariant='Npgsql'] or self::remove[@invariant='Npgsql']]"))
                {
                    el.ParentNode.RemoveChild(el);
                }

                {
                    XmlElement elc = xmlo.SelectSingleNode("configuration") as XmlElement;
                    if (elc == null)
                    {
                        xmlo.AppendChild(elc = xmlo.CreateElement("configuration"));
                    }

                    XmlElement els = elc.SelectSingleNode("system.data") as XmlElement;
                    if (els == null)
                    {
                        elc.AppendChild(els = xmlo.CreateElement("system.data"));
                    }

                    XmlElement eld = els.SelectSingleNode("DbProviderFactories") as XmlElement;
                    if (eld == null)
                    {
                        els.AppendChild(eld = xmlo.CreateElement("DbProviderFactories"));
                    }

                    {
                        XmlElement elr = xmlo.CreateElement("remove");
                        {
                            elr.SetAttribute("invariant", "Npgsql");
                        }
                        eld.AppendChild(elr);
                    }
                    {
                        XmlElement ela = xmlo.CreateElement("add");
                        {
                            ela.SetAttribute("name", "Npgsql Data Provider");
                            ela.SetAttribute("invariant", "Npgsql");
                            ela.SetAttribute("description", ".Net Data Provider for PostgreSQL");
                            ela.SetAttribute("type", typeof(Npgsql.NpgsqlFactory).AssemblyQualifiedName);
                        }
                        eld.AppendChild(ela);
                    }
                }

                xmlo.Save(fpxml);
            }
        }
    }
}
