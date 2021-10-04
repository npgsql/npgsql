using System;
using System.Collections.Generic;

#nullable disable
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable RS0016 // Add public types and members to the declared API
#pragma warning disable 618 // Member is obsolete

namespace Npgsql
{
    public sealed partial class NpgsqlConnectionStringBuilder
    {
        private partial int Init()
        {
            // Set the strongly-typed properties to their default values
            
            Port = 5432;
            
            Enlist = true;
            
            Encoding = "UTF8";
            
            KerberosServiceName = "postgres";
            
            Pooling = true;
            
            MinPoolSize = 0;
            
            MaxPoolSize = 100;
            
            ConnectionIdleLifetime = 300;
            
            ConnectionPruningInterval = 10;
            
            Timeout = 15;
            
            CommandTimeout = 30;
            
            InternalCommandTimeout = -1;
            
            CancellationTimeout = 2000;
            
            HostRecheckSeconds = 10;
            
            ReadBufferSize = 8192;
            
            WriteBufferSize = 8192;
            
            AutoPrepareMinUsages = 5;
            
            Multiplexing = false;
            
            WriteCoalescingBufferThresholdBytes = 1000;

            SslMode = SslMode.Prefer;

            CheckCertificateRevocation = true;
            

            // Setting the strongly-typed properties here also set the string-based properties in the base class.
            // Clear them (default settings = empty connection string)
            base.Clear();

            return 0;
        }

        private partial int GeneratedSetter(string keyword, object value)
        {
            switch (keyword)
            {
            
            case "HOST":
            
            
                Host = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "SERVER":
            
            
                Host = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "PORT":
            
            
                Port = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "DATABASE":
            
            
                Database = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "DB":
            
            
                Database = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "USERNAME":
            
            
                Username = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "USER NAME":
            
            
                Username = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "USERID":
            
            
                Username = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "USER ID":
            
            
                Username = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "UID":
            
            
                Username = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "PASSWORD":
            
            
                Password = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "PSW":
            
            
                Password = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "PWD":
            
            
                Password = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "PASSFILE":
            
            
                Passfile = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "APPLICATION NAME":
            
            
                ApplicationName = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "APPLICATIONNAME":
            
            
                ApplicationName = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "ENLIST":
            
            
                Enlist = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "SEARCH PATH":
            
            
                SearchPath = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "SEARCHPATH":
            
            
                SearchPath = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "CLIENT ENCODING":
            
            
                ClientEncoding = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "CLIENTENCODING":
            
            
                ClientEncoding = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "ENCODING":
            
            
                Encoding = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "TIMEZONE":
            
            
                Timezone = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "SSL MODE":
            
            
            {
                SslMode = value is string s
                    ? (SslMode)Enum.Parse(typeof(SslMode), s)
                    : (SslMode)Convert.ChangeType(value, typeof(SslMode));
            }
            
                break;
            
            case "SSLMODE":
            
            
            {
                SslMode = value is string s
                    ? (SslMode)Enum.Parse(typeof(SslMode), s)
                    : (SslMode)Convert.ChangeType(value, typeof(SslMode));
            }
            
                break;
            
            case "TRUST SERVER CERTIFICATE":
            
            
                TrustServerCertificate = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "TRUSTSERVERCERTIFICATE":
            
            
                TrustServerCertificate = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "SSL CERTIFICATE":
            
            
                SslCertificate = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "SSLCERTIFICATE":
            
            
                SslCertificate = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "SSL KEY":
            
            
                SslKey = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "SSLKEY":
            
            
                SslKey = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "SSL PASSWORD":
            
            
                SslPassword = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "SSLPASSWORD":
            
            
                SslPassword = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "ROOT CERTIFICATE":
            
            
                RootCertificate = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "ROOTCERTIFICATE":
            
            
                RootCertificate = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "CHECK CERTIFICATE REVOCATION":
            
            
                CheckCertificateRevocation = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "CHECKCERTIFICATEREVOCATION":
            
            
                CheckCertificateRevocation = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "INTEGRATED SECURITY":
            
            
                IntegratedSecurity = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "INTEGRATEDSECURITY":
            
            
                IntegratedSecurity = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "KERBEROS SERVICE NAME":
            
            
                KerberosServiceName = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "KERBEROSSERVICENAME":
            
            
                KerberosServiceName = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "KRBSRVNAME":
            
            
                KerberosServiceName = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "INCLUDE REALM":
            
            
                IncludeRealm = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "INCLUDEREALM":
            
            
                IncludeRealm = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "PERSIST SECURITY INFO":
            
            
                PersistSecurityInfo = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "PERSISTSECURITYINFO":
            
            
                PersistSecurityInfo = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "LOG PARAMETERS":
            
            
                LogParameters = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "LOGPARAMETERS":
            
            
                LogParameters = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "INCLUDE ERROR DETAIL":
            
            
                IncludeErrorDetail = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "INCLUDEERRORDETAIL":
            
            
                IncludeErrorDetail = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "POOLING":
            
            
                Pooling = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "MINIMUM POOL SIZE":
            
            
                MinPoolSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "MINPOOLSIZE":
            
            
                MinPoolSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "MAXIMUM POOL SIZE":
            
            
                MaxPoolSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "MAXPOOLSIZE":
            
            
                MaxPoolSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "CONNECTION IDLE LIFETIME":
            
            
                ConnectionIdleLifetime = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "CONNECTIONIDLELIFETIME":
            
            
                ConnectionIdleLifetime = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "CONNECTION PRUNING INTERVAL":
            
            
                ConnectionPruningInterval = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "CONNECTIONPRUNINGINTERVAL":
            
            
                ConnectionPruningInterval = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "CONNECTION LIFETIME":
            
            
                ConnectionLifetime = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "CONNECTIONLIFETIME":
            
            
                ConnectionLifetime = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "LOAD BALANCE TIMEOUT":
            
            
                ConnectionLifetime = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "TIMEOUT":
            
            
                Timeout = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "COMMAND TIMEOUT":
            
            
                CommandTimeout = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "COMMANDTIMEOUT":
            
            
                CommandTimeout = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "INTERNAL COMMAND TIMEOUT":
            
            
                InternalCommandTimeout = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "INTERNALCOMMANDTIMEOUT":
            
            
                InternalCommandTimeout = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "CANCELLATION TIMEOUT":
            
            
                CancellationTimeout = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "CANCELLATIONTIMEOUT":
            
            
                CancellationTimeout = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "TARGET SESSION ATTRIBUTES":
            
            
                TargetSessionAttributes = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "TARGETSESSIONATTRIBUTES":
            
            
                TargetSessionAttributes = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "LOAD BALANCE HOSTS":
            
            
                LoadBalanceHosts = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "LOADBALANCEHOSTS":
            
            
                LoadBalanceHosts = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "HOST RECHECK SECONDS":
            
            
                HostRecheckSeconds = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "HOSTRECHECKSECONDS":
            
            
                HostRecheckSeconds = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "EF TEMPLATE DATABASE":
            
            
                EntityTemplateDatabase = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "ENTITYTEMPLATEDATABASE":
            
            
                EntityTemplateDatabase = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "EF ADMIN DATABASE":
            
            
                EntityAdminDatabase = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "ENTITYADMINDATABASE":
            
            
                EntityAdminDatabase = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "KEEPALIVE":
            
            
                KeepAlive = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "TCP KEEPALIVE":
            
            
                TcpKeepAlive = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "TCPKEEPALIVE":
            
            
                TcpKeepAlive = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "TCP KEEPALIVE TIME":
            
            
                TcpKeepAliveTime = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "TCPKEEPALIVETIME":
            
            
                TcpKeepAliveTime = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "TCP KEEPALIVE INTERVAL":
            
            
                TcpKeepAliveInterval = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "TCPKEEPALIVEINTERVAL":
            
            
                TcpKeepAliveInterval = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "READ BUFFER SIZE":
            
            
                ReadBufferSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "READBUFFERSIZE":
            
            
                ReadBufferSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "WRITE BUFFER SIZE":
            
            
                WriteBufferSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "WRITEBUFFERSIZE":
            
            
                WriteBufferSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "SOCKET RECEIVE BUFFER SIZE":
            
            
                SocketReceiveBufferSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "SOCKETRECEIVEBUFFERSIZE":
            
            
                SocketReceiveBufferSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "SOCKET SEND BUFFER SIZE":
            
            
                SocketSendBufferSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "SOCKETSENDBUFFERSIZE":
            
            
                SocketSendBufferSize = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "MAX AUTO PREPARE":
            
            
                MaxAutoPrepare = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "MAXAUTOPREPARE":
            
            
                MaxAutoPrepare = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "AUTO PREPARE MIN USAGES":
            
            
                AutoPrepareMinUsages = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "AUTOPREPAREMINUSAGES":
            
            
                AutoPrepareMinUsages = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "NO RESET ON CLOSE":
            
            
                NoResetOnClose = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "NORESETONCLOSE":
            
            
                NoResetOnClose = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "LOAD TABLE COMPOSITES":
            
            
                LoadTableComposites = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "LOADTABLECOMPOSITES":
            
            
                LoadTableComposites = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "REPLICATION MODE":
            
            
            {
                ReplicationMode = value is string s
                    ? (ReplicationMode)Enum.Parse(typeof(ReplicationMode), s)
                    : (ReplicationMode)Convert.ChangeType(value, typeof(ReplicationMode));
            }
            
                break;
            
            case "REPLICATIONMODE":
            
            
            {
                ReplicationMode = value is string s
                    ? (ReplicationMode)Enum.Parse(typeof(ReplicationMode), s)
                    : (ReplicationMode)Convert.ChangeType(value, typeof(ReplicationMode));
            }
            
                break;
            
            case "OPTIONS":
            
            
                Options = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "ARRAY NULLABILITY MODE":
            
            
            {
                ArrayNullabilityMode = value is string s
                    ? (ArrayNullabilityMode)Enum.Parse(typeof(ArrayNullabilityMode), s)
                    : (ArrayNullabilityMode)Convert.ChangeType(value, typeof(ArrayNullabilityMode));
            }
            
                break;
            
            case "ARRAYNULLABILITYMODE":
            
            
            {
                ArrayNullabilityMode = value is string s
                    ? (ArrayNullabilityMode)Enum.Parse(typeof(ArrayNullabilityMode), s)
                    : (ArrayNullabilityMode)Convert.ChangeType(value, typeof(ArrayNullabilityMode));
            }
            
                break;
            
            case "MULTIPLEXING":
            
            
                Multiplexing = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "WRITE COALESCING BUFFER THRESHOLD BYTES":
            
            
                WriteCoalescingBufferThresholdBytes = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "WRITECOALESCINGBUFFERTHRESHOLDBYTES":
            
            
                WriteCoalescingBufferThresholdBytes = (Int32)Convert.ChangeType(value, typeof(Int32));
            
                break;
            
            case "SERVER COMPATIBILITY MODE":
            
            
            {
                ServerCompatibilityMode = value is string s
                    ? (ServerCompatibilityMode)Enum.Parse(typeof(ServerCompatibilityMode), s)
                    : (ServerCompatibilityMode)Convert.ChangeType(value, typeof(ServerCompatibilityMode));
            }
            
                break;
            
            case "SERVERCOMPATIBILITYMODE":
            
            
            {
                ServerCompatibilityMode = value is string s
                    ? (ServerCompatibilityMode)Enum.Parse(typeof(ServerCompatibilityMode), s)
                    : (ServerCompatibilityMode)Convert.ChangeType(value, typeof(ServerCompatibilityMode));
            }
            
                break;
            
            case "CONVERT INFINITY DATETIME":
            
            
                ConvertInfinityDateTime = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "CONVERTINFINITYDATETIME":
            
            
                ConvertInfinityDateTime = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "CONTINUOUS PROCESSING":
            
            
                ContinuousProcessing = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "CONTINUOUSPROCESSING":
            
            
                ContinuousProcessing = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "BACKEND TIMEOUTS":
            
            
                BackendTimeouts = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "BACKENDTIMEOUTS":
            
            
                BackendTimeouts = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "PRELOAD READER":
            
            
                PreloadReader = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "PRELOADREADER":
            
            
                PreloadReader = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "USE EXTENDED TYPES":
            
            
                UseExtendedTypes = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "USEEXTENDEDTYPES":
            
            
                UseExtendedTypes = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "USE SSL STREAM":
            
            
                UseSslStream = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "USESSLSTREAM":
            
            
                UseSslStream = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "USE PERF COUNTERS":
            
            
                UsePerfCounters = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "USEPERFCOUNTERS":
            
            
                UsePerfCounters = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "CLIENT CERTIFICATE":
            
            
                ClientCertificate = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "CLIENTCERTIFICATE":
            
            
                ClientCertificate = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "CLIENT CERTIFICATE KEY":
            
            
                ClientCertificateKey = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "CLIENTCERTIFICATEKEY":
            
            
                ClientCertificateKey = (String)Convert.ChangeType(value, typeof(String));
            
                break;
            
            case "INCLUDE ERROR DETAILS":
            
            
                IncludeErrorDetails = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            
            case "INCLUDEERRORDETAILS":
            
            
                IncludeErrorDetails = (Boolean)Convert.ChangeType(value, typeof(Boolean));
            
                break;
            

                default:
                    throw new KeyNotFoundException();
            }

            return 0;
        }

