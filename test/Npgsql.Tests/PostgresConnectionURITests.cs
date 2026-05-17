using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Npgsql.Tests;

class PostgresConnectionURIParserTests
{
    [Test]
    public void Scheme()
    {
        Assert.That(() => new PostgresConnectionURIParser("http://localhost"), Throws.ArgumentException);
        Assert.That(() => new PostgresConnectionURIParser("npgsql://localhost"), Throws.ArgumentException);
        Assert.That(() => new PostgresConnectionURIParser("postgresq://localhost"), Throws.ArgumentException);

        Assert.That(() => new PostgresConnectionURIParser("postgres://localhost"), Throws.Nothing);
        Assert.That(() => new PostgresConnectionURIParser("POSTGRES://localhost"), Throws.Nothing);
        Assert.That(() => new PostgresConnectionURIParser("postgresql://localhost"), Throws.Nothing);
        Assert.That(() => new PostgresConnectionURIParser("PostgreSQL://localhost"), Throws.Nothing);
    }

    [Test]
    public void Empty()
    {
        var parser = new PostgresConnectionURIParser("postgresql://");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parser.Host, Is.Null);
            Assert.That(parser.Port, Is.Null);
            Assert.That(parser.Username, Is.Null);
            Assert.That(parser.Password, Is.Null);
            Assert.That(parser.Database, Is.Null);
            Assert.That(parser.Parameters, Is.Empty);
        }
    }

    [Test]
    public void Host()
    {
        var parser = new PostgresConnectionURIParser("postgresql://localhost");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parser.Host, Is.EqualTo("localhost"));
            Assert.That(parser.Port, Is.Null);
            Assert.That(parser.Username, Is.Null);
            Assert.That(parser.Password, Is.Null);
            Assert.That(parser.Database, Is.Null);
            Assert.That(parser.Parameters, Is.Empty);
        }
    }

    [Test]
    public void Host_and_port()
    {
        var parser = new PostgresConnectionURIParser("postgresql://localhost:5433");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parser.Host, Is.EqualTo("localhost"));
            Assert.That(parser.Port, Is.EqualTo(5433));
            Assert.That(parser.Username, Is.Null);
            Assert.That(parser.Password, Is.Null);
            Assert.That(parser.Database, Is.Null);
            Assert.That(parser.Parameters, Is.Empty);
        }
    }

    [Test]
    public void Host_and_db()
    {
        var parser = new PostgresConnectionURIParser("postgresql://localhost/mydb");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parser.Host, Is.EqualTo("localhost"));
            Assert.That(parser.Port, Is.Null);
            Assert.That(parser.Username, Is.Null);
            Assert.That(parser.Password, Is.Null);
            Assert.That(parser.Database, Is.EqualTo("mydb"));
            Assert.That(parser.Parameters, Is.Empty);
        }
    }

    [Test]
    public void User()
    {
        var parser = new PostgresConnectionURIParser("postgresql://user@localhost");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parser.Host, Is.EqualTo("localhost"));
            Assert.That(parser.Port, Is.Null);
            Assert.That(parser.Username, Is.EqualTo("user"));
            Assert.That(parser.Password, Is.Null);
            Assert.That(parser.Database, Is.Null);
            Assert.That(parser.Parameters, Is.Empty);
        }
    }

    [Test]
    public void User_with_password()
    {
        var parser = new PostgresConnectionURIParser("postgresql://user:secret@localhost");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parser.Host, Is.EqualTo("localhost"));
            Assert.That(parser.Port, Is.Null);
            Assert.That(parser.Username, Is.EqualTo("user"));
            Assert.That(parser.Password, Is.EqualTo("secret"));
            Assert.That(parser.Database, Is.Null);
            Assert.That(parser.Parameters, Is.Empty);
        }
    }

    [Test]
    public void Parameters()
    {
        var parser = new PostgresConnectionURIParser("postgresql://other@localhost/otherdb?connect_timeout=10&application_name=myapp");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parser.Host, Is.EqualTo("localhost"));
            Assert.That(parser.Port, Is.Null);
            Assert.That(parser.Username, Is.EqualTo("other"));
            Assert.That(parser.Password, Is.Null);
            Assert.That(parser.Database, Is.EqualTo("otherdb"));
            Assert.That(parser.Parameters, Is.EquivalentTo([
                new KeyValuePair<string, string>("connect_timeout", "10"),
                new KeyValuePair<string, string>("application_name", "myapp")
            ]));
        }
    }

    [Test]
    public void Multiple_hosts()
    {
        var parser = new PostgresConnectionURIParser("postgresql://host1:123,host2:456/somedb");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(parser.Host, Is.EqualTo("host1:123,host2:456"));
            Assert.That(parser.Port, Is.Null);
            Assert.That(parser.Username, Is.Null);
            Assert.That(parser.Password, Is.Null);
            Assert.That(parser.Database, Is.EqualTo("somedb"));
            Assert.That(parser.Parameters, Is.Empty);
        }
    }
}

