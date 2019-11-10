using System;
using NUnit.Framework;

namespace Npgsql.Tests
{
    [Category("ConnectionUri")]
    public class ConnectionUriTests : TestBase
    {
        [Test]
        public void InvalidArgument()
        {
            Assert.That(() => new NpgsqlConnectionUri(null!),
                Throws.InstanceOf(typeof(ArgumentNullException)));

            Assert.That(() => new NpgsqlConnectionUri(""),
                Throws.InstanceOf(typeof(UriFormatException)));
        }

        [Test]
        public void InvalidConnectionUri()
        {
            Assert.That(() => new NpgsqlConnectionUri("http://localhost/mydb"),
                Throws.InstanceOf(typeof(UriFormatException)));

            Assert.That(() => new NpgsqlConnectionUri("postgresql://host1,host2/mydb"),
                Throws.InstanceOf(typeof(NotSupportedException)));

            Assert.That(() => new NpgsqlConnectionUri("postgresql://host:port"),
                Throws.InstanceOf(typeof(UriFormatException)));

            Assert.That(() => new NpgsqlConnectionUri("postgresql://testhost/mydb?port=a432").ToSettings(),
                Throws.InstanceOf(typeof(UriFormatException)));

            Assert.That(() => new NpgsqlConnectionUri("postgresql:///mydb?host=host1,host2").ToSettings(),
                Throws.InstanceOf(typeof(NotSupportedException)));

            Assert.That(() => new NpgsqlConnectionUri("postgresql:///mydb?port=4321,1234&host=host1,host2").ToSettings(),
                Throws.InstanceOf(typeof(NotSupportedException)));
        }

        [Test]
        public void ValidDbNameWithSlash()
        {
            var uri = new NpgsqlConnectionUri("postgresql:///invalid/dbname");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.EqualTo("invalid/dbname"));
        }