        private partial bool TryGetValueGenerated(string keyword, out object value)
        {
            switch (keyword)
            {
            
            case "HOST":
            
                value = (object)Host ?? "";
                return true;
            
            case "SERVER":
            
                value = (object)Host ?? "";
                return true;
            
            case "PORT":
            
                value = (object)Port ?? "";
                return true;
            
            case "DATABASE":
            
                value = (object)Database ?? "";
                return true;
            
            case "DB":
            
                value = (object)Database ?? "";
                return true;
            
            case "USERNAME":
            
                value = (object)Username ?? "";
                return true;
            
            case "USER NAME":
            
                value = (object)Username ?? "";
                return true;
            
            case "USERID":
            
                value = (object)Username ?? "";
                return true;
            
            case "USER ID":
            
                value = (object)Username ?? "";
                return true;
            
            case "UID":
            
                value = (object)Username ?? "";
                return true;
            
            case "PASSWORD":
            
                value = (object)Password ?? "";
                return true;
            
            case "PSW":
            
                value = (object)Password ?? "";
                return true;
            
            case "PWD":
            
                value = (object)Password ?? "";
                return true;
            
            case "PASSFILE":
            
                value = (object)Passfile ?? "";
                return true;
            
            case "APPLICATION NAME":
            
                value = (object)ApplicationName ?? "";
                return true;
            
            case "APPLICATIONNAME":
            
                value = (object)ApplicationName ?? "";
                return true;
            
            case "ENLIST":
            
                value = (object)Enlist ?? "";
                return true;
            
            case "SEARCH PATH":
            
                value = (object)SearchPath ?? "";
                return true;
            
            case "SEARCHPATH":
            
                value = (object)SearchPath ?? "";
                return true;
            
            case "CLIENT ENCODING":
            
                value = (object)ClientEncoding ?? "";
                return true;
            
            case "CLIENTENCODING":
            
                value = (object)ClientEncoding ?? "";
                return true;
            
            case "ENCODING":
            
                value = (object)Encoding ?? "";
                return true;
            
            case "TIMEZONE":
            
                value = (object)Timezone ?? "";
                return true;
            
            case "SSL MODE":
            
                value = (object)SslMode ?? "";
                return true;
            
            case "SSLMODE":
            
                value = (object)SslMode ?? "";
                return true;
            
            case "TRUST SERVER CERTIFICATE":
            
                value = (object)TrustServerCertificate ?? "";
                return true;
            
            case "TRUSTSERVERCERTIFICATE":
            
                value = (object)TrustServerCertificate ?? "";
                return true;
            
            case "SSL CERTIFICATE":
            
                value = (object)SslCertificate ?? "";
                return true;
            
            case "SSLCERTIFICATE":
            
                value = (object)SslCertificate ?? "";
                return true;
            
            case "SSL KEY":
            
                value = (object)SslKey ?? "";
                return true;
            
            case "SSLKEY":
            
                value = (object)SslKey ?? "";
                return true;
            
            case "SSL PASSWORD":
            
                value = (object)SslPassword ?? "";
                return true;
            
            case "SSLPASSWORD":
            
                value = (object)SslPassword ?? "";
                return true;
            
            case "ROOT CERTIFICATE":
            
                value = (object)RootCertificate ?? "";
                return true;
            
            case "ROOTCERTIFICATE":
            
                value = (object)RootCertificate ?? "";
                return true;
            
            case "CHECK CERTIFICATE REVOCATION":
            
                value = (object)CheckCertificateRevocation ?? "";
                return true;
            
            case "CHECKCERTIFICATEREVOCATION":
            
                value = (object)CheckCertificateRevocation ?? "";
                return true;
            
            case "INTEGRATED SECURITY":
            
                value = (object)IntegratedSecurity ?? "";
                return true;
            
            case "INTEGRATEDSECURITY":
            
                value = (object)IntegratedSecurity ?? "";
                return true;
            
            case "KERBEROS SERVICE NAME":
            
                value = (object)KerberosServiceName ?? "";
                return true;
            
            case "KERBEROSSERVICENAME":
            
                value = (object)KerberosServiceName ?? "";
                return true;
            
            case "KRBSRVNAME":
            
                value = (object)KerberosServiceName ?? "";
                return true;
            
            case "INCLUDE REALM":
            
                value = (object)IncludeRealm ?? "";
                return true;
            
            case "INCLUDEREALM":
            
                value = (object)IncludeRealm ?? "";
                return true;
            
            case "PERSIST SECURITY INFO":
            
                value = (object)PersistSecurityInfo ?? "";
                return true;
            
            case "PERSISTSECURITYINFO":
            
                value = (object)PersistSecurityInfo ?? "";
                return true;
            
            case "LOG PARAMETERS":
            
                value = (object)LogParameters ?? "";
                return true;
            
            case "LOGPARAMETERS":
            
                value = (object)LogParameters ?? "";
                return true;
            
            case "INCLUDE ERROR DETAIL":
            
                value = (object)IncludeErrorDetail ?? "";
                return true;
            
            case "INCLUDEERRORDETAIL":
            
                value = (object)IncludeErrorDetail ?? "";
                return true;
            
            case "POOLING":
            
                value = (object)Pooling ?? "";
                return true;
            
            case "MINIMUM POOL SIZE":
            
                value = (object)MinPoolSize ?? "";
                return true;
            
            case "MINPOOLSIZE":
            
                value = (object)MinPoolSize ?? "";
                return true;
            
            case "MAXIMUM POOL SIZE":
            
                value = (object)MaxPoolSize ?? "";
                return true;
            
            case "MAXPOOLSIZE":
            
                value = (object)MaxPoolSize ?? "";
                return true;
            
            case "CONNECTION IDLE LIFETIME":
            
                value = (object)ConnectionIdleLifetime ?? "";
                return true;
            
            case "CONNECTIONIDLELIFETIME":
            
                value = (object)ConnectionIdleLifetime ?? "";
                return true;
            
            case "CONNECTION PRUNING INTERVAL":
            
                value = (object)ConnectionPruningInterval ?? "";
                return true;
            
            case "CONNECTIONPRUNINGINTERVAL":
            
                value = (object)ConnectionPruningInterval ?? "";
                return true;
            
            case "CONNECTION LIFETIME":
            
                value = (object)ConnectionLifetime ?? "";
                return true;
            
            case "CONNECTIONLIFETIME":
            
                value = (object)ConnectionLifetime ?? "";
                return true;
            
            case "LOAD BALANCE TIMEOUT":
            
                value = (object)ConnectionLifetime ?? "";
                return true;
            
            case "TIMEOUT":
            
                value = (object)Timeout ?? "";
                return true;
            
            case "COMMAND TIMEOUT":
            
                value = (object)CommandTimeout ?? "";
                return true;
            
            case "COMMANDTIMEOUT":
            
                value = (object)CommandTimeout ?? "";
                return true;
            
            case "INTERNAL COMMAND TIMEOUT":
            
                value = (object)InternalCommandTimeout ?? "";
                return true;
            
            case "INTERNALCOMMANDTIMEOUT":
            
                value = (object)InternalCommandTimeout ?? "";
                return true;
            
            case "CANCELLATION TIMEOUT":
            
                value = (object)CancellationTimeout ?? "";
                return true;
            
            case "CANCELLATIONTIMEOUT":
            
                value = (object)CancellationTimeout ?? "";
                return true;
            
            case "TARGET SESSION ATTRIBUTES":
            
                value = (object)TargetSessionAttributes ?? "";
                return true;
            
            case "TARGETSESSIONATTRIBUTES":
            
                value = (object)TargetSessionAttributes ?? "";
                return true;
            
            case "LOAD BALANCE HOSTS":
            
                value = (object)LoadBalanceHosts ?? "";
                return true;
            
            case "LOADBALANCEHOSTS":
            
                value = (object)LoadBalanceHosts ?? "";
                return true;
            
            case "HOST RECHECK SECONDS":
            
                value = (object)HostRecheckSeconds ?? "";
                return true;
            
            case "HOSTRECHECKSECONDS":
            
                value = (object)HostRecheckSeconds ?? "";
                return true;
            
            case "EF TEMPLATE DATABASE":
            
                value = (object)EntityTemplateDatabase ?? "";
                return true;
            
            case "ENTITYTEMPLATEDATABASE":
            
                value = (object)EntityTemplateDatabase ?? "";
                return true;
            
            case "EF ADMIN DATABASE":
            
                value = (object)EntityAdminDatabase ?? "";
                return true;
            
            case "ENTITYADMINDATABASE":
            
                value = (object)EntityAdminDatabase ?? "";
                return true;
            
            case "KEEPALIVE":
            
                value = (object)KeepAlive ?? "";
                return true;
            
            case "TCP KEEPALIVE":
            
                value = (object)TcpKeepAlive ?? "";
                return true;
            
            case "TCPKEEPALIVE":
            
                value = (object)TcpKeepAlive ?? "";
                return true;
            
            case "TCP KEEPALIVE TIME":
            
                value = (object)TcpKeepAliveTime ?? "";
                return true;
            
            case "TCPKEEPALIVETIME":
            
                value = (object)TcpKeepAliveTime ?? "";
                return true;
            
            case "TCP KEEPALIVE INTERVAL":
            
                value = (object)TcpKeepAliveInterval ?? "";
                return true;
            
            case "TCPKEEPALIVEINTERVAL":
            
                value = (object)TcpKeepAliveInterval ?? "";
                return true;
            
            case "READ BUFFER SIZE":
            
                value = (object)ReadBufferSize ?? "";
                return true;
            
            case "READBUFFERSIZE":
            
                value = (object)ReadBufferSize ?? "";
                return true;
            
            case "WRITE BUFFER SIZE":
            
                value = (object)WriteBufferSize ?? "";
                return true;
            
            case "WRITEBUFFERSIZE":
            
                value = (object)WriteBufferSize ?? "";
                return true;
            
            case "SOCKET RECEIVE BUFFER SIZE":
            
                value = (object)SocketReceiveBufferSize ?? "";
                return true;
            
            case "SOCKETRECEIVEBUFFERSIZE":
            
                value = (object)SocketReceiveBufferSize ?? "";
                return true;
            
            case "SOCKET SEND BUFFER SIZE":
            
                value = (object)SocketSendBufferSize ?? "";
                return true;
            
            case "SOCKETSENDBUFFERSIZE":
            
                value = (object)SocketSendBufferSize ?? "";
                return true;
            
            case "MAX AUTO PREPARE":
            
                value = (object)MaxAutoPrepare ?? "";
                return true;
            
            case "MAXAUTOPREPARE":
            
                value = (object)MaxAutoPrepare ?? "";
                return true;
            
            case "AUTO PREPARE MIN USAGES":
            
                value = (object)AutoPrepareMinUsages ?? "";
                return true;
            
            case "AUTOPREPAREMINUSAGES":
            
                value = (object)AutoPrepareMinUsages ?? "";
                return true;
            
            case "NO RESET ON CLOSE":
            
                value = (object)NoResetOnClose ?? "";
                return true;
            
            case "NORESETONCLOSE":
            
                value = (object)NoResetOnClose ?? "";
                return true;
            
            case "LOAD TABLE COMPOSITES":
            
                value = (object)LoadTableComposites ?? "";
                return true;
            
            case "LOADTABLECOMPOSITES":
            
                value = (object)LoadTableComposites ?? "";
                return true;
            
            case "REPLICATION MODE":
            
                value = (object)ReplicationMode ?? "";
                return true;
            
            case "REPLICATIONMODE":
            
                value = (object)ReplicationMode ?? "";
                return true;
            
            case "OPTIONS":
            
                value = (object)Options ?? "";
                return true;
            
            case "ARRAY NULLABILITY MODE":
            
                value = (object)ArrayNullabilityMode ?? "";
                return true;
            
            case "ARRAYNULLABILITYMODE":
            
                value = (object)ArrayNullabilityMode ?? "";
                return true;
            
            case "MULTIPLEXING":
            
                value = (object)Multiplexing ?? "";
                return true;
            
            case "WRITE COALESCING BUFFER THRESHOLD BYTES":
            
                value = (object)WriteCoalescingBufferThresholdBytes ?? "";
                return true;
            
            case "WRITECOALESCINGBUFFERTHRESHOLDBYTES":
            
                value = (object)WriteCoalescingBufferThresholdBytes ?? "";
                return true;
            
            case "SERVER COMPATIBILITY MODE":
            
                value = (object)ServerCompatibilityMode ?? "";
                return true;
            
            case "SERVERCOMPATIBILITYMODE":
            
                value = (object)ServerCompatibilityMode ?? "";
                return true;
            
            case "CONVERT INFINITY DATETIME":
            
                value = (object)ConvertInfinityDateTime ?? "";
                return true;
            
            case "CONVERTINFINITYDATETIME":
            
                value = (object)ConvertInfinityDateTime ?? "";
                return true;
            
            case "CONTINUOUS PROCESSING":
            
                value = (object)ContinuousProcessing ?? "";
                return true;
            
            case "CONTINUOUSPROCESSING":
            
                value = (object)ContinuousProcessing ?? "";
                return true;
            
            case "BACKEND TIMEOUTS":
            
                value = (object)BackendTimeouts ?? "";
                return true;
            
            case "BACKENDTIMEOUTS":
            
                value = (object)BackendTimeouts ?? "";
                return true;
            
            case "PRELOAD READER":
            
                value = (object)PreloadReader ?? "";
                return true;
            
            case "PRELOADREADER":
            
                value = (object)PreloadReader ?? "";
                return true;
            
            case "USE EXTENDED TYPES":
            
                value = (object)UseExtendedTypes ?? "";
                return true;
            
            case "USEEXTENDEDTYPES":
            
                value = (object)UseExtendedTypes ?? "";
                return true;
            
            case "USE SSL STREAM":
            
                value = (object)UseSslStream ?? "";
                return true;
            
            case "USESSLSTREAM":
            
                value = (object)UseSslStream ?? "";
                return true;
            
            case "USE PERF COUNTERS":
            
                value = (object)UsePerfCounters ?? "";
                return true;
            
            case "USEPERFCOUNTERS":
            
                value = (object)UsePerfCounters ?? "";
                return true;
            
            case "CLIENT CERTIFICATE":
            
                value = (object)ClientCertificate ?? "";
                return true;
            
            case "CLIENTCERTIFICATE":
            
                value = (object)ClientCertificate ?? "";
                return true;
            
            case "CLIENT CERTIFICATE KEY":
            
                value = (object)ClientCertificateKey ?? "";
                return true;
            
            case "CLIENTCERTIFICATEKEY":
            
                value = (object)ClientCertificateKey ?? "";
                return true;
            
            case "INCLUDE ERROR DETAILS":
            
                value = (object)IncludeErrorDetails ?? "";
                return true;
            
            case "INCLUDEERRORDETAILS":
            
                value = (object)IncludeErrorDetails ?? "";
                return true;
            
            }

            value = null;
            return false;
        }

