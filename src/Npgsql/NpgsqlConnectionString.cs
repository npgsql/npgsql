// created on 6/21/2004

// Npgsql.NpgsqlConnecionString.cs
//
// Author:
//	Glen Parker (glenebob@nwlink.com)
//	Ben Sagal (bensagal@gmail.com)
//
//	Copyright (C) 2002 The Npgsql Development Team
//	npgsql-general@gborg.postgresql.org
//	http://gborg.postgresql.org/project/npgsql/projdisplay.php
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


using System;
using System.Collections;
using System.Globalization;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Resources;

namespace Npgsql
{
    /// <summary>
    /// Represents a connection string.
    /// </summary>
    internal sealed class NpgsqlConnectionString : IEnumerable
    {
        // Logging related values
        private static readonly String CLASSNAME = "NpgsqlConnectionString";
        private static System.Resources.ResourceManager resman;
        
        // Regexs to extract data from connection string
        private readonly static Regex keyRegex=new Regex(@"(?<=\G(?:\s*[""'])?[\s;]*)(?:==|(?<=[^^;\s])\s*(?=[^;=\s])|[^;=\s])*(?=\s*=)(?!\s*==)");
        private readonly static Regex valueRegex= new Regex(@"(?<=\G\s*=\s*""\s*)(?:(?<![""\s])\s*(?![\s""])|[^\s""])*(?=\s*""\s*(?:$|;))|(?<=\G\s*=\s*'\s*)(?:(?<!['\s])\s*(?![\s'])|[^\s'])*(?=\s*'\s*(?:$|;))|(?<=\G\s*=\s*)(?![""'])(?:(?<![=\s])\s*(?![\s;])|[^\s;])*(?=\s*(?:$|;))");
        private readonly static Regex emptyRegex= new Regex(@"\G(?:\s*[""'])?[;\s]*$");

        private String                                 connection_string = null;
        private ListDictionary                         connection_string_values;

        static NpgsqlConnectionString()
        {
            resman = new System.Resources.ResourceManager(typeof(NpgsqlConnectionString));
        }

        private NpgsqlConnectionString(NpgsqlConnectionString Other)
        {
            connection_string = Other.connection_string;
            connection_string_values = new ListDictionary(CaseInsensitiveComparer.Default);
            foreach (DictionaryEntry DE in Other.connection_string_values)
            {
                connection_string_values.Add(DE.Key, DE.Value);
            }
        }

        private NpgsqlConnectionString(ListDictionary Values)
        {
            connection_string_values = Values;
        }