class PostgresConnectionURITests
{
    [Test]
    public void Convert_to_string()
    {
        var uri = "postgres://user@pghost/mydb?connect_timeout=10&application_name=myapp";
        var connUri = new PostgresConnectionURI(uri);

        Assert.That(connUri.ToString(), Is.EqualTo(uri));
    }

    [Test]
    public void Convert_to_ConnectionStringBuilder()
    {
        var uri = "postgres://user:secret@pghost:4321/mydb?channel_binding=prefer&require_auth=password,gss,sspi,scram-sha-256";
        var connUri = new PostgresConnectionURI(uri);
        var builder = connUri.ToConnectionStringBuilder();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(builder.Host, Is.EqualTo("pghost"));
            Assert.That(builder.Port, Is.EqualTo(4321));
            Assert.That(builder.Username, Is.EqualTo("user"));
            Assert.That(builder.Password, Is.EqualTo("secret"));
            Assert.That(builder.Database, Is.EqualTo("mydb"));
            Assert.That(builder.ChannelBinding, Is.EqualTo(ChannelBinding.Prefer));
            Assert.That(builder.RequireAuth, Is.EqualTo("Password,GSS,SSPI,ScramSHA256"));
        }
    }

    [Test]
    public void Parameter_overwrite()
    {
        var uri = "postgres://user:pass@pghost:4321/mydb?host=localhost&port=6543&dbname=newdb&user=foo&password=secret";

        Assert.That(() => new PostgresConnectionURI(uri), Throws.ArgumentException);

        var connUri = new PostgresConnectionURI(uri,
            new PostgresConnectionURIParseOptions() { ThrowsOnParameterOverwrite = false });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(connUri.Host, Is.EqualTo("localhost"));
            Assert.That(connUri.Port, Is.EqualTo(6543));
            Assert.That(connUri.Username, Is.EqualTo("foo"));
            Assert.That(connUri.Password, Is.EqualTo("secret"));
            Assert.That(connUri.Database, Is.EqualTo("newdb"));
        }
    }

    [Test]
    public void Unexpected_parameters()
    {
        var uri = "postgres://localhost/mydb?unexpected=value&another=123";

        Assert.That(() => new PostgresConnectionURI(uri),
            Throws.ArgumentException.And.Message.Contains("unexpected, another"));

        var connUri = new PostgresConnectionURI(uri,
            new PostgresConnectionURIParseOptions() { ThrowsOnUnsupportedKeys = false });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(connUri.Host, Is.EqualTo("localhost"));
            Assert.That(connUri.Database, Is.EqualTo("mydb"));
        }
    }

    [Test]
    public void Host()
    {
        var uri = "postgresql:///db?";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(new PostgresConnectionURI(uri + "host=pghost").Host, Is.EqualTo("pghost"));
            Assert.That(new PostgresConnectionURI(uri + "hostaddr=127.0.0.1").Host, Is.EqualTo("127.0.0.1"));
        }
    }

    [Test]
    public void Multiple_ports()
    {
        var uri = "postgresql:///db?host=pg1,pg2,pg3&port=5432,6543,7654";

        Assert.That(() => new PostgresConnectionURI(uri), Throws.ArgumentException);

        var connUri = new PostgresConnectionURI(uri,
            new PostgresConnectionURIParseOptions() { ThrowsOnMultiplePorts = false });
        Assert.That(connUri.Port, Is.Null);
    }

    [Test]
    public void LoadBalanceHosts()
    {
        var uri = "postgresql://localhost/db?load_balance_hosts=";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(new PostgresConnectionURI(uri + "disable").LoadBalanceHosts, Is.False);
            Assert.That(new PostgresConnectionURI(uri + "random").LoadBalanceHosts, Is.True);
            Assert.That(() => new PostgresConnectionURI(uri + "invalid"), Throws.ArgumentException);
        }
    }

    [Test]
    public void RequireAuth()
    {
        var uri = "postgresql://localhost/db?require_auth=";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(new PostgresConnectionURI(uri + "password").RequireAuth, Is.EqualTo("Password"));
            Assert.That(new PostgresConnectionURI(uri + "gss,sspi,scram-sha-256").RequireAuth, Is.EqualTo("GSS,SSPI,ScramSHA256"));
            Assert.That(new PostgresConnectionURI(uri + "!md5,!password").RequireAuth, Is.EqualTo("!MD5,!Password"));
        }
    }

    [Test]
    public void SslMode()
    {
        var uri = "postgresql://localhost/db?sslmode=";

        using (Assert.EnterMultipleScope())
        {
            Assert.That(new PostgresConnectionURI(uri + "disable").SslMode, Is.EqualTo(Npgsql.SslMode.Disable));
            Assert.That(new PostgresConnectionURI(uri + "verify-ca").SslMode, Is.EqualTo(Npgsql.SslMode.VerifyCA));
        }
    }

    [Test]
    public void Options()
    {
        var uri = "postgresql://localhost/db?options=-c%20search_path%3Dmyschema%2Cpublic%20-c%20client_min_messages%3DDEBUG3";

        Assert.That(new PostgresConnectionURI(uri).Options,
            Is.EquivalentTo("-c search_path=myschema,public -c client_min_messages=DEBUG3"));
    }

    [Test]
    public void Invalid_int()
    {
        var uri = "postgresql://localhost/db?connect_timeout=invalid";

        Assert.That(() => new PostgresConnectionURI(uri), Throws.ArgumentException);
    }

    [Test]
    public void Invalid_bool()
    {
        var uri = "postgresql://localhost/db?keepalives=123";

        Assert.That(() => new PostgresConnectionURI(uri), Throws.ArgumentException);
    }

    [Test]
    public void Bindable_properties_must_exist_on_NpgsqlConnectionStringBuilder()
    {
        var props = PostgresConnectionURI.EnumerateConnectionParameters(attr => attr.Bindable);

        foreach (var prop in props)
        {
            using (Assert.EnterMultipleScope())
            {
                var destType = typeof(NpgsqlConnectionStringBuilder).GetProperty(prop.Name);
                var srcType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                Assert.That(destType, Is.Not.Null,
                            $"Property {prop.Name} must exist in NpgsqlConnectionStringBuilder");
                Assert.That(srcType, Is.EqualTo(destType?.PropertyType),
                    $"{prop.Name} type must be same as NpgsqlConnectionStringBuilder.{prop.Name}");
            }
        }
    }

    [Test]
    public void NonBindable_properties_must_not_exist_on_NpgsqlConnectionStringBuilder()
    {
        var props = PostgresConnectionURI.EnumerateConnectionParameters(attr => !attr.Bindable);

        foreach (var prop in props)
        {
            Assert.That(typeof(NpgsqlConnectionStringBuilder).GetProperty(prop.Name), Is.Null,
                $"Property {prop.Name} must not exist on NpgsqlConnectionStringBuilder");
        }
    }

    [Test]
    public void ConnectionParameters_must_be_internal()
    {
        var props = PostgresConnectionURI.EnumerateConnectionParameters(_ => true);

        foreach (var prop in props)
        {
            using (Assert.EnterMultipleScope())
            {
                Assert.That(prop.GetMethod?.IsAssembly, Is.True, $"Property {prop.Name} must have an internal getter");
                Assert.That(prop.SetMethod?.IsPublic, Is.False, $"Property {prop.Name} must not have a public setter");
            }
        }
    }
}