        private partial bool ContainsKeyGenerated(string keyword)
            => keyword switch
            {
                
                "HOST" => true,
                
                "SERVER" => true,
                
                "PORT" => true,
                
                "DATABASE" => true,
                
                "DB" => true,
                
                "USERNAME" => true,
                
                "USER NAME" => true,
                
                "USERID" => true,
                
                "USER ID" => true,
                
                "UID" => true,
                
                "PASSWORD" => true,
                
                "PSW" => true,
                
                "PWD" => true,
                
                "PASSFILE" => true,
                
                "APPLICATION NAME" => true,
                
                "APPLICATIONNAME" => true,
                
                "ENLIST" => true,
                
                "SEARCH PATH" => true,
                
                "SEARCHPATH" => true,
                
                "CLIENT ENCODING" => true,
                
                "CLIENTENCODING" => true,
                
                "ENCODING" => true,
                
                "TIMEZONE" => true,
                
                "SSL MODE" => true,
                
                "SSLMODE" => true,
                
                "TRUST SERVER CERTIFICATE" => true,
                
                "TRUSTSERVERCERTIFICATE" => true,
                
                "SSL CERTIFICATE" => true,
                
                "SSLCERTIFICATE" => true,
                
                "SSL KEY" => true,
                
                "SSLKEY" => true,
                
                "SSL PASSWORD" => true,
                
                "SSLPASSWORD" => true,
                
                "ROOT CERTIFICATE" => true,
                
                "ROOTCERTIFICATE" => true,
                
                "CHECK CERTIFICATE REVOCATION" => true,
                
                "CHECKCERTIFICATEREVOCATION" => true,
                
                "INTEGRATED SECURITY" => true,
                
                "INTEGRATEDSECURITY" => true,
                
                "KERBEROS SERVICE NAME" => true,
                
                "KERBEROSSERVICENAME" => true,
                
                "KRBSRVNAME" => true,
                
                "INCLUDE REALM" => true,
                
                "INCLUDEREALM" => true,
                
                "PERSIST SECURITY INFO" => true,
                
                "PERSISTSECURITYINFO" => true,
                
                "LOG PARAMETERS" => true,
                
                "LOGPARAMETERS" => true,
                
                "INCLUDE ERROR DETAIL" => true,
                
                "INCLUDEERRORDETAIL" => true,
                
                "POOLING" => true,
                
                "MINIMUM POOL SIZE" => true,
                
                "MINPOOLSIZE" => true,
                
                "MAXIMUM POOL SIZE" => true,
                
                "MAXPOOLSIZE" => true,
                
                "CONNECTION IDLE LIFETIME" => true,
                
                "CONNECTIONIDLELIFETIME" => true,
                
                "CONNECTION PRUNING INTERVAL" => true,
                
                "CONNECTIONPRUNINGINTERVAL" => true,
                
                "CONNECTION LIFETIME" => true,
                
                "CONNECTIONLIFETIME" => true,
                
                "LOAD BALANCE TIMEOUT" => true,
                
                "TIMEOUT" => true,
                
                "COMMAND TIMEOUT" => true,
                
                "COMMANDTIMEOUT" => true,
                
                "INTERNAL COMMAND TIMEOUT" => true,
                
                "INTERNALCOMMANDTIMEOUT" => true,
                
                "CANCELLATION TIMEOUT" => true,
                
                "CANCELLATIONTIMEOUT" => true,
                
                "TARGET SESSION ATTRIBUTES" => true,
                
                "TARGETSESSIONATTRIBUTES" => true,
                
                "LOAD BALANCE HOSTS" => true,
                
                "LOADBALANCEHOSTS" => true,
                
                "HOST RECHECK SECONDS" => true,
                
                "HOSTRECHECKSECONDS" => true,
                
                "EF TEMPLATE DATABASE" => true,
                
                "ENTITYTEMPLATEDATABASE" => true,
                
                "EF ADMIN DATABASE" => true,
                
                "ENTITYADMINDATABASE" => true,
                
                "KEEPALIVE" => true,
                
                "TCP KEEPALIVE" => true,
                
                "TCPKEEPALIVE" => true,
                
                "TCP KEEPALIVE TIME" => true,
                
                "TCPKEEPALIVETIME" => true,
                
                "TCP KEEPALIVE INTERVAL" => true,
                
                "TCPKEEPALIVEINTERVAL" => true,
                
                "READ BUFFER SIZE" => true,
                
                "READBUFFERSIZE" => true,
                
                "WRITE BUFFER SIZE" => true,
                
                "WRITEBUFFERSIZE" => true,
                
                "SOCKET RECEIVE BUFFER SIZE" => true,
                
                "SOCKETRECEIVEBUFFERSIZE" => true,
                
                "SOCKET SEND BUFFER SIZE" => true,
                
                "SOCKETSENDBUFFERSIZE" => true,
                
                "MAX AUTO PREPARE" => true,
                
                "MAXAUTOPREPARE" => true,
                
                "AUTO PREPARE MIN USAGES" => true,
                
                "AUTOPREPAREMINUSAGES" => true,
                
                "NO RESET ON CLOSE" => true,
                
                "NORESETONCLOSE" => true,
                
                "LOAD TABLE COMPOSITES" => true,
                
                "LOADTABLECOMPOSITES" => true,
                
                "REPLICATION MODE" => true,
                
                "REPLICATIONMODE" => true,
                
                "OPTIONS" => true,
                
                "ARRAY NULLABILITY MODE" => true,
                
                "ARRAYNULLABILITYMODE" => true,
                
                "MULTIPLEXING" => true,
                
                "WRITE COALESCING BUFFER THRESHOLD BYTES" => true,
                
                "WRITECOALESCINGBUFFERTHRESHOLDBYTES" => true,
                
                "SERVER COMPATIBILITY MODE" => true,
                
                "SERVERCOMPATIBILITYMODE" => true,
                
                "CONVERT INFINITY DATETIME" => true,
                
                "CONVERTINFINITYDATETIME" => true,
                
                "CONTINUOUS PROCESSING" => true,
                
                "CONTINUOUSPROCESSING" => true,
                
                "BACKEND TIMEOUTS" => true,
                
                "BACKENDTIMEOUTS" => true,
                
                "PRELOAD READER" => true,
                
                "PRELOADREADER" => true,
                
                "USE EXTENDED TYPES" => true,
                
                "USEEXTENDEDTYPES" => true,
                
                "USE SSL STREAM" => true,
                
                "USESSLSTREAM" => true,
                
                "USE PERF COUNTERS" => true,
                
                "USEPERFCOUNTERS" => true,
                
                "CLIENT CERTIFICATE" => true,
                
                "CLIENTCERTIFICATE" => true,
                
                "CLIENT CERTIFICATE KEY" => true,
                
                "CLIENTCERTIFICATEKEY" => true,
                
                "INCLUDE ERROR DETAILS" => true,
                
                "INCLUDEERRORDETAILS" => true,
                

                _ => false
            };