        /// <summary>
        /// Return an exact copy of this NpgsqlConnectionString.
        /// </summary>
        public NpgsqlConnectionString Clone()
        {
            return new NpgsqlConnectionString(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return connection_string_values.GetEnumerator();
        }

        /// <summary>
        /// This method parses a connection string and returns a new NpgsqlConnectionString object.
        /// </summary>
        public static NpgsqlConnectionString ParseConnectionString(String CS)
        {
            ListDictionary newValues = new ListDictionary(CaseInsensitiveComparer.Default);
            Match keyMatch;
            Match valueMatch;
            int index = 0;


            if (CS == null)
                CS = String.Empty;

            while (!emptyRegex.IsMatch(CS,index))
            {
                keyMatch = keyRegex.Match(CS,index);
                if (!keyMatch.Success)
                {
                    throw new ArgumentException(resman.GetString("Exception_WrongKeyVal"), "<INVALID>");
                }

                index=keyMatch.Index + keyMatch.Length;
                
                // Make sure the key is even there...
                if (keyMatch.Length == 0)
                {
                    throw new ArgumentException(resman.GetString("Exception_WrongKeyVal"), "<BLANK>");
                }

                // Check if there is a value.
                valueMatch = valueRegex.Match(CS,index);
                if (!valueMatch.Success)                
                {
                    throw new ArgumentException(resman.GetString("Exception_WrongKeyVal"), keyMatch.Value);
                }

                index=valueMatch.Index + valueMatch.Length;
                
                String key=keyMatch.Value.ToUpper(CultureInfo.InvariantCulture);
                
                // Substitute the real key name if this is an alias key (ODBC stuff for example)...
                String aliasKey = (string)ConnectionStringKeys.Aliases[key];

                if (aliasKey != null)
                {
                    key = aliasKey;
                }

                // Add the pair to the dictionary..
                newValues.Add(key, valueMatch.Value);
            }

            return new NpgsqlConnectionString(newValues);
        }

        /// <summary>
        /// Case insensative accessor for indivual connection string values.
        /// </summary>
        public String this[String Key]
        {
            get
            {
                return (String)connection_string_values[Key];
            }
            set
            {
                connection_string_values[Key] = value;
                connection_string = null;
            }
        }

        /// <summary>
        /// Report whether a value with the provided key name exists in this connection string.
        /// </summary>
        public Boolean Contains(String Key)
        {
            return connection_string_values.Contains(Key);
        }

        /// <summary>
        /// Return a clean string representation of this connection string.
        /// </summary>
        public override String ToString()
        {
            if (connection_string == null)
            {
                StringBuilder      S = new StringBuilder();

                foreach (DictionaryEntry DE in this)
                {
                    S.AppendFormat("{0}={1};", DE.Key, DE.Value);
                }

                connection_string = S.ToString();
            }

            return connection_string;
        }

        /// <summary>
        /// Return a string value from the current connection string, even if the
        /// given key is not in the string or if the value is null.
        /// </summary>
        public String ToString(String Key)
        {
            return ToString(Key, "");
        }

        /// <summary>
        /// Return a string value from the current connection string, even if the
        /// given key is not in the string or if the value is null.
        /// </summary>
        public String ToString(String Key, String Default)
        {
            if (! connection_string_values.Contains(Key))
            {
                return Default;
            }

            return Convert.ToString(connection_string_values[Key]);
        }

        /// <summary>
        /// Return an integer value from the current connection string, even if the
        /// given key is not in the string or if the value is null.
        /// Throw an appropriate exception if the value cannot be coerced to an integer.
        /// </summary>
        public Int32 ToInt32(String Key)
        {
            return ToInt32(Key, 0);
        }

        /// <summary>
        /// Return an integer value from the current connection string, even if the
        /// given key is not in the string or if the value is null.
        /// Throw an appropriate exception if the value cannot be coerced to an integer.
        /// </summary>
        public Int32 ToInt32(String Key, Int32 Min, Int32 Max)
        {
            return ToInt32(Key, Min, Max, 0);
        }

        /// <summary>
        /// Return an integer value from the current connection string, even if the
        /// given key is not in the string or if the value is null.
        /// Throw an appropriate exception if the value cannot be coerced to an integer.
        /// </summary>
        public Int32 ToInt32(String Key, Int32 Default)
        {
            if (! connection_string_values.Contains(Key))
            {
                return Default;
            }

            try
            {
                return Convert.ToInt32(connection_string_values[Key]);
            }
            catch (Exception E)
            {
                throw new ArgumentException(String.Format(resman.GetString("Exception_InvalidIntegerKeyVal"), Key), Key, E);
            }
        }

        /// <summary>
        /// Return an integer value from the current connection string, even if the
        /// given key is not in the string.
        /// Throw an appropriate exception if the value cannot be coerced to an integer.
        /// </summary>
        public Int32 ToInt32(String Key, Int32 Min, Int32 Max, Int32 Default)
        {
            Int32   V;

            V = ToInt32(Key, Default);

            if (V < Min)
            {
                throw new ArgumentException(String.Format(resman.GetString("Exception_IntegerKeyValMin"), Key, Min), Key);
            }
            if (V > Max)
            {
                throw new ArgumentException(String.Format(resman.GetString("Exception_IntegerKeyValMax"), Key, Max), Key);
            }

            return V;
        }

        /// <summary>
        /// Return a boolean value from the current connection string, even if the
        /// given key is not in the string.
        /// Throw an appropriate exception if the value is not recognized as a boolean.
        /// </summary>
        public Boolean ToBool(String Key)
        {
            return ToBool(Key, false);
        }

        /// <summary>
        /// Return a boolean value from the current connection string, even if the
        /// given key is not in the string.
        /// Throw an appropriate exception if the value is not recognized as a boolean.
        /// </summary>
        public Boolean ToBool(String Key, Boolean Default)
        {
            if (! connection_string_values.Contains(Key))
            {
                return Default;
            }

            switch (connection_string_values[Key].ToString().ToLower())
            {
            case "t" :
            case "true" :
            case "y" :
            case "yes" :
                return true;

            case "f" :
            case "false" :
            case "n" :
            case "no" :
                return false;

            default :
                throw new ArgumentException(String.Format(resman.GetString("Exception_InvalidBooleanKeyVal"), Key), Key);

            }
        }

        /// <summary>
        /// Return a ProtocolVersion from the current connection string, even if the
        /// given key is not in the string.
        /// Throw an appropriate exception if the value is not recognized as
        /// integer 2 or 3.
        /// </summary>
        public ProtocolVersion ToProtocolVersion(String Key)
        {
            if (! connection_string_values.Contains(Key))
            {
                return ProtocolVersion.Version3;
            }

            switch (ToInt32(Key))
            {
            case 2 :
                return ProtocolVersion.Version2;

            case 3 :
                return ProtocolVersion.Version3;

            default :
                throw new ArgumentException(String.Format(resman.GetString("Exception_InvalidProtocolVersionKeyVal"), Key), Key);

            }
        }
        
        public SslMode ToSslMode(String Key)
        {
            return ToSslMode(Key, SslMode.Disable);
        }
        
        ///<summary>
        ///</summary>
        
        public SslMode ToSslMode(String Key, SslMode Default)
        {
            if (! connection_string_values.Contains(Key))
            {
                return Default;
            }
            
            return (SslMode)Enum.Parse(typeof(SslMode), ToString(Key), true);
        }


        
    }


