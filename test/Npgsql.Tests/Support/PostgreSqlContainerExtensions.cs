using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DotNet.Testcontainers.Configurations;
using Npgsql.Tests.Replication;
using Testcontainers.PostgreSql;

namespace Npgsql.Tests.Support;

static class PostgreSqlContainerExtensions
{
    /// <summary>
    /// Configures the container for maximum test compatibility.
    /// <list type="bullet">
    /// <item>Enable SSL for <see cref="SecurityTests"/></item>
    /// <item>Configure wal_level to <c>logical</c> and max_wal_senders to <c>50</c> for <see cref="SafeReplicationTestBase{T}"/></item>
    /// <item>Configure max_prepared_transactions to <c>100</c> for <see cref="DistributedTransactionTests"/></item>
    /// <item>Configure logical_decoding_work_mem to <c>64kB</c> for <see cref="PgOutputReplicationTests"/> in streaming mode</item>
    /// <item>Configure <c>host replication all all scram-sha-256</c> in <c>pg_hba.conf</c> for replication tests</item>
    /// </list>
    /// </summary>
    public static async Task ConfigureAsync(this PostgreSqlContainer container)
    {
        var now = DateTimeOffset.UtcNow;
        const UnixFileModes certMode = UnixFileModes.UserRead | UnixFileModes.UserWrite | UnixFileModes.GroupRead | UnixFileModes.OtherRead;
        const UnixFileModes keyMode = UnixFileModes.UserRead | UnixFileModes.UserWrite;
        const string sslCertPath = "/var/lib/postgresql/server.crt";
        const string sslKeyPath = "/var/lib/postgresql/server.key";
        const string configureSsl = $"""
                                     ALTER SYSTEM SET ssl = 'on';
                                     ALTER SYSTEM SET ssl_cert_file = '{sslCertPath}';
                                     ALTER SYSTEM SET ssl_key_file = '{sslKeyPath}';
                                     ALTER SYSTEM SET wal_level = 'logical';
                                     ALTER SYSTEM SET max_wal_senders = '50';
                                     ALTER SYSTEM SET max_prepared_transactions = '100';
                                     ALTER SYSTEM SET logical_decoding_work_mem = '64kB';
                                     """;

        var uid = uint.Parse((await container.ExecAsync(["id", "-u", "postgres"])).Stdout);
        var gid = uint.Parse((await container.ExecAsync(["id", "-g", "postgres"])).Stdout);

        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest(new X500DistinguishedName("CN=localhost"), rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var certificate = request.CreateSelfSigned(notBefore: now, notAfter: now.AddDays(1));
        var certPem = certificate.ExportCertificatePem();
        var keyPem = rsa.ExportPkcs8PrivateKeyPem();

        await container.CopyAsync(Encoding.Default.GetBytes(certPem), sslCertPath, uid, gid, certMode);
        await container.CopyAsync(Encoding.Default.GetBytes(keyPem), sslKeyPath, uid, gid, keyMode);

        await container.ExecScriptAsync(configureSsl);
        await container.ExecAsync(["sh", "-c", """
                                               echo 'host replication all all scram-sha-256' >> "$PGDATA/pg_hba.conf"
                                               """]);

        await container.StopAsync();
        await container.StartAsync();
    }
}
