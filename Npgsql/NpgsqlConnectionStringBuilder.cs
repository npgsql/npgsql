// created on 29/11/2007

// Npgsql.NpgsqlConnectionStringBuilder.cs
//
// Author:
//    Glen Parker (glenebob@gmail.com)
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
using System.Diagnostics;
using System.DirectoryServices;
using System.Reflection;
using System.Resources;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
using Npgsql.Localization;

namespace Npgsql
{
// Keep the xml comment warning quiet for this file.
#pragma warning disable 1591

    /// <summary>
    /// Npgsql specific connection string builder 
    /// </summary>
    public sealed class NpgsqlConnectionStringBuilder : DbConnectionStringBuilder
    {
        #region Delegates
        private delegate string ValueNativeToString(object value);
        #endregion

        #region private classes
        /// <summary>
        /// Class contains a cached obtained UserPrincipalName (UPN), which is the name associated with the current thread's associated user.
        /// </summary>
        private class CachedUpn
        {
            public string Upn { get; set;}
            public DateTime ExpiryTimeUtc { get; set;}
        }


        /// <summary>
        /// Derived category attribute which supports localized categories.
        /// </summary>
        private sealed class NpgsqlConnectionStringCategoryAttribute : CategoryAttribute
        {
            public NpgsqlConnectionStringCategoryAttribute(String category) : base(category) { }

            protected override string GetLocalizedString(string value)
            {
                return _resman.GetString(value);
            }
        }