        [Test]
        public void ValidHost()
        {
            var uri = new NpgsqlConnectionUri("postgresql://host:1234/mydb?application_name=myapp@npgsql.org");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.EqualTo("mydb"));
            Assert.That(settings.ApplicationName, Is.EqualTo("myapp@npgsql.org"));
        }

        [Test]
        public void ValidHostIpv6()
        {
            var uri = new NpgsqlConnectionUri("postgresql://[2001:db8::1234]/database");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("2001:db8::1234"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.EqualTo("database"));
        }

        [Test]
        public void ValidHostUnixDomainSocketDirectory1()
        {
            var uri = new NpgsqlConnectionUri("postgresql://%2Fvar%2Flib%2Fpostgresql/dbname");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("/var/lib/postgresql"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.EqualTo("dbname"));
        }

        [Test]
        public void ValidHostUnixDomainSocketDirectory2()
        {
            var uri = new NpgsqlConnectionUri("postgresql://?host=/var/lib/postgresql");
            var settings = uri.ToSettings();

            Assert.That(settings.Host, Is.EqualTo("/var/lib/postgresql"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart1()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user@");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart2()
        {
            var uri = new NpgsqlConnectionUri("postgresql://:pwd@");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart3()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart4()
        {
            var uri = new NpgsqlConnectionUri("postgresql://host");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart5()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user@host");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart6()
        {
            var uri = new NpgsqlConnectionUri("postgresql://:pwd@host");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart7()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart8()
        {
            var uri = new NpgsqlConnectionUri("postgresql://:1234");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart9()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user@:1234");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart10()
        {
            var uri = new NpgsqlConnectionUri("postgresql://:pwd@:1234");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart11()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@:1234");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart12()
        {
            var uri = new NpgsqlConnectionUri("postgresql://host:1234");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart13()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user@host:1234");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart14()
        {
            var uri = new NpgsqlConnectionUri("postgresql://:pwd@host:1234");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidAuthorityPart15()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidConnectionUri1()
        {
            var uri = new NpgsqlConnectionUri("postgresql://");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidConnectionUri2()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidConnectionUri3()
        {
            var uri = new NpgsqlConnectionUri("postgresql:///path");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.EqualTo("path"));
        }

        [Test]
        public void ValidConnectionUri4()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/path");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.EqualTo("path"));
        }

        [Test]
        public void ValidConnectionUri5()
        {
            var uri = new NpgsqlConnectionUri("postgresql://?application_name=myapp&ssl=require");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidConnectionUri6()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234?application_name=myapp&ssl=require");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidConnectionUri7()
        {
            var uri = new NpgsqlConnectionUri("postgresql:///path?application_name=myapp&ssl=require");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.EqualTo("path"));
        }

        [Test]
        public void ValidConnectionUri8()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/path?application_name=myapp&ssl=require");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.EqualTo("path"));
        }

        [Test]
        public void ValidConnectionUri9()
        {
            var uri = new NpgsqlConnectionUri("postgresql://#frag");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidConnectionUri10()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234#frag");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidConnectionUri11()
        {
            var uri = new NpgsqlConnectionUri("postgresql:///path#frag");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.EqualTo("path"));
        }

        [Test]
        public void ValidConnectionUri12()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/path#frag");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.EqualTo("path"));
        }

        [Test]
        public void ValidConnectionUri13()
        {
            var uri = new NpgsqlConnectionUri("postgresql://?application_name=myapp&ssl=require#frag");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidConnectionUri14()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234?application_name=myapp&ssl=require#frag");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.Null);
        }

        [Test]
        public void ValidConnectionUri15()
        {
            var uri = new NpgsqlConnectionUri("postgresql:///path?application_name=myapp&ssl=require#frag");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("localhost"));
            Assert.That(settings.Port, Is.EqualTo(5432));
            Assert.That(settings.Username, Is.Null);
            Assert.That(settings.Password, Is.Null);
            Assert.That(settings.Database, Is.EqualTo("path"));
        }

        [Test]
        public void ValidConnectionUri16()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/path?application_name=myapp&ssl=require#frag");
            var settings = uri.ToSettings();
            Assert.That(settings.Host, Is.EqualTo("host"));
            Assert.That(settings.Port, Is.EqualTo(1234));
            Assert.That(settings.Username, Is.EqualTo("user"));
            Assert.That(settings.Password, Is.EqualTo("pwd"));
            Assert.That(settings.Database, Is.EqualTo("path"));
        }

        [Test]
        public void ParameterDefaults()
        {
            var connUri = new NpgsqlConnection("postgresql://");
            var connStr = new NpgsqlConnection("");


            Assert.That(connUri.Settings.Host, Is.EqualTo("localhost"));
            Assert.That(connUri.Settings.Port, Is.EqualTo(connStr.Settings.Port));
            Assert.That(connUri.Settings.Username, Is.EqualTo(connStr.Settings.Username));
            Assert.That(connUri.Settings.Password, Is.EqualTo(connStr.Settings.Password));
            Assert.That(connUri.Settings.Database, Is.EqualTo(connStr.Settings.Database));

            Assert.That(connUri.Settings.Passfile, Is.EqualTo(connStr.Settings.Passfile)); // TODO
            Assert.That(connUri.Settings.Enlist, Is.EqualTo(connStr.Settings.Enlist));

            Assert.That(connUri.Settings.SslMode, Is.EqualTo(SslMode.Prefer));
            Assert.That(connUri.Settings.TrustServerCertificate, Is.EqualTo(connStr.Settings.TrustServerCertificate));
            Assert.That(connUri.Settings.ClientCertificate, Is.EqualTo(connStr.Settings.ClientCertificate));
            Assert.That(connUri.Settings.CheckCertificateRevocation, Is.EqualTo(connStr.Settings.CheckCertificateRevocation));
            Assert.That(connUri.Settings.IntegratedSecurity, Is.EqualTo(connStr.Settings.IntegratedSecurity));
            Assert.That(connUri.Settings.KerberosServiceName, Is.EqualTo(connStr.Settings.KerberosServiceName));
            Assert.That(connUri.Settings.IncludeRealm, Is.EqualTo(connStr.Settings.IncludeRealm));
            Assert.That(connUri.Settings.PersistSecurityInfo, Is.EqualTo(connStr.Settings.PersistSecurityInfo));

            Assert.That(connUri.Settings.Pooling, Is.EqualTo(connStr.Settings.Pooling));
            Assert.That(connUri.Settings.MinPoolSize, Is.EqualTo(connStr.Settings.MinPoolSize));
            Assert.That(connUri.Settings.MaxPoolSize, Is.EqualTo(connStr.Settings.MaxPoolSize));
            Assert.That(connUri.Settings.ConnectionIdleLifetime, Is.EqualTo(connStr.Settings.ConnectionIdleLifetime));
            Assert.That(connUri.Settings.ConnectionPruningInterval, Is.EqualTo(connStr.Settings.ConnectionPruningInterval));

            Assert.That(connUri.Settings.Timeout, Is.EqualTo(connStr.Settings.Timeout));
            Assert.That(connUri.Settings.CommandTimeout, Is.EqualTo(connStr.Settings.CommandTimeout));
            Assert.That(connUri.Settings.InternalCommandTimeout, Is.EqualTo(connStr.Settings.InternalCommandTimeout));

            Assert.That(connUri.Settings.EntityTemplateDatabase, Is.EqualTo(connStr.Settings.EntityTemplateDatabase));
            Assert.That(connUri.Settings.EntityAdminDatabase, Is.EqualTo(connStr.Settings.EntityAdminDatabase));

            Assert.That(connUri.Settings.KeepAlive, Is.EqualTo(connStr.Settings.KeepAlive));
            Assert.That(connUri.Settings.TcpKeepAlive, Is.True);
            Assert.That(connUri.Settings.TcpKeepAliveTime, Is.EqualTo(connStr.Settings.TcpKeepAliveTime)); // TODO
            Assert.That(connUri.Settings.TcpKeepAliveInterval, Is.EqualTo(connStr.Settings.TcpKeepAliveInterval)); // TODO
            Assert.That(connUri.Settings.ReadBufferSize, Is.EqualTo(connStr.Settings.ReadBufferSize));
            Assert.That(connUri.Settings.WriteBufferSize, Is.EqualTo(connStr.Settings.WriteBufferSize));
            Assert.That(connUri.Settings.SocketReceiveBufferSize, Is.EqualTo(connStr.Settings.SocketReceiveBufferSize));
            Assert.That(connUri.Settings.SocketSendBufferSize, Is.EqualTo(connStr.Settings.SocketSendBufferSize));
            Assert.That(connUri.Settings.MaxAutoPrepare, Is.EqualTo(connStr.Settings.MaxAutoPrepare));
            Assert.That(connUri.Settings.AutoPrepareMinUsages, Is.EqualTo(connStr.Settings.AutoPrepareMinUsages));
            Assert.That(connUri.Settings.UsePerfCounters, Is.EqualTo(connStr.Settings.UsePerfCounters));
            Assert.That(connUri.Settings.NoResetOnClose, Is.EqualTo(connStr.Settings.NoResetOnClose));
            Assert.That(connUri.Settings.LoadTableComposites, Is.EqualTo(connStr.Settings.LoadTableComposites));

            Assert.That(connUri.Settings.ServerCompatibilityMode, Is.EqualTo(connStr.Settings.ServerCompatibilityMode));
            Assert.That(connUri.Settings.ConvertInfinityDateTime, Is.EqualTo(connStr.Settings.ConvertInfinityDateTime));
        }

        [Test]
        public void ValidParameters1()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/dbname"
                + "?host=otherhost"
                + "&port=9876"
                + "&dbname=otherdb"
                + "&user=otheruser"
                + "&password=asdf");
            var settings = uri.ToSettings();

            Assert.That(settings.Host, Is.EqualTo("otherhost"));
            Assert.That(settings.Port, Is.EqualTo(9876));
            Assert.That(settings.Username, Is.EqualTo("otheruser"));
            Assert.That(settings.Password, Is.EqualTo("asdf"));
            Assert.That(settings.Database, Is.EqualTo("otherdb"));
        }

        [Test]
        public void ValidParameters2()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/dbname"
                + "?passfile=/user/.pgpass"
                + "&application_name=myapp"
                + "&enlist=1"
                + "&search_path=namespaceA"
                + "&client_encoding=SJIS"
                + "&encoding=euc-jp"
                + "&timezone=NZT");
            var settings = uri.ToSettings();

            Assert.That(settings.Passfile, Is.EqualTo("/user/.pgpass"));
            Assert.That(settings.ApplicationName, Is.EqualTo("myapp"));
            Assert.That(settings.Enlist, Is.True);
            Assert.That(settings.SearchPath, Is.EqualTo("namespaceA"));
            Assert.That(settings.ClientEncoding, Is.EqualTo("SJIS"));
            Assert.That(settings.Encoding, Is.EqualTo("euc-jp"));
            Assert.That(settings.Timezone, Is.EqualTo("NZT"));
        }

        [Test]
        public void ValidParameters3()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/dbname"
                + "?sslmode=require"
                + "&trust_server_certificate=1"
                + "&sslcert=client.cert"
                + "&check_certificate_revocation=1"
                + (Type.GetType("Mono.Runtime") == null ? "&integrated_security=1" : "")
                + "&krbsrvname=MyKerberosService"
                + "&include_realm=1"
                + "&persist_security_info=1");
            var settings = uri.ToSettings();

            Assert.That(settings.SslMode, Is.EqualTo(SslMode.Require));
            Assert.That(settings.TrustServerCertificate, Is.True);
            Assert.That(settings.ClientCertificate, Is.EqualTo("client.cert"));
            Assert.That(settings.CheckCertificateRevocation, Is.True);
            if (Type.GetType("Mono.Runtime") == null)
                Assert.That(settings.IntegratedSecurity, Is.True);
            else
                Assert.That(settings.IntegratedSecurity, Is.False);
            Assert.That(settings.KerberosServiceName, Is.EqualTo("MyKerberosService"));
            Assert.That(settings.IncludeRealm, Is.True);
            Assert.That(settings.PersistSecurityInfo, Is.True);
        }

        [Test]
        public void ValidParameters4()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/dbname"
                + "?pooling=0"
                + "&minimum_pool_size=10"
                + "&maximum_pool_size=20"
                + "&connection_idle_lifetime=60"
                + "&connection_pruning_interval=20");
            var settings = uri.ToSettings();

            Assert.That(settings.Pooling, Is.False);
            Assert.That(settings.MinPoolSize, Is.EqualTo(10));
            Assert.That(settings.MaxPoolSize, Is.EqualTo(20));
            Assert.That(settings.ConnectionIdleLifetime, Is.EqualTo(60));
            Assert.That(settings.ConnectionPruningInterval, Is.EqualTo(20));
        }

        [Test]
        public void ValidParameters5()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/dbname"
                + "?connect_timeout=5"
                + "&command_timeout=10"
                + "&internal_command_timeout=20");
            var settings = uri.ToSettings();

            Assert.That(settings.Timeout, Is.EqualTo(5));
            Assert.That(settings.CommandTimeout, Is.EqualTo(10));
            Assert.That(settings.InternalCommandTimeout, Is.EqualTo(20));
        }

        [Test]
        public void ValidParameters6()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/dbname"
                + "?ef_template_database=template2"
                + "&ef_admin_database=dba");
            var settings = uri.ToSettings();

            Assert.That(settings.EntityTemplateDatabase, Is.EqualTo("template2"));
            Assert.That(settings.EntityAdminDatabase, Is.EqualTo("dba"));
        }

        [Test]
        public void ValidParameters7()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/dbname"
                + "?query_keepalives_time=5"
                + "&keepalives=0"
                + "&keepalives_time=5"
                + "&keepalives_interval=1"
                + "&read_buffer_size=4096"
                + "&write_buffer_size=2048"
                + "&socket_receive_buffer_size=1024"
                + "&socket_send_buffer_size=512"
                + "&max_auto_prepare=10"
                + "&auto_prepare_min_usages=2"
                + "&use_perf_counters=1"
                + "&no_reset_on_close=1"
                + "&load_table_composites=1");
            var settings = uri.ToSettings();

            Assert.That(settings.KeepAlive, Is.EqualTo(5));
            Assert.That(settings.TcpKeepAlive, Is.False);
            Assert.That(settings.TcpKeepAliveTime, Is.EqualTo(5000));
            Assert.That(settings.TcpKeepAliveInterval, Is.EqualTo(1000));
            Assert.That(settings.ReadBufferSize, Is.EqualTo(4096));
            Assert.That(settings.WriteBufferSize, Is.EqualTo(2048));
            Assert.That(settings.SocketReceiveBufferSize, Is.EqualTo(1024));
            Assert.That(settings.SocketSendBufferSize, Is.EqualTo(512));
            Assert.That(settings.MaxAutoPrepare, Is.EqualTo(10));
            Assert.That(settings.AutoPrepareMinUsages, Is.EqualTo(2));
            Assert.That(settings.UsePerfCounters, Is.True);
            Assert.That(settings.NoResetOnClose, Is.True);
            Assert.That(settings.LoadTableComposites, Is.True);
        }

        [Test]
        public void ValidParameters8()
        {
            var uri = new NpgsqlConnectionUri("postgresql://user:pwd@host:1234/dbname"
                + "?server_compatibility_mode=NoTypeLoading"
                + "&convert_infinity_datetime=1");
            var settings = uri.ToSettings();

            Assert.That(settings.ServerCompatibilityMode, Is.EqualTo(ServerCompatibilityMode.NoTypeLoading));
            Assert.That(settings.ConvertInfinityDateTime, Is.True);
        }

        [Test]
        public void InvalidParameters()
        {
            Assert.That(() => new NpgsqlConnectionUri("postgresql://?host").ToSettings(),
                Throws.InstanceOf(typeof(UriFormatException)));
            Assert.That(() => new NpgsqlConnectionUri("postgresql://?options=asdf").ToSettings(),
                Throws.InstanceOf(typeof(NotSupportedException)));
        }
    }
}
