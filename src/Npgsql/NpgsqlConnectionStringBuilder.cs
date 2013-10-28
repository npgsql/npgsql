// created on 29/11/2007

// Npgsql.NpgsqlConnectionStringBuilder.cs
//
// Author:
//    Glen Parker (glenebob@nwlink.com)
//    Ben Sagal (bensagal@gmail.com)
//    Tao Wang (dancefire@gmail.com)
//
//    Copyright (C) 2007 The Npgsql Development Team
//    npgsql-general@gborg.postgresql.org
//    http://gborg.postgresql.org/project/npgsql/projdisplay.php
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Npgsql
{
    public sealed class NpgsqlConnectionStringBuilder : DbConnectionStringBuilder
    {
        private static readonly ResourceManager resman = new ResourceManager(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IDictionary<Keywords, object> defaults = new Dictionary<Keywords, object>();
        private static readonly IDictionary<string, Keywords> keyword_mappings = new Dictionary<string, Keywords>();
        private static readonly IDictionary<Keywords, string> underlying_keywords = new Dictionary<Keywords, string>();
        private static readonly IDictionary<Keywords, PropertyInfo> props = new Dictionary<Keywords, PropertyInfo>();
        private static readonly IDictionary<string, string> prop_key = new Dictionary<string, string>();

        private const int POOL_SIZE_LIMIT = 1024;
        private const int TIMEOUT_LIMIT = 1024;

        private bool SuppressCheckValues = false;

        [AttributeUsage(AttributeTargets.Property)]
        private sealed class NpgsqlConnectionStringKeywordAttribute : Attribute
        {
            public Keywords Keyword;
            public string UnderlyingConnectionKeyword;
            public bool IsInternal = false;
            public NpgsqlConnectionStringKeywordAttribute(Keywords keyword, bool is_internal = false)
            {
                this.Keyword = keyword;
                this.UnderlyingConnectionKeyword = keyword.ToString().ToUpperInvariant();
                this.IsInternal = is_internal;
            }
            public NpgsqlConnectionStringKeywordAttribute(Keywords keyword, string underlying_connection_keyword)
            {
                this.Keyword = keyword;
                this.UnderlyingConnectionKeyword = underlying_connection_keyword;
            }
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
        private sealed class NpgsqlConnectionStringAcceptableKeywordAttribute : Attribute
        {
            public string Keyword;
            public NpgsqlConnectionStringAcceptableKeywordAttribute(string keyword)
            {
                this.Keyword = keyword;
            }
        }

        private sealed class NpgsqlConnectionStringCategoryAttribute : CategoryAttribute
        {
            public NpgsqlConnectionStringCategoryAttribute(String category) : base(category) { }
            protected override string GetLocalizedString(string value)
            {
                return resman.GetString(value);
            }
        }

        private sealed class NpgsqlConnectionStringDisplayNameAttribute : DisplayNameAttribute
        {
            public NpgsqlConnectionStringDisplayNameAttribute(string resourceName) : base(resourceName)
            {
                try
                {
                    string value = resman.GetString(resourceName);
                    if (value != null)
                        DisplayNameValue = value;
                }
                catch (Exception e)
                {
                }
            }
        }

        private sealed class NpgsqlConnectionStringDescriptionAttribute : DescriptionAttribute
        {
            public NpgsqlConnectionStringDescriptionAttribute(string resourceName) : base(resourceName)
            {
                try
                {
                    string value = resman.GetString(resourceName);
                    if (value != null)
                        DescriptionValue = value;
                }
                catch (Exception e)
                {
                }
            }
        }

        private sealed class NpgsqlEnumConverter<T> : EnumConverter
        {
            public NpgsqlEnumConverter() : base(typeof(T)) { }
            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                return value.ToString();
            }
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                return value.ToString();
            }
        }

        static NpgsqlConnectionStringBuilder()
        {
            try
            {
                foreach (PropertyInfo prop in typeof(NpgsqlConnectionStringBuilder).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    NpgsqlConnectionStringKeywordAttribute current_keywordattr = null;
                    foreach (Attribute attr in prop.GetCustomAttributes(typeof(NpgsqlConnectionStringKeywordAttribute), false))
                        current_keywordattr = (NpgsqlConnectionStringKeywordAttribute)attr;
                    if (current_keywordattr == null)
                        continue;
                    if (!current_keywordattr.IsInternal)
                        props[current_keywordattr.Keyword] = prop;
                    keyword_mappings[current_keywordattr.Keyword.ToString()] = current_keywordattr.Keyword;
                    keyword_mappings[current_keywordattr.Keyword.ToString().ToUpperInvariant()] = current_keywordattr.Keyword;
                    keyword_mappings[current_keywordattr.Keyword.ToString().ToLowerInvariant()] = current_keywordattr.Keyword;
                    keyword_mappings[current_keywordattr.UnderlyingConnectionKeyword] = current_keywordattr.Keyword;
                    keyword_mappings[current_keywordattr.UnderlyingConnectionKeyword.ToUpperInvariant()] = current_keywordattr.Keyword;
                    keyword_mappings[current_keywordattr.UnderlyingConnectionKeyword.ToLowerInvariant()] = current_keywordattr.Keyword;
                    underlying_keywords[current_keywordattr.Keyword] = current_keywordattr.UnderlyingConnectionKeyword;
                    foreach (Attribute attr in prop.GetCustomAttributes(typeof(NpgsqlConnectionStringAcceptableKeywordAttribute), false))
                        keyword_mappings[((NpgsqlConnectionStringAcceptableKeywordAttribute)attr).Keyword] = current_keywordattr.Keyword;
                    foreach (Attribute attr in prop.GetCustomAttributes(typeof(NpgsqlConnectionStringDisplayNameAttribute), false))
                    {
                        keyword_mappings[((NpgsqlConnectionStringDisplayNameAttribute)attr).DisplayName] = current_keywordattr.Keyword;
                        prop_key[current_keywordattr.UnderlyingConnectionKeyword] = ((NpgsqlConnectionStringDisplayNameAttribute)attr).DisplayName;
                    }
                    foreach (Attribute attr in prop.GetCustomAttributes(typeof(DefaultValueAttribute), true))
                        defaults[current_keywordattr.Keyword] = ((DefaultValueAttribute)attr).Value;
                }
                defaults[Keywords.Protocol] = ProtocolVersion.Version3.ToString();
                defaults[Keywords.SslMode] = SslMode.Disable.ToString();
                defaults[Keywords.Compatible] = THIS_VERSION.ToString();
            }
            catch (Exception ex)
            {
            }
        }

        public NpgsqlConnectionStringBuilder()
        {
            this.Clear();
        }

        public NpgsqlConnectionStringBuilder(string connectionString)
        {
            this.Clear();
            this.ConnectionString = connectionString;
        }

        new public string ConnectionString
        {
            get { return base.ConnectionString; }
            set
            {
                SuppressCheckValues = true;
                base.ConnectionString = value;
                SuppressCheckValues = false;
                CheckValues();
            }
        }

        /// <summary>
        /// Return an exact copy of this NpgsqlConnectionString.
        /// </summary>
        public NpgsqlConnectionStringBuilder Clone()
        {
            return new NpgsqlConnectionStringBuilder(this.ConnectionString);
        }

        private void CheckValues()
        {
            // At Initialization, not all default properties are set, ignore for the moment
            if (SuppressCheckValues || !ContainsKey(Keywords.MinPoolSize) || !ContainsKey(Keywords.MaxPoolSize))
                return;

            if ((MaxPoolSize > 0) && (MinPoolSize > MaxPoolSize))
            {
                string key = underlying_keywords[Keywords.MinPoolSize];
                throw new ArgumentOutOfRangeException(
                    key, String.Format(resman.GetString("Exception_IntegerKeyValMax"), key, MaxPoolSize));
            }
        }

        #region Parsing Functions

        private static int ToInt32(object value, int min, int max, string key)
        {
            int v = Convert.ToInt32(value);

            if (v < min)
            {
                throw new ArgumentOutOfRangeException(
                    key, String.Format(resman.GetString("Exception_IntegerKeyValMin"), key, min));
            }
            else if (v > max)
            {
                throw new ArgumentOutOfRangeException(
                    key, String.Format(resman.GetString("Exception_IntegerKeyValMax"), key, max));
            }

            return v;
        }

        #endregion

        #region Properties

        [NpgsqlConnectionStringCategory("DataCategory_Source")]
        [NpgsqlConnectionStringKeyword(Keywords.Host)]
        [NpgsqlConnectionStringAcceptableKeyword("SERVER")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Host")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Host")]
        [RefreshProperties(RefreshProperties.All)]
        public string Host
        {
            get { return (string) GetValue(Keywords.Host); }
            set { SetValue(Keywords.Host, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Source")]
        [NpgsqlConnectionStringKeyword(Keywords.Port)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Port")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Port")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(5432)]
        public int Port
        {
            get { return (int)GetValue(Keywords.Port); }
            set { SetValue(Keywords.Port, value); }
        }

        [Browsable(false)]
        internal ProtocolVersion Protocol
        {
            get { return (ProtocolVersion)Enum.Parse(typeof(ProtocolVersion), ProtocolAsString); }
            set { ProtocolAsString = value.ToString(); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Advanced")]
        [NpgsqlConnectionStringKeyword(Keywords.Protocol)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Protocol")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Protocol")]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(NpgsqlEnumConverter<ProtocolVersion>))]
        public string ProtocolAsString
        {
            get { return (string)base[underlying_keywords[Keywords.Protocol]]; }
            set { SetValue(Keywords.Protocol, Enum.Parse(typeof(ProtocolVersion), value).ToString()); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Source")]
        [NpgsqlConnectionStringKeyword(Keywords.Database)]
        [NpgsqlConnectionStringAcceptableKeyword("DB")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Database")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Database")]
        [RefreshProperties(RefreshProperties.All)]
        public string Database
        {
            get { return (string)GetValue(Keywords.Database); }
            set { SetValue(Keywords.Database, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Security")]
        [NpgsqlConnectionStringKeyword(Keywords.UserName, "USER ID")]
        [NpgsqlConnectionStringAcceptableKeyword("USER NAME")]
        [NpgsqlConnectionStringAcceptableKeyword("USERID")]
        [NpgsqlConnectionStringAcceptableKeyword("USER ID")]
        [NpgsqlConnectionStringAcceptableKeyword("UID")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_UserName")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_UserName")]
        [RefreshProperties(RefreshProperties.All)]
        public string UserName
        {
            get
            {
                if (IntegratedSecurity && (String.IsNullOrEmpty((string)GetValue(Keywords.UserName))))
                {
                    System.Security.Principal.WindowsIdentity identity =
                        System.Security.Principal.WindowsIdentity.GetCurrent();
                    return identity.Name.Split('\\')[1];
                }
                return (string) GetValue(Keywords.UserName);
            }
            set { SetValue(Keywords.UserName, value); }
        }

        [Browsable(false)]
        public byte[] PasswordAsByteArray
        {
            get { return System.Text.Encoding.UTF8.GetBytes((string)base[Keywords.Password.ToString()]); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Security")]
        [NpgsqlConnectionStringKeyword(Keywords.Password)]
        [NpgsqlConnectionStringAcceptableKeyword("PSW")]
        [NpgsqlConnectionStringAcceptableKeyword("PWD")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Password")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Password")]
        [RefreshProperties(RefreshProperties.All)]
        [PasswordPropertyText(true)]
        public string Password
        {
            get { return (string)GetValue(Keywords.Password); }
            set { SetValue(Keywords.Password, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Advanced")]
        [NpgsqlConnectionStringKeyword(Keywords.SSL)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_SSL")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_SSL")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool SSL
        {
            get { return (bool)GetValue(Keywords.SSL); }
            set { SetValue(Keywords.SSL, value); }
        }

        [Browsable(false)]
        internal SslMode SslMode
        {
            get { return (SslMode)Enum.Parse(typeof(SslMode), SslModeAsString); }
            set { SslModeAsString = value.ToString(); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Advanced")]
        [NpgsqlConnectionStringKeyword(Keywords.SslMode)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_SslMode")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_SslMode")]
        [RefreshProperties(RefreshProperties.All)]
        [TypeConverter(typeof(NpgsqlEnumConverter<SslMode>))]
        public string SslModeAsString
        {
            get { return (string)base[underlying_keywords[Keywords.SslMode]]; }
            set { SetValue(Keywords.SslMode, Enum.Parse(typeof(SslMode), value).ToString()); }
        }

        [Browsable(false)]
        [Obsolete("UTF-8 is always used regardless of this setting.")]
        public string Encoding
        {
            get { return "UNICODE"; }
            //set { }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Initialization")]
        [NpgsqlConnectionStringKeyword(Keywords.Timeout)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Timeout")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Timeout")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(15)]
        public int Timeout
        {
            get { return (int)GetValue(Keywords.Timeout); }
            set { SetValue(Keywords.Timeout, ToInt32(value, 0, TIMEOUT_LIMIT, Keywords.Timeout.ToString())); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Context")]
        [NpgsqlConnectionStringKeyword(Keywords.SearchPath)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_SearchPath")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_SearchPath")]
        [RefreshProperties(RefreshProperties.All)]
        public string SearchPath
        {
            get { return (string)GetValue(Keywords.SearchPath); }
            set { SetValue(Keywords.SearchPath, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Pooling")]
        [NpgsqlConnectionStringKeyword(Keywords.Pooling)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Pooling")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Pooling")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(true)]
        public bool Pooling
        {
            get { return (bool)GetValue(Keywords.Pooling); }
            set { SetValue(Keywords.Pooling, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Pooling")]
        [NpgsqlConnectionStringKeyword(Keywords.ConnectionLifeTime)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_ConnectionLifeTime")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_ConnectionLifeTime")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(15)]
        public int ConnectionLifeTime
        {
            get { return (int)GetValue(Keywords.ConnectionLifeTime); }
            set { SetValue(Keywords.ConnectionLifeTime, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Pooling")]
        [NpgsqlConnectionStringKeyword(Keywords.MinPoolSize)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_MinPoolSize")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_MinPoolSize")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(1)]
        public int MinPoolSize
        {
            get { return (int)GetValue(Keywords.MinPoolSize); }
            set { SetValue(Keywords.MinPoolSize, ToInt32(value, 0, POOL_SIZE_LIMIT, Keywords.MinPoolSize.ToString())); CheckValues(); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Pooling")]
        [NpgsqlConnectionStringKeyword(Keywords.MaxPoolSize)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_MaxPoolSize")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_MaxPoolSize")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(20)]
        public int MaxPoolSize
        {
            get { return (int)GetValue(Keywords.MaxPoolSize); }
            set { SetValue(Keywords.MaxPoolSize, ToInt32(value, 0, POOL_SIZE_LIMIT, Keywords.MaxPoolSize.ToString())); CheckValues(); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Advanced")]
        [NpgsqlConnectionStringKeyword(Keywords.SyncNotification)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_SyncNotification")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_SyncNotification")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool SyncNotification
        {
            get { return (bool)GetValue(Keywords.SyncNotification); }
            set { SetValue(Keywords.SyncNotification, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Initialization")]
        [NpgsqlConnectionStringKeyword(Keywords.CommandTimeout)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_CommandTimeout")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_CommandTimeout")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(20)]
        public int CommandTimeout
        {
            get { return (int)GetValue(Keywords.CommandTimeout); }
            set { SetValue(Keywords.CommandTimeout, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Pooling")]
        [NpgsqlConnectionStringKeyword(Keywords.Enlist)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Enlist")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Enlist")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(true)]
        public bool Enlist
        {
            get { return (bool)GetValue(Keywords.Enlist); }
            set { SetValue(Keywords.Enlist, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Advanced")]
        [NpgsqlConnectionStringKeyword(Keywords.PreloadReader)]
        [NpgsqlConnectionStringAcceptableKeyword("PRELOAD READER")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_PreloadReader")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_PreloadReader")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(true)]
        public bool PreloadReader
        {
            get { return (bool)GetValue(Keywords.PreloadReader); }
            set { SetValue(Keywords.PreloadReader, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Advanced")]
        [NpgsqlConnectionStringKeyword(Keywords.UseExtendedTypes)]
        [NpgsqlConnectionStringAcceptableKeyword("USE EXTENDED TYPES")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_UseExtendedTypes")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_UseExtendedTypes")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(true)]
        public bool UseExtendedTypes
        {
            get { return (bool)GetValue(Keywords.UseExtendedTypes); }
            set { SetValue(Keywords.UseExtendedTypes, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Security")]
        [NpgsqlConnectionStringKeyword(Keywords.IntegratedSecurity)]
        [NpgsqlConnectionStringAcceptableKeyword("INTEGRATED SECURITY")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_IntegratedSecurity")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_IntegratedSecurity")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool IntegratedSecurity
        {
            get { return (bool)GetValue(Keywords.IntegratedSecurity); }
            set { SetValue(Keywords.IntegratedSecurity, value); }
        }

        private static readonly Version THIS_VERSION =
            MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Version;

        /// <summary>
        /// Compatibilty version. When possible, behaviour caused by breaking changes will be preserved
        /// if this version is less than that where the breaking change was introduced.
        /// </summary>
        [Browsable(false)]
        internal Version Compatible
        {
            get { return new Version(CompatibleAsString); }
            set { CompatibleAsString = value.ToString(); }
        }
        
        [NpgsqlConnectionStringCategory("DataCategory_Advanced")]
        [NpgsqlConnectionStringKeyword(Keywords.Compatible)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Compatible")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Compatible")]
        [RefreshProperties(RefreshProperties.All)]
        [ReadOnly(true)]
        internal string CompatibleAsString
        {
            get { return (string)base[underlying_keywords[Keywords.Compatible]]; }
            set { SetValue(Keywords.Compatible, new Version(value).ToString()); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Context")]
        [NpgsqlConnectionStringKeyword(Keywords.ApplicationName)]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_ApplicationName")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_ApplicationName")]
        [RefreshProperties(RefreshProperties.All)]
        public string ApplicationName
        {
            get { return (string)GetValue(Keywords.ApplicationName); }
            set { SetValue(Keywords.ApplicationName, value); }
        }

        #endregion

        internal static object GetDefaultValue(Keywords keyword)
        {
            object obj;
            defaults.TryGetValue(keyword, out obj);
            return obj;
        }

        /// <summary>
        /// Case insensative accessor for indivual connection string values.
        /// </summary>
        public override object this[string keyword]
        {
            get
            {
                if (!keyword_mappings.ContainsKey(keyword.ToUpperInvariant()))
                    throw new ArgumentException(resman.GetString("Exception_WrongKeyVal"), keyword);
                return this[keyword_mappings[keyword.ToUpperInvariant()]];
            }
            set
            {
                if (!keyword_mappings.ContainsKey(keyword.ToUpperInvariant()))
                    throw new ArgumentException(resman.GetString("Exception_WrongKeyVal"), keyword);
                this[keyword_mappings[keyword.ToUpperInvariant()]] = value;
            }
        }

        public object this[Keywords keyword]
        {
            get { return props[keyword].GetValue(this, null); }
            set
            {
                try
                {
                    if (props[keyword].PropertyType == value.GetType())
                        props[keyword].SetValue(this, value, null);
                    else
                        props[keyword].SetValue(this, TypeDescriptor.GetConverter(props[keyword].PropertyType).ConvertFrom(value), null);
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
        }

        public override bool Remove(string keyword)
        {
            Keywords k;
            if (keyword_mappings.TryGetValue(keyword.ToUpperInvariant(), out k))
                return Remove(k);
            return false;
        }

        public bool Remove(Keywords keyword)
        {
            return base.Remove(underlying_keywords[keyword]);
        }

        public override bool ContainsKey(string keyword)
        {
            if (keyword_mappings.ContainsKey(keyword.ToUpperInvariant()))
                return ContainsKey(keyword_mappings[keyword.ToUpperInvariant()]);
            return false;
        }

        public bool ContainsKey(Keywords keyword)
        {
            return base.ContainsKey(underlying_keywords[keyword]);
        }

        public override bool TryGetValue(string keyword, out object value)
        {
            value = null;
            if (keyword_mappings.ContainsKey(keyword.ToUpperInvariant()))
                return base.TryGetValue(underlying_keywords[keyword_mappings[keyword.ToUpperInvariant()]], out value);
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="value"></param>
        private void SetValue(Keywords keyword, object value)
        {
            string key_name = underlying_keywords[keyword];
            try
            {
                string value2 = value as string;
                if (value2 != null)
                {
                    value2 = value2.Trim();
                    if (String.IsNullOrEmpty(value2))
                        value = null;
                    else
                        value = value2;
                }
                base[key_name] = value;
            }
            catch (InvalidCastException exception)
            {
                string exception_template = string.Empty;

                switch (keyword)
                {
                    case Keywords.Port:
                    case Keywords.Timeout:
                    case Keywords.ConnectionLifeTime:
                    case Keywords.MinPoolSize:
                    case Keywords.MaxPoolSize:
                    case Keywords.CommandTimeout:
                        exception_template = resman.GetString("Exception_InvalidIntegerKeyVal");
                        break;
                    case Keywords.SSL:
                    case Keywords.Pooling:
                    case Keywords.SyncNotification:
                        exception_template = resman.GetString("Exception_InvalidBooleanKeyVal");
                        break;
                    case Keywords.Protocol:
                        exception_template = resman.GetString("Exception_InvalidProtocolVersionKeyVal");
                        break;
                }

                if (!string.IsNullOrEmpty(exception_template))
                {
                    throw new ArgumentException(string.Format(exception_template, key_name), key_name, exception);
                }

                throw;
            }
        }

        private object GetValue(Keywords keyword)
        {
            return TypeDescriptor.GetConverter(props[keyword].PropertyType).ConvertFrom(base[underlying_keywords[keyword]]);
        }

        /// <summary>
        /// Clear the member and assign them to the default value.
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            foreach (KeyValuePair<Keywords, object> pair in defaults)
                this[pair.Key] = pair.Value;
        }
    }

    public enum Keywords
    {
        Host,
        Port,
        Protocol,
        Database,
        UserName,
        Password,
        SSL,
        SslMode,
        [Obsolete("UTF-8 is always used regardless of this setting.")] Encoding,
        Timeout,
        SearchPath,
        //    These are for the connection pool
        Pooling,
        ConnectionLifeTime,
        MinPoolSize,
        MaxPoolSize,
        SyncNotification,
        // These are for the command
        CommandTimeout,
        // These are for the resource manager
        Enlist,
        PreloadReader,
        UseExtendedTypes,
        IntegratedSecurity,
        Compatible,
        ApplicationName
    }

    public enum SslMode
    {
        Disable = 1 << 0,
        Allow = 1 << 1,
        Prefer = 1 << 2,
        Require = 1 << 3
    }
}