        /// <summary>
        /// Derived DisplayName attribute which supports localized display names
        /// </summary>
        private sealed class NpgsqlConnectionStringDisplayNameAttribute : DisplayNameAttribute
        {
            public NpgsqlConnectionStringDisplayNameAttribute(string resourceName)
                : base(resourceName)
            {
                try
                {
                    string value = _resman.GetString(resourceName);
                    if(value != null)
                    {
                        DisplayNameValue = value;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Derived Description attribute which supports localized descriptions
        /// </summary>
        private sealed class NpgsqlConnectionStringDescriptionAttribute : DescriptionAttribute
        {
            public NpgsqlConnectionStringDescriptionAttribute(string resourceName)
                : base(resourceName)
            {
                try
                {
                    string value = _resman.GetString(resourceName);
                    if(value != null)
                    {
                        DescriptionValue = value;
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Class which stores a password as a byte[].  On update, first wipe the old password value. 
        /// </summary>
        private class PasswordBytes
        {
            #region Members
            private byte[] _password = new byte[0];
            #endregion

            /// <summary>
            /// Gets or sets the password contained by this instance as a byte array.
            /// </summary>
            internal byte[] PasswordAsByteArray
            {
                set
                {
                    Wipe();
                    _password = new byte[value.Length];
                    value.CopyTo(_password, 0);
                }

                get { return _password; }
            }

            /// <summary>
            /// Gets or sets the password in string form contained by this instance.
            /// </summary>
            internal string Password
            {
                set
                {
                    Wipe();
                    _password = BackendEncoding.UTF8Encoding.GetBytes(value);
                }

                get { return BackendEncoding.UTF8Encoding.GetString(_password); }
            }

            /// <summary>
            /// Wipes the password bytes set
            /// </summary>
            private void Wipe()
            {
                for(int i = 0; i < _password.Length; i++)
                {
                    _password[i] = 0;
                }
            }
        }


        /// <summary>
        /// Class which defines a value description for a connection string value. All connection string values have an implicit default (its type's default value),
        /// and an explicit default which can be different from its implicit default.
        /// </summary>
        private class ValueDescription
        {
            #region Members
            private readonly IComparable _implicitDefault;
            private readonly ValueNativeToString _nativeToString;
            #endregion

            /// <summary>
            /// Set both ImplicitDefault and ExplicitDefault to the <paramref name="t"/>'s default value.
            /// </summary>
            /// <param name="t"></param>
            /// <param name="storeInBase"></param>
            /// <param name="nativeToString"></param>
            internal ValueDescription(Type t, bool storeInBase = true, ValueNativeToString nativeToString = null)
            {
                _implicitDefault = GetImplicitDefault(t);
                this.ExplicitDefault = _implicitDefault;
                this.DefaultsDiffer = false;
                this.StoreInBase = storeInBase;
                _nativeToString = nativeToString;
            }

            /// <summary>
            /// Set ImplicitDefault to the default value of <paramref name="explicitDefault"/>'s type,
            /// and ExplicitDefault to <paramref name="explicitDefault"/>.
            /// </summary>
            /// <param name="explicitDefault"></param>
            /// <param name="storeInBase"></param>
            /// <param name="nativeToString"></param>
            internal ValueDescription(IComparable explicitDefault, bool storeInBase = true, ValueNativeToString nativeToString = null)
            {
                _implicitDefault = GetImplicitDefault(explicitDefault.GetType());
                this.ExplicitDefault = explicitDefault;
                this.DefaultsDiffer = (_implicitDefault.CompareTo(ExplicitDefault) != 0);
                this.StoreInBase = storeInBase;
                _nativeToString = nativeToString;
            }

            private static IComparable GetImplicitDefault(Type t)
            {
                return (t == typeof(string)) ? string.Empty: (IComparable)Activator.CreateInstance(t);
            }

            internal string ConvertNativeToString(object value)
            {
                string asString = value as string;
                if(asString != null)
                {
                    return asString.Trim();
                }
                else if(_nativeToString != null)
                {
                    return _nativeToString(value);
                }
                else
                {
                    return value.ToString();
                }
            }

            #region Properties
            internal IComparable ExplicitDefault { get; private set;}
            internal bool DefaultsDiffer { get; private set; }
            internal bool StoreInBase { get; private set;}
            #endregion
        }
        #endregion

        #region Statics
        private static readonly ResourceManager _resman = new ResourceManager("Npgsql.Localization.L10N", typeof(L10N).Assembly);
        private static readonly Dictionary<Keywords, ValueDescription> _valueDescriptions = new Dictionary<Keywords, ValueDescription>();
        private static readonly Version THIS_VERSION = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Version;
        private static Dictionary<SecurityIdentifier, CachedUpn> _cachedUpns = new Dictionary<SecurityIdentifier, CachedUpn>();

        static NpgsqlConnectionStringBuilder()
        {
            // Set up value descriptions.
            // All connection string values have an implicit default (its type's default value),
            // and an explicit default which can be different from its implicit default.
            _valueDescriptions.Add(Keywords.Host, new ValueDescription(typeof(string)));
            _valueDescriptions.Add(Keywords.Port, new ValueDescription((Int32)5432));
            _valueDescriptions.Add(Keywords.Database, new ValueDescription(typeof(string)));
            _valueDescriptions.Add(Keywords.UserName, new ValueDescription(typeof(string)));
            _valueDescriptions.Add(Keywords.Password, new ValueDescription(typeof(string)));
            _valueDescriptions.Add(Keywords.SSL, new ValueDescription(typeof(bool)));
            _valueDescriptions.Add(Keywords.SslMode, new ValueDescription(typeof(SslMode)));
            _valueDescriptions.Add(Keywords.Timeout, new ValueDescription((Int32)15));
            _valueDescriptions.Add(Keywords.SearchPath, new ValueDescription(typeof(string)));
            _valueDescriptions.Add(Keywords.Pooling, new ValueDescription(true));
            _valueDescriptions.Add(Keywords.ConnectionLifeTime, new ValueDescription(typeof(Int32)));
            _valueDescriptions.Add(Keywords.MinPoolSize, new ValueDescription((Int32)1));
            _valueDescriptions.Add(Keywords.MaxPoolSize, new ValueDescription((Int32)20));
            _valueDescriptions.Add(Keywords.SyncNotification, new ValueDescription(typeof(bool)));
            _valueDescriptions.Add(Keywords.CommandTimeout, new ValueDescription(NpgsqlCommand.DefaultTimeout));
            _valueDescriptions.Add(Keywords.Enlist, new ValueDescription(typeof(bool)));
            _valueDescriptions.Add(Keywords.PreloadReader, new ValueDescription(typeof(bool)));
            _valueDescriptions.Add(Keywords.UseExtendedTypes, new ValueDescription(typeof(bool)));
            _valueDescriptions.Add(Keywords.IntegratedSecurity, new ValueDescription(typeof(bool)));
            _valueDescriptions.Add(Keywords.IncludeRealm, new ValueDescription(typeof(bool)));
            _valueDescriptions.Add(Keywords.Compatible, new ValueDescription(THIS_VERSION));
            _valueDescriptions.Add(Keywords.ApplicationName, new ValueDescription(typeof(string)));
            _valueDescriptions.Add(Keywords.AlwaysPrepare, new ValueDescription(typeof(bool)));
        }
        #endregion

        #region Constants
        private const int POOL_SIZE_LIMIT = 1024;
        private const int TIMEOUT_LIMIT = 1024;
        #endregion

        #region Members
        private string _originalConnectionString, _host, _database, _username, _searchpath, _applicationName;
        private PasswordBytes _password;
        private int _port, _timeout, _connectionLifeTime, _minPoolSize, _maxPoolSize, _commandTimeout;
        private bool _ssl, _pooling, _syncNotification, _enlist, _integratedSecurity, _includeRealm, _always_prepare, _useExtendedTypes;
        private Version _compatible;
        private SslMode _sslmode;
        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlConnectionStringBuilder"/> class.
        /// </summary>
        public NpgsqlConnectionStringBuilder()
        {
            _password = new PasswordBytes();
            this.Clear();
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="NpgsqlConnectionStringBuilder"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public NpgsqlConnectionStringBuilder(string connectionString)
        {
            _password = new PasswordBytes();
            this._originalConnectionString = connectionString;
            this.ConnectionString = connectionString;
            CheckValues();
        }
        

        /// <summary>
        /// Return an exact copy of this NpgsqlConnectionString.
        /// </summary>
        public NpgsqlConnectionStringBuilder Clone()
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder();

            foreach(string key in this.Keys)
            {
                builder[key] = this[key];
            }
            return builder;
        }


        /// <summary>
        /// Removes the entry with the specified key from the <see cref="T:System.Data.Common.DbConnectionStringBuilder" /> instance.
        /// </summary>
        /// <param name="keyword">The key of the key/value pair to be removed from the connection string in this <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.</param>
        /// <returns>
        /// true if the key existed within the connection string and was removed; false if the key did not exist.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        /// </PermissionSet>
        public override bool Remove(string keyword)
        {
            Keywords key = GetKey(keyword);
            SetValue(key, GetDefaultValue(key));
            return base.Remove(keyword);
        }


        /// <summary>
        /// Determines whether this instance contains a key with the name associated with the keyword specified
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        public bool ContainsKey(Keywords keyword)
        {
            return this.ContainsKey(GetKeyName(keyword));
        }


        /// <summary>
        /// Retrieves a value corresponding to the supplied key from this <see cref="T:System.Data.Common.DbConnectionStringBuilder" />.
        /// </summary>
        /// <param name="keyword">The key of the item to retrieve.</param>
        /// <param name="value">The value corresponding to the <paramref name="keyword" />.</param>
        /// <returns>
        /// true if <paramref name="keyword" /> was found within the connection string, false otherwise.
        /// </returns>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" PathDiscovery="*AllFiles*" />
        /// </PermissionSet>
        public override bool TryGetValue(string keyword, out object value)
        {
            try
            {
                value = GetValue(GetKey(keyword));
                return true;
            }
            catch(ArgumentException)
            {
                value = null;
                return false;
            }
        }


        /// <summary>
        /// Clear the member and assign them to the default value.
        /// </summary>
        public override void Clear()
        {
            base.Clear();

            foreach(KeyValuePair<Keywords, ValueDescription> kvp in _valueDescriptions)
            {
                if(kvp.Value.StoreInBase)
                {
                    SetValue(GetKeyName(kvp.Key), kvp.Key, kvp.Value.ExplicitDefault);
                }
            }
        }



        /// <summary>
        /// Gets the name of the key to use for the keyword specified
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        internal static string GetKeyName(Keywords keyword)
        {
            switch(keyword)
            {
                case Keywords.Host:
                    return "HOST";
                case Keywords.Port:
                    return "PORT";
                case Keywords.Database:
                    return "DATABASE";
                case Keywords.UserName:
                    return "USER ID";
                case Keywords.Password:
                    return "PASSWORD";
                case Keywords.SSL:
                    return "SSL";
                case Keywords.SslMode:
                    return "SSLMODE";
                case Keywords.Timeout:
                    return "TIMEOUT";
                case Keywords.SearchPath:
                    return "SEARCHPATH";
                case Keywords.Pooling:
                    return "POOLING";
                case Keywords.ConnectionLifeTime:
                    return "CONNECTIONLIFETIME";
                case Keywords.MinPoolSize:
                    return "MINPOOLSIZE";
                case Keywords.MaxPoolSize:
                    return "MAXPOOLSIZE";
                case Keywords.SyncNotification:
                    return "SYNCNOTIFICATION";
                case Keywords.CommandTimeout:
                    return "COMMANDTIMEOUT";
                case Keywords.Enlist:
                    return "ENLIST";
                case Keywords.PreloadReader:
                    return "PRELOADREADER";
                case Keywords.UseExtendedTypes:
                    return "USEEXTENDEDTYPES";
                case Keywords.IntegratedSecurity:
                    return "INTEGRATED SECURITY";
                case Keywords.IncludeRealm:
                    return "INCLUDEREALM";
                case Keywords.Compatible:
                    return "COMPATIBLE";
                case Keywords.ApplicationName:
                    return "APPLICATIONNAME";
                case Keywords.AlwaysPrepare:
                    return "ALWAYSPREPARE";
                default:
                    return keyword.ToString().ToUpperInvariant();
            }
        }


        /// <summary>
        /// Gets the default value for the keyword specified.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        internal static object GetDefaultValue(Keywords keyword)
        {
            return _valueDescriptions[keyword].ExplicitDefault;
        }


        /// <summary>
        /// This function will set value for known key, both private member and base[key].
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>value, coerced as needed to the stored type.</returns>
        private object SetValue(string keyword, Keywords key, object value)
        {
            ValueDescription description = _valueDescriptions[key];

            if(!description.StoreInBase)
            {
                return value;
            }
            if(value == null)
            {
                Remove(keyword);
                return value;
            }

            value = SetValue(key, value);

            // If the value matches both the parameter's default and the type's default, remove it from base,
            // otherwise convert to string and set the base value.
            if(description.DefaultsDiffer || (description.ExplicitDefault.CompareTo((IComparable)value) != 0))
            {
                base[keyword] = description.ConvertNativeToString(value);
            }
            else
            {
                base.Remove(keyword);
            }
            return value;
        }


        /// <summary>
        /// The function will modify private member only, not base[key].
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="value"></param>
        /// <returns>value, coerced as needed to the stored type.</returns>
        private object SetValue(Keywords keyword, object value)
        {
            try
            {
                switch(keyword)
                {
                    case Keywords.Host:
                        return this._host = Convert.ToString(value);
                    case Keywords.Port:
                        return this._port = Convert.ToInt32(value);
                    case Keywords.Database:
                        return this._database = Convert.ToString(value);
                    case Keywords.UserName:
                        return this._username = Convert.ToString(value);
                    case Keywords.Password:
                        this._password.Password = value as string;
                        return value as string;
                    case Keywords.SSL:
                        return this._ssl = ToBoolean(value);
                    case Keywords.SslMode:
                        return this._sslmode = ToSslMode(value);
                    case Keywords.Timeout:
                        return this._timeout = ToInt32(value, 0, TIMEOUT_LIMIT, keyword);
                    case Keywords.SearchPath:
                        return this._searchpath = Convert.ToString(value);
                    case Keywords.Pooling:
                        return this._pooling = ToBoolean(value);
                    case Keywords.ConnectionLifeTime:
                        return this._connectionLifeTime = Convert.ToInt32(value);
                    case Keywords.MinPoolSize:
                        return this._minPoolSize = ToInt32(value, 0, POOL_SIZE_LIMIT, keyword);
                    case Keywords.MaxPoolSize:
                        return this._maxPoolSize = ToInt32(value, 0, POOL_SIZE_LIMIT, keyword);
                    case Keywords.SyncNotification:
                        return this._syncNotification = ToBoolean(value);
                    case Keywords.CommandTimeout:
                        return this._commandTimeout = Convert.ToInt32(value);
                    case Keywords.Enlist:
                        return this._enlist = ToBoolean(value);
                    case Keywords.PreloadReader:
                        if(ToBoolean(value))
                        {
                            throw new NotSupportedException("Support for the preload reader has been removed in Npgsql 3.0. Please see https://github.com/npgsql/Npgsql/wiki/Preload-Removal");
                        }
                        return false;
                    case Keywords.UseExtendedTypes:
                        return this._useExtendedTypes = ToBoolean(value);
                    case Keywords.IntegratedSecurity:
                        bool iS = ToIntegratedSecurity(value);
                        if(iS == true)
                        {
                            CheckIntegratedSecuritySupport();
                        }
                        return this._integratedSecurity = ToIntegratedSecurity(iS);
                    case Keywords.IncludeRealm:
                        return this._includeRealm = ToBoolean(value);
                    case Keywords.Compatible:
                        Version ver = new Version(value.ToString());
                        if(ver > THIS_VERSION)
                        {
                            throw new ArgumentException("Attempt to set compatibility with version " + value +
                                                        " when using version " + THIS_VERSION);
                        }
                        return _compatible = ver;
                    case Keywords.ApplicationName:
                        return this._applicationName = Convert.ToString(value);
                    case Keywords.AlwaysPrepare:
                        return this._always_prepare = Convert.ToBoolean(value);
                }
            }
            catch(InvalidCastException ex)
            {
                string exception_template = string.Empty;

                switch(keyword)
                {
                    case Keywords.Port:
                    case Keywords.Timeout:
                    case Keywords.ConnectionLifeTime:
                    case Keywords.MinPoolSize:
                    case Keywords.MaxPoolSize:
                    case Keywords.CommandTimeout:
                        exception_template = L10N.InvalidIntegerKeyVal;
                        break;
                    case Keywords.SSL:
                    case Keywords.Pooling:
                    case Keywords.SyncNotification:
                        exception_template = L10N.InvalidBooleanKeyVal;
                        break;
                }

                if(!string.IsNullOrEmpty(exception_template))
                {
                    string key_name = GetKeyName(keyword);

                    throw new ArgumentException(string.Format(exception_template, key_name), key_name, ex);
                }
                throw;
            }

            return null;
        }

        /// <summary>
        /// The function will access private member only, not base[key].
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns>value.</returns>
        private object GetValue(Keywords keyword)
        {
            switch(keyword)
            {
                case Keywords.Host:
                    return this._host;
                case Keywords.Port:
                    return this._port;
                case Keywords.Database:
                    return this._database;
                case Keywords.UserName:
                    return this._username;
                case Keywords.Password:
                    return this._password.Password;
                case Keywords.SSL:
                    return this._ssl;
                case Keywords.SslMode:
                    return this._sslmode;
                case Keywords.Timeout:
                    return this._timeout;
                case Keywords.SearchPath:
                    return this._searchpath;
                case Keywords.Pooling:
                    return this._pooling;
                case Keywords.ConnectionLifeTime:
                    return this._connectionLifeTime;
                case Keywords.MinPoolSize:
                    return this._minPoolSize;
                case Keywords.MaxPoolSize:
                    return this._maxPoolSize;
                case Keywords.SyncNotification:
                    return this._syncNotification;
                case Keywords.CommandTimeout:
                    return this._commandTimeout;
                case Keywords.Enlist:
                    return this._enlist;
                case Keywords.UseExtendedTypes:
                    return this._useExtendedTypes;
                case Keywords.IntegratedSecurity:
                    return this._integratedSecurity;
                case Keywords.IncludeRealm:
                    return this._includeRealm;
                case Keywords.Compatible:
                    return _compatible;
                case Keywords.ApplicationName:
                    return this._applicationName;
                case Keywords.AlwaysPrepare:
                    return this._always_prepare;
                default:
                    return null;

            }
        }


        /// <summary>
        /// Checks the values of this instance.
        /// </summary>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        private void CheckValues()
        {
            if((MaxPoolSize > 0) && (MinPoolSize > MaxPoolSize))
            {
                string key = GetKeyName(Keywords.MinPoolSize);
                throw new ArgumentOutOfRangeException(key, String.Format(L10N.IntegerKeyValMax, key, MaxPoolSize));
            }
            if(Compatible != THIS_VERSION)
            {
                throw new NotSupportedException("No compatibility modes supported in this Npgsql version");
            }
            if(IntegratedSecurity && Type.GetType("Mono.Runtime") != null)
            {
                throw new NotSupportedException("IntegratedSecurity isn't supported on mono");
            }
        }


        /// <summary>
        /// Converts the specified value to an SslMode value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static SslMode ToSslMode(object value)
        {
            if(value is SslMode)
            {
                return (SslMode)value;
            }
            else
            {
                return (SslMode)Enum.Parse(typeof(SslMode), value.ToString(), true);
            }
        }


        /// <summary>
        /// Converts the specified value to an int32 within the specified boundaries defined for the keyword specified.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The minimum.</param>
        /// <param name="max">The maximum.</param>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException">if the value is outside the specified boundaries</exception>
        private static int ToInt32(object value, int min, int max, Keywords keyword)
        {
            int v = Convert.ToInt32(value);
            if(v < min)
            {
                string key = GetKeyName(keyword);
                throw new ArgumentOutOfRangeException(key, String.Format(L10N.IntegerKeyValMin, key, min));
            }
            else if(v > max)
            {
                string key = GetKeyName(keyword);
                throw new ArgumentOutOfRangeException(key, String.Format(L10N.IntegerKeyValMax, key, max));
            }
            return v;
        }


        /// <summary>
        /// Converts the specified value to a boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException"></exception>
        private static Boolean ToBoolean(object value)
        {
            string text = value as string;
            if(text != null)
            {
                switch(text.ToLowerInvariant())
                {
                    case "t":
                    case "true":
                    case "y":
                    case "yes":
                        return true;
                    case "f":
                    case "false":
                    case "n":
                    case "no":
                        return false;
                    default:
                        throw new InvalidCastException(value.ToString());
                }
            }
            else
            {
                return Convert.ToBoolean(value);
            }
        }


        /// <summary>
        /// Converts the value specified to a boolean value which signals whether Integrated Security is to be used (true) or not (false)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException"></exception>
        private Boolean ToIntegratedSecurity(object value)
        {
            string text = value as string;
            if(text != null)
            {
                switch(text.ToLowerInvariant())
                {
                    case "t":
                    case "true":
                    case "y":
                    case "yes":
                    case "sspi":
                        return true;
                    case "f":
                    case "false":
                    case "n":
                    case "no":
                        return false;

                    default:
                        throw new InvalidCastException(value.ToString());
                }
            }
            else
            {
                return Convert.ToBoolean(value);
            }
        }


        /// <summary>
        /// Gets the key associated with the keyword which is specified as string.
        /// </summary>
        /// <param name="keyWord">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        private static Keywords GetKey(string keyWord)
        {
            switch(keyWord.ToUpperInvariant())
            {
                case "HOST":
                case "SERVER":
                    return Keywords.Host;
                case "PORT":
                    return Keywords.Port;
                case "DATABASE":
                case "DB":
                    return Keywords.Database;
                case "USERNAME":
                case "USER NAME":
                case "USER":
                case "USERID":
                case "USER ID":
                case "UID":
                    return Keywords.UserName;
                case "PASSWORD":
                case "PSW":
                case "PWD":
                    return Keywords.Password;
                case "SSL":
                    return Keywords.SSL;
                case "SSLMODE":
                    return Keywords.SslMode;
                case "TIMEOUT":
                    return Keywords.Timeout;
                case "SEARCHPATH":
                    return Keywords.SearchPath;
                case "POOLING":
                    return Keywords.Pooling;
                case "CONNECTIONLIFETIME":
                    return Keywords.ConnectionLifeTime;
                case "MINPOOLSIZE":
                    return Keywords.MinPoolSize;
                case "MAXPOOLSIZE":
                    return Keywords.MaxPoolSize;
                case "SYNCNOTIFICATION":
                    return Keywords.SyncNotification;
                case "COMMANDTIMEOUT":
                    return Keywords.CommandTimeout;
                case "ENLIST":
                    return Keywords.Enlist;
                case "PRELOADREADER":
                case "PRELOAD READER":
                    return Keywords.PreloadReader;
                case "USEEXTENDEDTYPES":
                case "USE EXTENDED TYPES":
                    return Keywords.UseExtendedTypes;
                case "INTEGRATEDSECURITY":
                case "INTEGRATED SECURITY":
                    return Keywords.IntegratedSecurity;
                case "INCLUDEREALM":
                    return Keywords.IncludeRealm;
                case "COMPATIBLE":
                    return Keywords.Compatible;
                case "APPLICATIONNAME":
                    return Keywords.ApplicationName;
                case "ALWAYSPREPARE":
                    return Keywords.AlwaysPrepare;
                default:
                    throw new ArgumentException(_resman.GetString("Exception_WrongKeyVal"), keyWord);
            }
        }


        /// <summary>
        /// Gets the current user's username for integrated security purposes
        /// </summary>
        /// <returns></returns>
        private string GetIntegratedUserName()
        {
            // Side note: This maintains the hack fix mentioned before for https://github.com/npgsql/Npgsql/issues/133.
            // In a nutshell, starting with .NET 4.5 WindowsIdentity inherits from ClaimsIdentity
            // which doesn't exist in mono, and calling a WindowsIdentity method bombs.
            // The workaround is that this function that actually deals with WindowsIdentity never
            // gets called on mono, so never gets JITted and the problem goes away.

            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            CachedUpn cachedUpn = null;
            string upn = null;

            // Check to see if we already have this UPN cached
            lock(_cachedUpns)
            {
                if(_cachedUpns.TryGetValue(identity.User, out cachedUpn))
                {
                    if(cachedUpn.ExpiryTimeUtc > DateTime.UtcNow)
                    {
                        upn = cachedUpn.Upn;
                    }
                    else
                    {
                        _cachedUpns.Remove(identity.User);
                    }
                }
            }

            try
            {
                if(upn == null)
                {
                    // Try to get the user's UPN in its correct case; this is what the
                    // server will need to verify against a Kerberos/SSPI ticket

                    // First, find a domain server we can talk to
                    string domainHostName;
                    using(DirectoryEntry rootDse = new DirectoryEntry("LDAP://rootDSE") { AuthenticationType = AuthenticationTypes.Secure })
                    {
                        domainHostName = (string)rootDse.Properties["dnsHostName"].Value;
                    }

                    // Query the domain server by the current user's SID
                    using(DirectoryEntry entry = new DirectoryEntry("LDAP://" + domainHostName) { AuthenticationType = AuthenticationTypes.Secure })
                    {
                        DirectorySearcher search = new DirectorySearcher(entry,
                            "(objectSid=" + identity.User.Value + ")", new string[] { "userPrincipalName" });

                        SearchResult result = search.FindOne();
                        upn = (string)result.Properties["userPrincipalName"][0];
                    }
                }

                if(cachedUpn == null)
                {
                    // Save this value
                    cachedUpn = new CachedUpn() { Upn = upn, ExpiryTimeUtc = DateTime.UtcNow.AddHours(3.0) };
                    lock(_cachedUpns)
                    {
                        _cachedUpns[identity.User] = cachedUpn;
                    }
                }

                string[] upnParts = upn.Split('@');
                if(_includeRealm)
                {
                    // Make it Kerberos-y by uppercasing the realm part
                    return upnParts[0] + "@" + upnParts[1].ToUpperInvariant();
                }
                else
                {
                    return upnParts[0];
                }
            }
            catch
            {
                // Querying the directory failed, so return the SAM name
                // (which probably won't work, but it's better than nothing)
                return identity.Name.Split('\\')[1];
            }
        }

        #region Properties
        /// <summary>
        /// Case insensative accessor for indivual connection string values.
        /// </summary>
        public override object this[string keyword]
        {
            get { return GetValue(GetKey(keyword)); }
            set { this[GetKey(keyword)] = value; }
        }

        /// <summary>
        /// Gets or sets the value with the specified keyword.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns></returns>
        public object this[Keywords keyword]
        {
            get { return GetValue(keyword); }
            set { SetValue(GetKeyName(keyword), keyword, value); }
        }

        /// <summary>
        /// Gets or sets the backend server host name.
        /// </summary>
        [NpgsqlConnectionStringCategory("DataCategory_Source")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Host")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Host")]
        [RefreshProperties(RefreshProperties.All)]
        public string Host
        {
            get { return _host; }
            set { SetValue(GetKeyName(Keywords.Host), Keywords.Host, value); }
        }

        /// <summary>
        /// Gets or sets the backend server port.
        /// </summary>
        [NpgsqlConnectionStringCategory("DataCategory_Source")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Port")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Port")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(5432)]
        public int Port
        {
            get { return _port; }
            set { SetValue(GetKeyName(Keywords.Port), Keywords.Port, value); }
        }

        ///<summary>
        /// Gets or sets the name of the database to be used after a connection is opened.
        /// </summary>
        /// <value>The name of the database to be
        /// used after a connection is opened.</value>
        [NpgsqlConnectionStringCategory("DataCategory_Source")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Database")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Database")]
        [RefreshProperties(RefreshProperties.All)]
        public string Database
        {
            get { return _database; }
            set { SetValue(GetKeyName(Keywords.Database), Keywords.Database, value); }
        }

        /// <summary>
        /// Gets or sets the login user name.
        /// </summary>
        [NpgsqlConnectionStringCategory("DataCategory_Security")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_UserName")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_UserName")]
        [RefreshProperties(RefreshProperties.All)]
        public string UserName
        {
            get
            {
                if((_integratedSecurity) && (String.IsNullOrEmpty(_username)))
                {
                    _username = GetIntegratedUserName();
                }

                return _username;
            }
            set { SetValue(GetKeyName(Keywords.UserName), Keywords.UserName, value); }
        }

        /// <summary>
        /// Gets or sets the login password as a UTF8 encoded byte array.
        /// </summary>
        [Browsable(false)]
        public byte[] PasswordAsByteArray
        {
            get { return _password.PasswordAsByteArray; }
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException("PasswordAsByteArray");
                }
                _password.PasswordAsByteArray = value;
            }
        }

        /// <summary>
        /// Sets the login password as a string.
        /// </summary>
        [NpgsqlConnectionStringCategory("DataCategory_Security")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Password")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Password")]
        [RefreshProperties(RefreshProperties.All)]
        [PasswordPropertyText(true)]
        public string Password
        {
            get { return String.Empty; }
            set { SetValue(GetKeyName(Keywords.Password), Keywords.Password, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to attempt to use SSL.
        /// </summary>
        [NpgsqlConnectionStringCategory("DataCategory_Advanced")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_SSL")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_SSL")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool SSL
        {
            get { return _ssl; }
            set { SetValue(GetKeyName(Keywords.SSL), Keywords.SSL, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to attempt to use SSL.
        /// </summary>
        [Browsable(false)]
        public SslMode SslMode
        {
            get { return _sslmode; }
            set { SetValue(GetKeyName(Keywords.SslMode), Keywords.SslMode, value); }
        }

        /// <summary>
        /// Gets or sets the time to wait while trying to establish a connection
        /// before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for a connection to open. The default value is 15 seconds.</value>
        [NpgsqlConnectionStringCategory("DataCategory_Initialization")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Timeout")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Timeout")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(15)]
        public int Timeout
        {
            get { return _timeout; }
            set { SetValue(GetKeyName(Keywords.Timeout), Keywords.Timeout, value); }
        }

        /// <summary>
        /// Gets or sets the schema search path.
        /// </summary>
        [NpgsqlConnectionStringCategory("DataCategory_Context")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_SearchPath")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_SearchPath")]
        [RefreshProperties(RefreshProperties.All)]
        public string SearchPath
        {
            get { return _searchpath; }
            set { SetValue(GetKeyName(Keywords.SearchPath), Keywords.SearchPath, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether connection pooling should be used.
        /// </summary>
        [NpgsqlConnectionStringCategory("DataCategory_Pooling")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Pooling")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Pooling")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(true)]
        public bool Pooling
        {
            get { return _pooling; }
            set { SetValue(GetKeyName(Keywords.Pooling), Keywords.Pooling, value); }
        }

        /// <summary>
        /// Gets or sets the time to wait before closing unused connections in the pool if the count
        /// of all connections exeeds MinPoolSize.
        /// </summary>
        /// <remarks>
        /// If connection pool contains unused connections for ConnectionLifeTime seconds,
        /// the half of them will be closed. If there will be unused connections in a second
        /// later then again the half of them will be closed and so on.
        /// This strategy provide smooth change of connection count in the pool.
        /// </remarks>
        /// <value>The time (in seconds) to wait. The default value is 15 seconds.</value>
        [NpgsqlConnectionStringCategory("DataCategory_Pooling")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_ConnectionLifeTime")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_ConnectionLifeTime")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(15)]
        public int ConnectionLifeTime
        {
            get { return _connectionLifeTime; }
            set { SetValue(GetKeyName(Keywords.ConnectionLifeTime), Keywords.ConnectionLifeTime, value); }
        }

        /// <summary>
        /// Gets or sets the minimum connection pool size.
        /// </summary>
        [NpgsqlConnectionStringCategory("DataCategory_Pooling")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_MinPoolSize")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_MinPoolSize")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(1)]
        public int MinPoolSize
        {
            get { return _minPoolSize; }
            set { SetValue(GetKeyName(Keywords.MinPoolSize), Keywords.MinPoolSize, value); }
        }

        /// <summary>
        /// Gets or sets the maximum connection pool size.
        /// </summary>
        [NpgsqlConnectionStringCategory("DataCategory_Pooling")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_MaxPoolSize")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_MaxPoolSize")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(20)]
        public int MaxPoolSize
        {
            get { return _maxPoolSize; }
            set { SetValue(GetKeyName(Keywords.MaxPoolSize), Keywords.MaxPoolSize, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to listen for notifications and report them between command activity.
        /// </summary>
        [NpgsqlConnectionStringCategory("DataCategory_Advanced")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_SyncNotification")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_SyncNotification")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool SyncNotification
        {
            get { return _syncNotification; }
            set { SetValue(GetKeyName(Keywords.SyncNotification), Keywords.SyncNotification, value); }
        }

        /// <summary>
        /// Gets the time to wait while trying to execute a command
        /// before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The time (in seconds) to wait for a command to complete. The default value is 20 seconds.</value>
        [NpgsqlConnectionStringCategory("DataCategory_Initialization")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_CommandTimeout")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_CommandTimeout")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(20)]
        public int CommandTimeout
        {
            get { return _commandTimeout; }
            set { SetValue(GetKeyName(Keywords.CommandTimeout), Keywords.CommandTimeout, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Pooling")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_Enlist")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_Enlist")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(true)]
        public bool Enlist
        {
            get { return _enlist; }
            set { SetValue(GetKeyName(Keywords.Enlist), Keywords.Enlist, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Advanced")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_UseExtendedTypes")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_UseExtendedTypes")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(true)]
        public bool UseExtendedTypes
        {
            get { return _useExtendedTypes; }
            set { SetValue(GetKeyName(Keywords.UseExtendedTypes), Keywords.UseExtendedTypes, value); }
        }

        [NpgsqlConnectionStringCategory("DataCategory_Security")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_IntegratedSecurity")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_IntegratedSecurity")]
        [RefreshProperties(RefreshProperties.All)]
        [DefaultValue(false)]
        public bool IntegratedSecurity
        {
            get { return _integratedSecurity; }
            set
            {
                if(value == true)
                    CheckIntegratedSecuritySupport();
                SetValue(GetKeyName(Keywords.IntegratedSecurity), Keywords.IntegratedSecurity, value);
            }
        }

        /// <summary>
        /// No integrated security if we're on mono and .NET 4.5 because of ClaimsIdentity,
        /// see https://github.com/npgsql/Npgsql/issues/133
        /// </summary>
        [Conditional("NET45")]
        private static void CheckIntegratedSecuritySupport()
        {
            if(Type.GetType("Mono.Runtime") != null)
            {
                throw new NotSupportedException("IntegratedSecurity is currently unsupported on mono and .NET 4.5 (see https://github.com/npgsql/Npgsql/issues/133)");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the real (associated with the integrated security) should be included or not. 
        /// </summary>
        public bool IncludeRealm
        {
            get { return _includeRealm; }
            set { SetValue(GetKeyName(Keywords.IncludeRealm), Keywords.IncludeRealm, value); }
        }

        /// <summary>
        /// Compatibilty version. When possible, behaviour caused by breaking changes will be preserved
        /// if this version is less than that where the breaking change was introduced.
        /// </summary>
        [Browsable(false)]
        public Version Compatible
        {
            get { return _compatible; }
            set { SetValue(GetKeyName(Keywords.Compatible), Keywords.Compatible, value); }
        }

        /// <summary>
        /// Gets or sets the ootional application name parameter to be sent to the backend during connection initiation.
        /// </summary>
        [NpgsqlConnectionStringCategory("DataCategory_Context")]
        [NpgsqlConnectionStringDisplayName("ConnectionProperty_Display_ApplicationName")]
        [NpgsqlConnectionStringDescription("ConnectionProperty_Description_ApplicationName")]
        [RefreshProperties(RefreshProperties.All)]
        public string ApplicationName
        {
            get { return _applicationName; }
            set { SetValue(GetKeyName(Keywords.ApplicationName), Keywords.ApplicationName, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to silently Prepare() all commands before execution.
        /// </summary>
        public bool AlwaysPrepare
        {
            get { return _always_prepare; }
            set { SetValue(GetKeyName(Keywords.AlwaysPrepare), Keywords.AlwaysPrepare, value); }
        }
        #endregion
    }
#pragma warning restore 1591
}
