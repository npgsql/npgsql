﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Npgsql.Properties {
    using System;


    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class NpgsqlStrings {

        private static global::System.Resources.ResourceManager resourceMan;

        private static global::System.Globalization.CultureInfo resourceCulture;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal NpgsqlStrings() {
        }

        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Npgsql.Properties.NpgsqlStrings", typeof(NpgsqlStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }

        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to &apos;{0}&apos; must be positive..
        /// </summary>
        internal static string ArgumentMustBePositive {
            get {
                return ResourceManager.GetString("ArgumentMustBePositive", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cannot read infinity value since Npgsql.DisableDateTimeInfinityConversions is enabled..
        /// </summary>
        internal static string CannotReadInfinityValue {
            get {
                return ResourceManager.GetString("CannotReadInfinityValue", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Cannot read interval values with non-zero months as TimeSpan, since that type doesn&apos;t support months. Consider using NodaTime Period which better corresponds to PostgreSQL interval, or read the value as NpgsqlInterval, or transform the interval to not contain months or years in PostgreSQL before reading it..
        /// </summary>
        internal static string CannotReadIntervalWithMonthsAsTimeSpan {
            get {
                return ResourceManager.GetString("CannotReadIntervalWithMonthsAsTimeSpan", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to When registering a password provider, a password or password file may not be set..
        /// </summary>
        internal static string CannotSetBothPasswordProviderAndPassword {
            get {
                return ResourceManager.GetString("CannotSetBothPasswordProviderAndPassword", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to When creating a multi-host data source, TargetSessionAttributes cannot be specified. Create without TargetSessionAttributes, and then obtain DataSource wrappers from it. Consult the docs for more information..
        /// </summary>
        internal static string CannotSpecifyTargetSessionAttributes {
            get {
                return ResourceManager.GetString("CannotSpecifyTargetSessionAttributes", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to RootCertificate cannot be used in conjunction with UserCertificateValidationCallback; when registering a validation callback, perform whatever validation you require in that callback..
        /// </summary>
        internal static string CannotUseSslRootCertificateWithUserCallback {
            get {
                return ResourceManager.GetString("CannotUseSslRootCertificateWithUserCallback", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to SslMode.{0} cannot be used in conjunction with UserCertificateValidationCallback; when registering a validation callback, perform whatever validation you require in that callback..
        /// </summary>
        internal static string CannotUseSslVerifyWithUserCallback {
            get {
                return ResourceManager.GetString("CannotUseSslVerifyWithUserCallback", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ValidationRootCertificateCallback cannot be used in conjunction with UserCertificateValidationCallback; when registering a validation callback, perform whatever validation you require in that callback..
        /// </summary>
        internal static string CannotUseValidationRootCertificateCallbackWithUserCallback {
            get {
                return ResourceManager.GetString("CannotUseValidationRootCertificateCallbackWithUserCallback", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to NpgsqlSlimDataSourceBuilder is being used, and encryption hasn&apos;t been enabled, call EnableEncryption() on NpgsqlSlimDataSourceBuilder to enable it..
        /// </summary>
        internal static string EncryptionDisabled {
            get {
                return ResourceManager.GetString("EncryptionDisabled", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Full-text search isn&apos;t enabled; please call {0} on {1} to enable full-text search..
        /// </summary>
        internal static string FullTextSearchNotEnabled {
            get {
                return ResourceManager.GetString("FullTextSearchNotEnabled", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to No multirange type could be found in the database for subtype {0}..
        /// </summary>
        internal static string NoMultirangeTypeFound {
            get {
                return ResourceManager.GetString("NoMultirangeTypeFound", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Connection and transaction access is not supported on batches created from DbDataSource..
        /// </summary>
        internal static string NotSupportedOnDataSourceBatch {
            get {
                return ResourceManager.GetString("NotSupportedOnDataSourceBatch", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Connection and transaction access is not supported on commands created from DbDataSource..
        /// </summary>
        internal static string NotSupportedOnDataSourceCommand {
            get {
                return ResourceManager.GetString("NotSupportedOnDataSourceCommand", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to The right type of password provider (sync or async) was not found..
        /// </summary>
        internal static string PasswordProviderMissing {
            get {
                return ResourceManager.GetString("PasswordProviderMissing", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to When using CommandType.StoredProcedure, all positional parameters must come before named parameters..
        /// </summary>
        internal static string PositionalParameterAfterNamed {
            get {
                return ResourceManager.GetString("PositionalParameterAfterNamed", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Records aren&apos;t enabled; please call {0} on {1} to enable records..
        /// </summary>
        internal static string RecordsNotEnabled {
            get {
                return ResourceManager.GetString("RecordsNotEnabled", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Both sync and async connection initializers must be provided..
        /// </summary>
        internal static string SyncAndAsyncConnectionInitializersRequired {
            get {
                return ResourceManager.GetString("SyncAndAsyncConnectionInitializersRequired", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Ranges aren't enabled; please call {0} on {1} to enable ranges..
        /// </summary>
        internal static string RangesNotEnabled {
            get {
                return ResourceManager.GetString("RangesNotEnabled", resourceCulture);
            }
        }

        public static string RangeArraysNotEnabled
        {
            get {
                return ResourceManager.GetString("RangeArraysNotEnabled", resourceCulture);
            }
        }

        public static string MultirangeArraysNotEnabled
        {
            get {
                return ResourceManager.GetString("MultirangeArraysNotEnabled", resourceCulture);
            }
        }

        public static string MultirangesNotEnabled
        {
            get {
                return ResourceManager.GetString("MultirangesNotEnabled", resourceCulture);
            }
        }
    }
}
