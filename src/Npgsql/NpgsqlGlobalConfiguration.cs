#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Npgsql
{
    /// <summary>
    /// Provides methods to access configuration data stored in a machine specific configuration file.
    /// </summary>
    internal class NpgsqlGlobalConfiguration
    {
        const string NPGSQL_CONFIG_RELATIVEPATH = "Npgsql\\Npgsql_machine.config";
        const string NPGSQL_CONFIG_DBINFOFACTORIES = "databaseinfofactories";

        static NpgsqlGlobalConfiguration current;
        /// <summary>
        /// Returns a reference to the current NpgsqlGlobalConfiguration instance.
        /// </summary>
        public static NpgsqlGlobalConfiguration Current
        {
            get
            {
                if (current == null)
                {
                    current = new NpgsqlGlobalConfiguration();

                    try
                    {
                        current.Load();
                    }
                    catch
                    {
                        System.Diagnostics.Trace.WriteLine("Failed to load global configuration.");
                    }
                }
                return current;
            }
        }

        IList<DatabaseInfoFactoryMetadata> databaseInfoFactories = new List<DatabaseInfoFactoryMetadata>();

        /// <summary>
        /// Returns the path of the configuration file.
        /// </summary>
        string ConfigFilePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), NPGSQL_CONFIG_RELATIVEPATH);
        /// <summary>
        /// Determines whether the configuration file exists.
        /// </summary>
        bool ConfigFileExists => File.Exists(ConfigFilePath);

        /// <summary>
        /// Returns an enumeration of DatabaseInfoFactoryMetadata instances which provide metadata for globally registered DatabaseInfoFactory implementations.
        /// </summary>
        public IEnumerable<DatabaseInfoFactoryMetadata> DatabaseInfoFactories
        {
            get
            {
                return databaseInfoFactories;
            }
        }
        
        void Load()
        {
            if (ConfigFileExists)
            {
                using (var reader = XmlReader.Create(ConfigFilePath))
                {
                    while(reader.Read())
                    {
                        if (reader.IsStartElement(NPGSQL_CONFIG_DBINFOFACTORIES))
                        {
                            LoadRegisteredDatabaseInfoFactories(reader.ReadSubtree());
                        }
                    }
                }
            }
        }
        
        void LoadRegisteredDatabaseInfoFactories(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.IsStartElement("add"))
                {
                    var name = "";
                    var typeName = "";

                    if (reader.MoveToAttribute("name"))
                    {
                        name = reader.ReadContentAsString();
                        reader.MoveToElement();
                    }

                    if (reader.MoveToAttribute("value"))
                    {
                        typeName = reader.ReadContentAsString();
                        reader.MoveToElement();
                    }

                    AddRegisteredDatabaseInfoFactory(name, typeName);
                }
            }
        }

        void AddRegisteredDatabaseInfoFactory(string name, string typeName)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(typeName))
                return;

            var existing = databaseInfoFactories.FirstOrDefault(r => string.Equals(r.Name, name));
            if (existing != null)
            {
                existing.TypeName = typeName;
            }
            else
            {
                databaseInfoFactories.Add(new DatabaseInfoFactoryMetadata { Name = name, TypeName = typeName });
            }
        }

        /// <summary>
        /// Provides metadata for a globally registered DatabaseInfoFactory implementation.
        /// </summary>
        public class DatabaseInfoFactoryMetadata
        {
            public string Name { get; set; }
            public string TypeName { get; set; }
        }
    }
}