    /// <summary>
    /// Know connection string keys.
    /// </summary>
    internal abstract class ConnectionStringKeys
    {
        public static readonly String Host                  = "SERVER";
        public static readonly String Port                  = "PORT";
        public static readonly String Protocol              = "PROTOCOL";
        public static readonly String Database              = "DATABASE";
        public static readonly String UserName              = "USER ID";
        public static readonly String Password              = "PASSWORD";
        public static readonly String SSL                   = "SSL";
        public static readonly String SslMode               = "SSLMODE";
        public static readonly String Encoding              = "ENCODING";
        public static readonly String Timeout               = "TIMEOUT";
        
        // These are for the connection pool
        public static readonly String Pooling               = "POOLING";
        public static readonly String ConnectionLifeTime    = "CONNECTIONLIFETIME";
        public static readonly String MinPoolSize           = "MINPOOLSIZE";
        public static readonly String MaxPoolSize           = "MAXPOOLSIZE";
        public static readonly String SyncNotification      = "SYNCNOTIFICATION";

        // These are for the command
        public static readonly String CommandTimeout        = "COMMANDTIMEOUT";

        // A list of aliases for some of the above values.  If one of these aliases is
        // encountered when parsing a connection string, it's real key name will
        // be used instead.  These will be reflected if ToString() is used to inspect
        // the string.
        private static ListDictionary _aliases;

        static ConnectionStringKeys()
        {
            _aliases = new ListDictionary();

            // Aliases to help catch common errors.
            _aliases.Add("DB", Database);
            _aliases.Add("HOST", Host);
            _aliases.Add("USER", UserName);
            _aliases.Add("USERID", UserName);
            _aliases.Add("USER NAME", UserName);
            _aliases.Add("USERNAME", UserName);
            _aliases.Add("PSW", Password);

            // Aliases to make migration from ODBC easier.
            _aliases.Add("UID", UserName);
            _aliases.Add("PWD", Password);
        }

        public static IDictionary Aliases
        {
            get
            {
                return _aliases;
            }
        }
    }

    /// <summary>
    /// Connection string default values.
    /// </summary>
    internal abstract class ConnectionStringDefaults
    {
        // Connection string defaults
        public static readonly Int32 Port                   = 5432;
        public static readonly String Encoding              = "SQL_ASCII";
        public static readonly Boolean Pooling              = true;
        public static readonly Int32 MinPoolSize            = 1;
        public static readonly Int32 MaxPoolSize            = 20;
        public static readonly Int32 Timeout                = 15; // Seconds
        public static readonly Int32 ConnectionLifeTime     = 15; // Seconds
        public static readonly Boolean SyncNotification     = false;
        public static readonly Int32 CommandTimeout         = 20; // Seconds
    }
    
    internal enum SslMode
    {
        Disable = 1 << 0,
        Allow =   1 << 1,
        Prefer =  1 << 2,
        Require = 1 << 3
    }



}