        private partial bool RemoveGenerated(string keyword)
        {
            switch (keyword)
            {
            
            case "HOST":
            {
                
                var removed = base.ContainsKey("Host");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Host = default;
                
                base.Remove("Host");
                return removed;
            }
            
            case "SERVER":
            {
                
                var removed = base.ContainsKey("Host");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Host = default;
                
                base.Remove("Host");
                return removed;
            }
            
            case "PORT":
            {
                
                var removed = base.ContainsKey("Port");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Port = 5432;
                
                base.Remove("Port");
                return removed;
            }
            
            case "DATABASE":
            {
                
                var removed = base.ContainsKey("Database");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Database = default;
                
                base.Remove("Database");
                return removed;
            }
            
            case "DB":
            {
                
                var removed = base.ContainsKey("Database");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Database = default;
                
                base.Remove("Database");
                return removed;
            }
            
            case "USERNAME":
            {
                
                var removed = base.ContainsKey("Username");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Username = default;
                
                base.Remove("Username");
                return removed;
            }
            
            case "USER NAME":
            {
                
                var removed = base.ContainsKey("Username");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Username = default;
                
                base.Remove("Username");
                return removed;
            }
            
            case "USERID":
            {
                
                var removed = base.ContainsKey("Username");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Username = default;
                
                base.Remove("Username");
                return removed;
            }
            
            case "USER ID":
            {
                
                var removed = base.ContainsKey("Username");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Username = default;
                
                base.Remove("Username");
                return removed;
            }
            
            case "UID":
            {
                
                var removed = base.ContainsKey("Username");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Username = default;
                
                base.Remove("Username");
                return removed;
            }
            
            case "PASSWORD":
            {
                
                var removed = base.ContainsKey("Password");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Password = default;
                
                base.Remove("Password");
                return removed;
            }
            
            case "PSW":
            {
                
                var removed = base.ContainsKey("Password");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Password = default;
                
                base.Remove("Password");
                return removed;
            }
            
            case "PWD":
            {
                
                var removed = base.ContainsKey("Password");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Password = default;
                
                base.Remove("Password");
                return removed;
            }
            
            case "PASSFILE":
            {
                
                var removed = base.ContainsKey("Passfile");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Passfile = default;
                
                base.Remove("Passfile");
                return removed;
            }
            
            case "APPLICATION NAME":
            {
                
                var removed = base.ContainsKey("Application Name");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ApplicationName = default;
                
                base.Remove("Application Name");
                return removed;
            }
            
            case "APPLICATIONNAME":
            {
                
                var removed = base.ContainsKey("Application Name");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ApplicationName = default;
                
                base.Remove("Application Name");
                return removed;
            }
            
            case "ENLIST":
            {
                
                var removed = base.ContainsKey("Enlist");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Enlist = true;
                
                base.Remove("Enlist");
                return removed;
            }
            
            case "SEARCH PATH":
            {
                
                var removed = base.ContainsKey("Search Path");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SearchPath = default;
                
                base.Remove("Search Path");
                return removed;
            }
            
            case "SEARCHPATH":
            {
                
                var removed = base.ContainsKey("Search Path");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SearchPath = default;
                
                base.Remove("Search Path");
                return removed;
            }
            
            case "CLIENT ENCODING":
            {
                
                var removed = base.ContainsKey("Client Encoding");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ClientEncoding = default;
                
                base.Remove("Client Encoding");
                return removed;
            }
            
            case "CLIENTENCODING":
            {
                
                var removed = base.ContainsKey("Client Encoding");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ClientEncoding = default;
                
                base.Remove("Client Encoding");
                return removed;
            }
            
            case "ENCODING":
            {
                
                var removed = base.ContainsKey("Encoding");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Encoding = "UTF8";
                
                base.Remove("Encoding");
                return removed;
            }
            
            case "TIMEZONE":
            {
                
                var removed = base.ContainsKey("Timezone");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Timezone = default;
                
                base.Remove("Timezone");
                return removed;
            }
            
            case "SSL MODE":
            {
                
                var removed = base.ContainsKey("SSL Mode");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SslMode = SslMode.Prefer;
                
                base.Remove("SSL Mode");
                return removed;
            }
            
            case "SSLMODE":
            {
                
                var removed = base.ContainsKey("SSL Mode");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SslMode = SslMode.Prefer;
                
                base.Remove("SSL Mode");
                return removed;
            }
            
            case "TRUST SERVER CERTIFICATE":
            {
                
                var removed = base.ContainsKey("Trust Server Certificate");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                TrustServerCertificate = default;
                
                base.Remove("Trust Server Certificate");
                return removed;
            }
            
            case "TRUSTSERVERCERTIFICATE":
            {
                
                var removed = base.ContainsKey("Trust Server Certificate");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                TrustServerCertificate = default;
                
                base.Remove("Trust Server Certificate");
                return removed;
            }
            
            case "SSL CERTIFICATE":
            {
                
                var removed = base.ContainsKey("SSL Certificate");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SslCertificate = default;
                
                base.Remove("SSL Certificate");
                return removed;
            }
            
            case "SSLCERTIFICATE":
            {
                
                var removed = base.ContainsKey("SSL Certificate");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SslCertificate = default;
                
                base.Remove("SSL Certificate");
                return removed;
            }
            
            case "SSL KEY":
            {
                
                var removed = base.ContainsKey("SSL Key");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SslKey = default;
                
                base.Remove("SSL Key");
                return removed;
            }
            
            case "SSLKEY":
            {
                
                var removed = base.ContainsKey("SSL Key");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SslKey = default;
                
                base.Remove("SSL Key");
                return removed;
            }
            
            case "SSL PASSWORD":
            {
                
                var removed = base.ContainsKey("SSL Password");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SslPassword = default;
                
                base.Remove("SSL Password");
                return removed;
            }
            
            case "SSLPASSWORD":
            {
                
                var removed = base.ContainsKey("SSL Password");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SslPassword = default;
                
                base.Remove("SSL Password");
                return removed;
            }
            
            case "ROOT CERTIFICATE":
            {
                
                var removed = base.ContainsKey("Root Certificate");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                RootCertificate = default;
                
                base.Remove("Root Certificate");
                return removed;
            }
            
            case "ROOTCERTIFICATE":
            {
                
                var removed = base.ContainsKey("Root Certificate");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                RootCertificate = default;
                
                base.Remove("Root Certificate");
                return removed;
            }
            
            case "CHECK CERTIFICATE REVOCATION":
            {
                
                var removed = base.ContainsKey("Check Certificate Revocation");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                CheckCertificateRevocation = true;
                
                base.Remove("Check Certificate Revocation");
                return removed;
            }
            
            case "CHECKCERTIFICATEREVOCATION":
            {
                
                var removed = base.ContainsKey("Check Certificate Revocation");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                CheckCertificateRevocation = true;
                
                base.Remove("Check Certificate Revocation");
                return removed;
            }
            
            case "INTEGRATED SECURITY":
            {
                
                var removed = base.ContainsKey("Integrated Security");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                IntegratedSecurity = default;
                
                base.Remove("Integrated Security");
                return removed;
            }
            
            case "INTEGRATEDSECURITY":
            {
                
                var removed = base.ContainsKey("Integrated Security");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                IntegratedSecurity = default;
                
                base.Remove("Integrated Security");
                return removed;
            }
            
            case "KERBEROS SERVICE NAME":
            {
                
                var removed = base.ContainsKey("Kerberos Service Name");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                KerberosServiceName = "postgres";
                
                base.Remove("Kerberos Service Name");
                return removed;
            }
            
            case "KERBEROSSERVICENAME":
            {
                
                var removed = base.ContainsKey("Kerberos Service Name");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                KerberosServiceName = "postgres";
                
                base.Remove("Kerberos Service Name");
                return removed;
            }
            
            case "KRBSRVNAME":
            {
                
                var removed = base.ContainsKey("Kerberos Service Name");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                KerberosServiceName = "postgres";
                
                base.Remove("Kerberos Service Name");
                return removed;
            }
            
            case "INCLUDE REALM":
            {
                
                var removed = base.ContainsKey("Include Realm");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                IncludeRealm = default;
                
                base.Remove("Include Realm");
                return removed;
            }
            
            case "INCLUDEREALM":
            {
                
                var removed = base.ContainsKey("Include Realm");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                IncludeRealm = default;
                
                base.Remove("Include Realm");
                return removed;
            }
            
            case "PERSIST SECURITY INFO":
            {
                
                var removed = base.ContainsKey("Persist Security Info");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                PersistSecurityInfo = default;
                
                base.Remove("Persist Security Info");
                return removed;
            }
            
            case "PERSISTSECURITYINFO":
            {
                
                var removed = base.ContainsKey("Persist Security Info");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                PersistSecurityInfo = default;
                
                base.Remove("Persist Security Info");
                return removed;
            }
            
            case "LOG PARAMETERS":
            {
                
                var removed = base.ContainsKey("Log Parameters");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                LogParameters = default;
                
                base.Remove("Log Parameters");
                return removed;
            }
            
            case "LOGPARAMETERS":
            {
                
                var removed = base.ContainsKey("Log Parameters");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                LogParameters = default;
                
                base.Remove("Log Parameters");
                return removed;
            }
            
            case "INCLUDE ERROR DETAIL":
            {
                
                var removed = base.ContainsKey("Include Error Detail");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                IncludeErrorDetail = default;
                
                base.Remove("Include Error Detail");
                return removed;
            }
            
            case "INCLUDEERRORDETAIL":
            {
                
                var removed = base.ContainsKey("Include Error Detail");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                IncludeErrorDetail = default;
                
                base.Remove("Include Error Detail");
                return removed;
            }
            
            case "POOLING":
            {
                
                var removed = base.ContainsKey("Pooling");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Pooling = true;
                
                base.Remove("Pooling");
                return removed;
            }
            
            case "MINIMUM POOL SIZE":
            {
                
                var removed = base.ContainsKey("Minimum Pool Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                MinPoolSize = 0;
                
                base.Remove("Minimum Pool Size");
                return removed;
            }
            
            case "MINPOOLSIZE":
            {
                
                var removed = base.ContainsKey("Minimum Pool Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                MinPoolSize = 0;
                
                base.Remove("Minimum Pool Size");
                return removed;
            }
            
            case "MAXIMUM POOL SIZE":
            {
                
                var removed = base.ContainsKey("Maximum Pool Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                MaxPoolSize = 100;
                
                base.Remove("Maximum Pool Size");
                return removed;
            }
            
            case "MAXPOOLSIZE":
            {
                
                var removed = base.ContainsKey("Maximum Pool Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                MaxPoolSize = 100;
                
                base.Remove("Maximum Pool Size");
                return removed;
            }
            
            case "CONNECTION IDLE LIFETIME":
            {
                
                var removed = base.ContainsKey("Connection Idle Lifetime");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ConnectionIdleLifetime = 300;
                
                base.Remove("Connection Idle Lifetime");
                return removed;
            }
            
            case "CONNECTIONIDLELIFETIME":
            {
                
                var removed = base.ContainsKey("Connection Idle Lifetime");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ConnectionIdleLifetime = 300;
                
                base.Remove("Connection Idle Lifetime");
                return removed;
            }
            
            case "CONNECTION PRUNING INTERVAL":
            {
                
                var removed = base.ContainsKey("Connection Pruning Interval");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ConnectionPruningInterval = 10;
                
                base.Remove("Connection Pruning Interval");
                return removed;
            }
            
            case "CONNECTIONPRUNINGINTERVAL":
            {
                
                var removed = base.ContainsKey("Connection Pruning Interval");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ConnectionPruningInterval = 10;
                
                base.Remove("Connection Pruning Interval");
                return removed;
            }
            
            case "CONNECTION LIFETIME":
            {
                
                var removed = base.ContainsKey("Connection Lifetime");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ConnectionLifetime = default;
                
                base.Remove("Connection Lifetime");
                return removed;
            }
            
            case "CONNECTIONLIFETIME":
            {
                
                var removed = base.ContainsKey("Connection Lifetime");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ConnectionLifetime = default;
                
                base.Remove("Connection Lifetime");
                return removed;
            }
            
            case "LOAD BALANCE TIMEOUT":
            {
                
                var removed = base.ContainsKey("Connection Lifetime");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ConnectionLifetime = default;
                
                base.Remove("Connection Lifetime");
                return removed;
            }
            
            case "TIMEOUT":
            {
                
                var removed = base.ContainsKey("Timeout");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Timeout = 15;
                
                base.Remove("Timeout");
                return removed;
            }
            
            case "COMMAND TIMEOUT":
            {
                
                var removed = base.ContainsKey("Command Timeout");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                CommandTimeout = 30;
                
                base.Remove("Command Timeout");
                return removed;
            }
            
            case "COMMANDTIMEOUT":
            {
                
                var removed = base.ContainsKey("Command Timeout");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                CommandTimeout = 30;
                
                base.Remove("Command Timeout");
                return removed;
            }
            
            case "INTERNAL COMMAND TIMEOUT":
            {
                
                var removed = base.ContainsKey("Internal Command Timeout");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                InternalCommandTimeout = -1;
                
                base.Remove("Internal Command Timeout");
                return removed;
            }
            
            case "INTERNALCOMMANDTIMEOUT":
            {
                
                var removed = base.ContainsKey("Internal Command Timeout");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                InternalCommandTimeout = -1;
                
                base.Remove("Internal Command Timeout");
                return removed;
            }
            
            case "CANCELLATION TIMEOUT":
            {
                
                var removed = base.ContainsKey("Cancellation Timeout");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                CancellationTimeout = 2000;
                
                base.Remove("Cancellation Timeout");
                return removed;
            }
            
            case "CANCELLATIONTIMEOUT":
            {
                
                var removed = base.ContainsKey("Cancellation Timeout");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                CancellationTimeout = 2000;
                
                base.Remove("Cancellation Timeout");
                return removed;
            }
            
            case "TARGET SESSION ATTRIBUTES":
            {
                
                var removed = base.ContainsKey("Target Session Attributes");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                TargetSessionAttributes = default;
                
                base.Remove("Target Session Attributes");
                return removed;
            }
            
            case "TARGETSESSIONATTRIBUTES":
            {
                
                var removed = base.ContainsKey("Target Session Attributes");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                TargetSessionAttributes = default;
                
                base.Remove("Target Session Attributes");
                return removed;
            }
            
            case "LOAD BALANCE HOSTS":
            {
                
                var removed = base.ContainsKey("Load Balance Hosts");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                LoadBalanceHosts = default;
                
                base.Remove("Load Balance Hosts");
                return removed;
            }
            
            case "LOADBALANCEHOSTS":
            {
                
                var removed = base.ContainsKey("Load Balance Hosts");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                LoadBalanceHosts = default;
                
                base.Remove("Load Balance Hosts");
                return removed;
            }
            
            case "HOST RECHECK SECONDS":
            {
                
                var removed = base.ContainsKey("Host Recheck Seconds");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                HostRecheckSeconds = 10;
                
                base.Remove("Host Recheck Seconds");
                return removed;
            }
            
            case "HOSTRECHECKSECONDS":
            {
                
                var removed = base.ContainsKey("Host Recheck Seconds");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                HostRecheckSeconds = 10;
                
                base.Remove("Host Recheck Seconds");
                return removed;
            }
            
            case "EF TEMPLATE DATABASE":
            {
                
                var removed = base.ContainsKey("EF Template Database");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                EntityTemplateDatabase = default;
                
                base.Remove("EF Template Database");
                return removed;
            }
            
            case "ENTITYTEMPLATEDATABASE":
            {
                
                var removed = base.ContainsKey("EF Template Database");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                EntityTemplateDatabase = default;
                
                base.Remove("EF Template Database");
                return removed;
            }
            
            case "EF ADMIN DATABASE":
            {
                
                var removed = base.ContainsKey("EF Admin Database");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                EntityAdminDatabase = default;
                
                base.Remove("EF Admin Database");
                return removed;
            }
            
            case "ENTITYADMINDATABASE":
            {
                
                var removed = base.ContainsKey("EF Admin Database");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                EntityAdminDatabase = default;
                
                base.Remove("EF Admin Database");
                return removed;
            }
            
            case "KEEPALIVE":
            {
                
                var removed = base.ContainsKey("Keepalive");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                KeepAlive = default;
                
                base.Remove("Keepalive");
                return removed;
            }
            
            case "TCP KEEPALIVE":
            {
                
                var removed = base.ContainsKey("TCP Keepalive");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                TcpKeepAlive = default;
                
                base.Remove("TCP Keepalive");
                return removed;
            }
            
            case "TCPKEEPALIVE":
            {
                
                var removed = base.ContainsKey("TCP Keepalive");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                TcpKeepAlive = default;
                
                base.Remove("TCP Keepalive");
                return removed;
            }
            
            case "TCP KEEPALIVE TIME":
            {
                
                var removed = base.ContainsKey("TCP Keepalive Time");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                TcpKeepAliveTime = default;
                
                base.Remove("TCP Keepalive Time");
                return removed;
            }
            
            case "TCPKEEPALIVETIME":
            {
                
                var removed = base.ContainsKey("TCP Keepalive Time");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                TcpKeepAliveTime = default;
                
                base.Remove("TCP Keepalive Time");
                return removed;
            }
            
            case "TCP KEEPALIVE INTERVAL":
            {
                
                var removed = base.ContainsKey("TCP Keepalive Interval");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                TcpKeepAliveInterval = default;
                
                base.Remove("TCP Keepalive Interval");
                return removed;
            }
            
            case "TCPKEEPALIVEINTERVAL":
            {
                
                var removed = base.ContainsKey("TCP Keepalive Interval");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                TcpKeepAliveInterval = default;
                
                base.Remove("TCP Keepalive Interval");
                return removed;
            }
            
            case "READ BUFFER SIZE":
            {
                
                var removed = base.ContainsKey("Read Buffer Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ReadBufferSize = 8192;
                
                base.Remove("Read Buffer Size");
                return removed;
            }
            
            case "READBUFFERSIZE":
            {
                
                var removed = base.ContainsKey("Read Buffer Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ReadBufferSize = 8192;
                
                base.Remove("Read Buffer Size");
                return removed;
            }
            
            case "WRITE BUFFER SIZE":
            {
                
                var removed = base.ContainsKey("Write Buffer Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                WriteBufferSize = 8192;
                
                base.Remove("Write Buffer Size");
                return removed;
            }
            
            case "WRITEBUFFERSIZE":
            {
                
                var removed = base.ContainsKey("Write Buffer Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                WriteBufferSize = 8192;
                
                base.Remove("Write Buffer Size");
                return removed;
            }
            
            case "SOCKET RECEIVE BUFFER SIZE":
            {
                
                var removed = base.ContainsKey("Socket Receive Buffer Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SocketReceiveBufferSize = default;
                
                base.Remove("Socket Receive Buffer Size");
                return removed;
            }
            
            case "SOCKETRECEIVEBUFFERSIZE":
            {
                
                var removed = base.ContainsKey("Socket Receive Buffer Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SocketReceiveBufferSize = default;
                
                base.Remove("Socket Receive Buffer Size");
                return removed;
            }
            
            case "SOCKET SEND BUFFER SIZE":
            {
                
                var removed = base.ContainsKey("Socket Send Buffer Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SocketSendBufferSize = default;
                
                base.Remove("Socket Send Buffer Size");
                return removed;
            }
            
            case "SOCKETSENDBUFFERSIZE":
            {
                
                var removed = base.ContainsKey("Socket Send Buffer Size");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                SocketSendBufferSize = default;
                
                base.Remove("Socket Send Buffer Size");
                return removed;
            }
            
            case "MAX AUTO PREPARE":
            {
                
                var removed = base.ContainsKey("Max Auto Prepare");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                MaxAutoPrepare = default;
                
                base.Remove("Max Auto Prepare");
                return removed;
            }
            
            case "MAXAUTOPREPARE":
            {
                
                var removed = base.ContainsKey("Max Auto Prepare");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                MaxAutoPrepare = default;
                
                base.Remove("Max Auto Prepare");
                return removed;
            }
            
            case "AUTO PREPARE MIN USAGES":
            {
                
                var removed = base.ContainsKey("Auto Prepare Min Usages");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                AutoPrepareMinUsages = 5;
                
                base.Remove("Auto Prepare Min Usages");
                return removed;
            }
            
            case "AUTOPREPAREMINUSAGES":
            {
                
                var removed = base.ContainsKey("Auto Prepare Min Usages");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                AutoPrepareMinUsages = 5;
                
                base.Remove("Auto Prepare Min Usages");
                return removed;
            }
            
            case "NO RESET ON CLOSE":
            {
                
                var removed = base.ContainsKey("No Reset On Close");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                NoResetOnClose = default;
                
                base.Remove("No Reset On Close");
                return removed;
            }
            
            case "NORESETONCLOSE":
            {
                
                var removed = base.ContainsKey("No Reset On Close");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                NoResetOnClose = default;
                
                base.Remove("No Reset On Close");
                return removed;
            }
            
            case "LOAD TABLE COMPOSITES":
            {
                
                var removed = base.ContainsKey("Load Table Composites");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                LoadTableComposites = default;
                
                base.Remove("Load Table Composites");
                return removed;
            }
            
            case "LOADTABLECOMPOSITES":
            {
                
                var removed = base.ContainsKey("Load Table Composites");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                LoadTableComposites = default;
                
                base.Remove("Load Table Composites");
                return removed;
            }
            
            case "REPLICATION MODE":
            {
                
                var removed = base.ContainsKey("Replication Mode");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ReplicationMode = default;
                
                base.Remove("Replication Mode");
                return removed;
            }
            
            case "REPLICATIONMODE":
            {
                
                var removed = base.ContainsKey("Replication Mode");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ReplicationMode = default;
                
                base.Remove("Replication Mode");
                return removed;
            }
            
            case "OPTIONS":
            {
                
                var removed = base.ContainsKey("Options");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Options = default;
                
                base.Remove("Options");
                return removed;
            }
            
            case "ARRAY NULLABILITY MODE":
            {
                
                var removed = base.ContainsKey("Array Nullability Mode");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ArrayNullabilityMode = default;
                
                base.Remove("Array Nullability Mode");
                return removed;
            }
            
            case "ARRAYNULLABILITYMODE":
            {
                
                var removed = base.ContainsKey("Array Nullability Mode");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ArrayNullabilityMode = default;
                
                base.Remove("Array Nullability Mode");
                return removed;
            }
            
            case "MULTIPLEXING":
            {
                
                var removed = base.ContainsKey("Multiplexing");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                Multiplexing = false;
                
                base.Remove("Multiplexing");
                return removed;
            }
            
            case "WRITE COALESCING BUFFER THRESHOLD BYTES":
            {
                
                var removed = base.ContainsKey("Write Coalescing Buffer Threshold Bytes");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                WriteCoalescingBufferThresholdBytes = 1000;
                
                base.Remove("Write Coalescing Buffer Threshold Bytes");
                return removed;
            }
            
            case "WRITECOALESCINGBUFFERTHRESHOLDBYTES":
            {
                
                var removed = base.ContainsKey("Write Coalescing Buffer Threshold Bytes");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                WriteCoalescingBufferThresholdBytes = 1000;
                
                base.Remove("Write Coalescing Buffer Threshold Bytes");
                return removed;
            }
            
            case "SERVER COMPATIBILITY MODE":
            {
                
                var removed = base.ContainsKey("Server Compatibility Mode");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ServerCompatibilityMode = default;
                
                base.Remove("Server Compatibility Mode");
                return removed;
            }
            
            case "SERVERCOMPATIBILITYMODE":
            {
                
                var removed = base.ContainsKey("Server Compatibility Mode");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ServerCompatibilityMode = default;
                
                base.Remove("Server Compatibility Mode");
                return removed;
            }
            
            case "CONVERT INFINITY DATETIME":
            {
                
                var removed = base.ContainsKey("Convert Infinity DateTime");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ConvertInfinityDateTime = default;
                
                base.Remove("Convert Infinity DateTime");
                return removed;
            }
            
            case "CONVERTINFINITYDATETIME":
            {
                
                var removed = base.ContainsKey("Convert Infinity DateTime");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ConvertInfinityDateTime = default;
                
                base.Remove("Convert Infinity DateTime");
                return removed;
            }
            
            case "CONTINUOUS PROCESSING":
            {
                
                var removed = base.ContainsKey("Continuous Processing");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ContinuousProcessing = default;
                
                base.Remove("Continuous Processing");
                return removed;
            }
            
            case "CONTINUOUSPROCESSING":
            {
                
                var removed = base.ContainsKey("Continuous Processing");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ContinuousProcessing = default;
                
                base.Remove("Continuous Processing");
                return removed;
            }
            
            case "BACKEND TIMEOUTS":
            {
                
                var removed = base.ContainsKey("Backend Timeouts");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                BackendTimeouts = default;
                
                base.Remove("Backend Timeouts");
                return removed;
            }
            
            case "BACKENDTIMEOUTS":
            {
                
                var removed = base.ContainsKey("Backend Timeouts");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                BackendTimeouts = default;
                
                base.Remove("Backend Timeouts");
                return removed;
            }
            
            case "PRELOAD READER":
            {
                
                var removed = base.ContainsKey("Preload Reader");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                PreloadReader = default;
                
                base.Remove("Preload Reader");
                return removed;
            }
            
            case "PRELOADREADER":
            {
                
                var removed = base.ContainsKey("Preload Reader");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                PreloadReader = default;
                
                base.Remove("Preload Reader");
                return removed;
            }
            
            case "USE EXTENDED TYPES":
            {
                
                var removed = base.ContainsKey("Use Extended Types");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                UseExtendedTypes = default;
                
                base.Remove("Use Extended Types");
                return removed;
            }
            
            case "USEEXTENDEDTYPES":
            {
                
                var removed = base.ContainsKey("Use Extended Types");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                UseExtendedTypes = default;
                
                base.Remove("Use Extended Types");
                return removed;
            }
            
            case "USE SSL STREAM":
            {
                
                var removed = base.ContainsKey("Use Ssl Stream");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                UseSslStream = default;
                
                base.Remove("Use Ssl Stream");
                return removed;
            }
            
            case "USESSLSTREAM":
            {
                
                var removed = base.ContainsKey("Use Ssl Stream");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                UseSslStream = default;
                
                base.Remove("Use Ssl Stream");
                return removed;
            }
            
            case "USE PERF COUNTERS":
            {
                
                var removed = base.ContainsKey("Use Perf Counters");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                UsePerfCounters = default;
                
                base.Remove("Use Perf Counters");
                return removed;
            }
            
            case "USEPERFCOUNTERS":
            {
                
                var removed = base.ContainsKey("Use Perf Counters");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                UsePerfCounters = default;
                
                base.Remove("Use Perf Counters");
                return removed;
            }
            
            case "CLIENT CERTIFICATE":
            {
                
                var removed = base.ContainsKey("Client Certificate");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ClientCertificate = default;
                
                base.Remove("Client Certificate");
                return removed;
            }
            
            case "CLIENTCERTIFICATE":
            {
                
                var removed = base.ContainsKey("Client Certificate");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ClientCertificate = default;
                
                base.Remove("Client Certificate");
                return removed;
            }
            
            case "CLIENT CERTIFICATE KEY":
            {
                
                var removed = base.ContainsKey("Client Certificate Key");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ClientCertificateKey = default;
                
                base.Remove("Client Certificate Key");
                return removed;
            }
            
            case "CLIENTCERTIFICATEKEY":
            {
                
                var removed = base.ContainsKey("Client Certificate Key");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                ClientCertificateKey = default;
                
                base.Remove("Client Certificate Key");
                return removed;
            }
            
            case "INCLUDE ERROR DETAILS":
            {
                
                var removed = base.ContainsKey("Include Error Details");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                IncludeErrorDetails = default;
                
                base.Remove("Include Error Details");
                return removed;
            }
            
            case "INCLUDEERRORDETAILS":
            {
                
                var removed = base.ContainsKey("Include Error Details");
                // Note that string property setters call SetValue, which itself calls base.Remove().
                
                IncludeErrorDetails = default;
                
                base.Remove("Include Error Details");
                return removed;
            }
            

            default:
                throw new KeyNotFoundException();
            }
        }

        private partial string ToCanonicalKeyword(string keyword)
            => keyword switch
            {
                
                "HOST" => "Host",
                
                "SERVER" => "Host",
                
                "PORT" => "Port",
                
                "DATABASE" => "Database",
                
                "DB" => "Database",
                
                "USERNAME" => "Username",
                
                "USER NAME" => "Username",
                
                "USERID" => "Username",
                
                "USER ID" => "Username",
                
                "UID" => "Username",
                
                "PASSWORD" => "Password",
                
                "PSW" => "Password",
                
                "PWD" => "Password",
                
                "PASSFILE" => "Passfile",
                
                "APPLICATION NAME" => "Application Name",
                
                "APPLICATIONNAME" => "Application Name",
                
                "ENLIST" => "Enlist",
                
                "SEARCH PATH" => "Search Path",
                
                "SEARCHPATH" => "Search Path",
                
                "CLIENT ENCODING" => "Client Encoding",
                
                "CLIENTENCODING" => "Client Encoding",
                
                "ENCODING" => "Encoding",
                
                "TIMEZONE" => "Timezone",
                
                "SSL MODE" => "SSL Mode",
                
                "SSLMODE" => "SSL Mode",
                
                "TRUST SERVER CERTIFICATE" => "Trust Server Certificate",
                
                "TRUSTSERVERCERTIFICATE" => "Trust Server Certificate",
                
                "SSL CERTIFICATE" => "SSL Certificate",
                
                "SSLCERTIFICATE" => "SSL Certificate",
                
                "SSL KEY" => "SSL Key",
                
                "SSLKEY" => "SSL Key",
                
                "SSL PASSWORD" => "SSL Password",
                
                "SSLPASSWORD" => "SSL Password",
                
                "ROOT CERTIFICATE" => "Root Certificate",
                
                "ROOTCERTIFICATE" => "Root Certificate",
                
                "CHECK CERTIFICATE REVOCATION" => "Check Certificate Revocation",
                
                "CHECKCERTIFICATEREVOCATION" => "Check Certificate Revocation",
                
                "INTEGRATED SECURITY" => "Integrated Security",
                
                "INTEGRATEDSECURITY" => "Integrated Security",
                
                "KERBEROS SERVICE NAME" => "Kerberos Service Name",
                
                "KERBEROSSERVICENAME" => "Kerberos Service Name",
                
                "KRBSRVNAME" => "Kerberos Service Name",
                
                "INCLUDE REALM" => "Include Realm",
                
                "INCLUDEREALM" => "Include Realm",
                
                "PERSIST SECURITY INFO" => "Persist Security Info",
                
                "PERSISTSECURITYINFO" => "Persist Security Info",
                
                "LOG PARAMETERS" => "Log Parameters",
                
                "LOGPARAMETERS" => "Log Parameters",
                
                "INCLUDE ERROR DETAIL" => "Include Error Detail",
                
                "INCLUDEERRORDETAIL" => "Include Error Detail",
                
                "POOLING" => "Pooling",
                
                "MINIMUM POOL SIZE" => "Minimum Pool Size",
                
                "MINPOOLSIZE" => "Minimum Pool Size",
                
                "MAXIMUM POOL SIZE" => "Maximum Pool Size",
                
                "MAXPOOLSIZE" => "Maximum Pool Size",
                
                "CONNECTION IDLE LIFETIME" => "Connection Idle Lifetime",
                
                "CONNECTIONIDLELIFETIME" => "Connection Idle Lifetime",
                
                "CONNECTION PRUNING INTERVAL" => "Connection Pruning Interval",
                
                "CONNECTIONPRUNINGINTERVAL" => "Connection Pruning Interval",
                
                "CONNECTION LIFETIME" => "Connection Lifetime",
                
                "CONNECTIONLIFETIME" => "Connection Lifetime",
                
                "LOAD BALANCE TIMEOUT" => "Connection Lifetime",
                
                "TIMEOUT" => "Timeout",
                
                "COMMAND TIMEOUT" => "Command Timeout",
                
                "COMMANDTIMEOUT" => "Command Timeout",
                
                "INTERNAL COMMAND TIMEOUT" => "Internal Command Timeout",
                
                "INTERNALCOMMANDTIMEOUT" => "Internal Command Timeout",
                
                "CANCELLATION TIMEOUT" => "Cancellation Timeout",
                
                "CANCELLATIONTIMEOUT" => "Cancellation Timeout",
                
                "TARGET SESSION ATTRIBUTES" => "Target Session Attributes",
                
                "TARGETSESSIONATTRIBUTES" => "Target Session Attributes",
                
                "LOAD BALANCE HOSTS" => "Load Balance Hosts",
                
                "LOADBALANCEHOSTS" => "Load Balance Hosts",
                
                "HOST RECHECK SECONDS" => "Host Recheck Seconds",
                
                "HOSTRECHECKSECONDS" => "Host Recheck Seconds",
                
                "EF TEMPLATE DATABASE" => "EF Template Database",
                
                "ENTITYTEMPLATEDATABASE" => "EF Template Database",
                
                "EF ADMIN DATABASE" => "EF Admin Database",
                
                "ENTITYADMINDATABASE" => "EF Admin Database",
                
                "KEEPALIVE" => "Keepalive",
                
                "TCP KEEPALIVE" => "TCP Keepalive",
                
                "TCPKEEPALIVE" => "TCP Keepalive",
                
                "TCP KEEPALIVE TIME" => "TCP Keepalive Time",
                
                "TCPKEEPALIVETIME" => "TCP Keepalive Time",
                
                "TCP KEEPALIVE INTERVAL" => "TCP Keepalive Interval",
                
                "TCPKEEPALIVEINTERVAL" => "TCP Keepalive Interval",
                
                "READ BUFFER SIZE" => "Read Buffer Size",
                
                "READBUFFERSIZE" => "Read Buffer Size",
                
                "WRITE BUFFER SIZE" => "Write Buffer Size",
                
                "WRITEBUFFERSIZE" => "Write Buffer Size",
                
                "SOCKET RECEIVE BUFFER SIZE" => "Socket Receive Buffer Size",
                
                "SOCKETRECEIVEBUFFERSIZE" => "Socket Receive Buffer Size",
                
                "SOCKET SEND BUFFER SIZE" => "Socket Send Buffer Size",
                
                "SOCKETSENDBUFFERSIZE" => "Socket Send Buffer Size",
                
                "MAX AUTO PREPARE" => "Max Auto Prepare",
                
                "MAXAUTOPREPARE" => "Max Auto Prepare",
                
                "AUTO PREPARE MIN USAGES" => "Auto Prepare Min Usages",
                
                "AUTOPREPAREMINUSAGES" => "Auto Prepare Min Usages",
                
                "NO RESET ON CLOSE" => "No Reset On Close",
                
                "NORESETONCLOSE" => "No Reset On Close",
                
                "LOAD TABLE COMPOSITES" => "Load Table Composites",
                
                "LOADTABLECOMPOSITES" => "Load Table Composites",
                
                "REPLICATION MODE" => "Replication Mode",
                
                "REPLICATIONMODE" => "Replication Mode",
                
                "OPTIONS" => "Options",
                
                "ARRAY NULLABILITY MODE" => "Array Nullability Mode",
                
                "ARRAYNULLABILITYMODE" => "Array Nullability Mode",
                
                "MULTIPLEXING" => "Multiplexing",
                
                "WRITE COALESCING BUFFER THRESHOLD BYTES" => "Write Coalescing Buffer Threshold Bytes",
                
                "WRITECOALESCINGBUFFERTHRESHOLDBYTES" => "Write Coalescing Buffer Threshold Bytes",
                
                "SERVER COMPATIBILITY MODE" => "Server Compatibility Mode",
                
                "SERVERCOMPATIBILITYMODE" => "Server Compatibility Mode",
                
                "CONVERT INFINITY DATETIME" => "Convert Infinity DateTime",
                
                "CONVERTINFINITYDATETIME" => "Convert Infinity DateTime",
                
                "CONTINUOUS PROCESSING" => "Continuous Processing",
                
                "CONTINUOUSPROCESSING" => "Continuous Processing",
                
                "BACKEND TIMEOUTS" => "Backend Timeouts",
                
                "BACKENDTIMEOUTS" => "Backend Timeouts",
                
                "PRELOAD READER" => "Preload Reader",
                
                "PRELOADREADER" => "Preload Reader",
                
                "USE EXTENDED TYPES" => "Use Extended Types",
                
                "USEEXTENDEDTYPES" => "Use Extended Types",
                
                "USE SSL STREAM" => "Use Ssl Stream",
                
                "USESSLSTREAM" => "Use Ssl Stream",
                
                "USE PERF COUNTERS" => "Use Perf Counters",
                
                "USEPERFCOUNTERS" => "Use Perf Counters",
                
                "CLIENT CERTIFICATE" => "Client Certificate",
                
                "CLIENTCERTIFICATE" => "Client Certificate",
                
                "CLIENT CERTIFICATE KEY" => "Client Certificate Key",
                
                "CLIENTCERTIFICATEKEY" => "Client Certificate Key",
                
                "INCLUDE ERROR DETAILS" => "Include Error Details",
                
                "INCLUDEERRORDETAILS" => "Include Error Details",
                

                _ => throw new KeyNotFoundException()
            };
    }
}
