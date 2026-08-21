using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DotNet.Testcontainers.Configurations;
using Testcontainers.PostgreSql;

namespace Npgsql.Tests.Support;

static class PostgreSqlContainerExtensions
{
    public static async Task EnableSslAsync(this PostgreSqlContainer container)
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

        await container.StopAsync();
        await container.StartAsync();
    }
}
