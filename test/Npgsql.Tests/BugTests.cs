using Npgsql.BackendMessages;
using Npgsql.Tests.Support;
using Npgsql.TypeMapping;
using NpgsqlTypes;
using NUnit.Framework;
using System;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests;

public class BugTests : TestBase
{
    #region Sequential reader bugs

    [Test, Description("In sequential access, performing a null check on a non-first field would check the first field")]
    public void SequentialNullCheckOnNonFirstField()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT 'X', NULL", conn);
        using var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
        dr.Read();
        Assert.That(dr.IsDBNull(1), Is.True);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1034")]
    public void SequentialSkipOverFirstRow()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn);
        using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
        Assert.That(reader.NextResult(), Is.True);
        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetInt32(0), Is.EqualTo(2));
    }

    [Test]
    public void SequentialConsumeWithNull()
    {
        using var conn = OpenConnection();
        using var command = new NpgsqlCommand("SELECT 1, NULL", conn);
        using var reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
        reader.Read();
    }

    #endregion

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1210")]
    public void Many_parameters_with_mixed_FormatCode()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        var sb = new StringBuilder("SELECT @text_param");
        cmd.Parameters.AddWithValue("@text_param", "some_text");
        for (var i = 0; i < conn.Settings.WriteBufferSize; i++)
        {
            var paramName = $"@binary_param{i}";
            sb.Append(',');
            sb.Append(paramName);
            cmd.Parameters.AddWithValue(paramName, 8);
        }
        cmd.CommandText = sb.ToString();

        var ex = Assert.Throws<PostgresException>(() => cmd.ExecuteNonQuery())!;
        Assert.That(ex.SqlState, Is.EqualTo(PostgresErrorCodes.ProgramLimitExceeded)
            .Or.EqualTo(PostgresErrorCodes.TooManyColumns)); // PostgreSQL 14.5, 13.8, 12.12, 11.17 and 10.22 changed the returned error
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1238")]
    public void Record_with_non_int_field()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT ('one'::TEXT, 2)", conn);
        using var reader = cmd.ExecuteReader();
        reader.Read();
        var record = reader.GetFieldValue<object[]>(0);
        Assert.That(record[0], Is.EqualTo("one"));
        Assert.That(record[1], Is.EqualTo(2));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1450")]
    public void Bug1450()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "CREATE TEMP TABLE a (a1 int); CREATE TEMP TABLE b (b1 int);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "CREATE TEMP TABLE c (c1 int);";
        cmd.ExecuteNonQuery();
    }

    [Test]
    public async Task Bug1645()
    {
        await using var conn = await OpenConnectionAsync();
        var tableName = await CreateTempTable(conn, "field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER");
        Assert.That(() =>
            {
                using var writer = conn.BeginBinaryImport($"COPY {tableName} (field_text, field_int4) FROM STDIN BINARY");
                writer.StartRow();
                writer.Write("foo");
                writer.Write(8);

                writer.StartRow();
                throw new InvalidOperationException("Catch me outside the using statement if you can!");
            }, Throws.Exception
                .TypeOf<InvalidOperationException>()
                .With.Property(nameof(InvalidOperationException.Message)).EqualTo("Catch me outside the using statement if you can!")
        );
        Assert.That(await conn.ExecuteScalarAsync($"SELECT COUNT(*) FROM {tableName}"), Is.Zero);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3600")]
    public async Task Bug3600()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            CommandTimeout = 1,
        };
        await using var postmasterMock = PgPostmasterMock.Start(csb.ConnectionString);
        await using var conn = await OpenConnectionAsync(postmasterMock.ConnectionString);
        var serverMock = await postmasterMock.WaitForServerConnection();
        await serverMock
            .WriteCopyInResponse()
            .FlushAsync();
        var ex = Assert.ThrowsAsync<NpgsqlException>(async () =>
        {
            await using var importer = await conn.BeginTextImportAsync($"COPY SomeTable (field_text, field_int4) FROM STDIN");
        });
        Assert.That(ex!.InnerException, Is.TypeOf<TimeoutException>());
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1497")]
    public async Task Bug1497()
    {
        await using var conn = await OpenConnectionAsync();
        var tableName = await CreateTempTable(conn, "id INT4");
        conn.ExecuteNonQuery($"INSERT INTO {tableName} (id) VALUES (NULL)");
        await using var cmd = new NpgsqlCommand($"SELECT * FROM {tableName}", conn);
        await using var reader = await cmd.ExecuteReaderAsync();
        var dt = new DataTable();
        dt.Load(reader);
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1558")]
    public void Bug1558()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Pooling = false,
            Enlist = true
        };
        using var tx = new TransactionScope();
        using var conn = new NpgsqlConnection(csb.ToString());
        conn.Open();
    }

    [Test]
    public void Bug1695()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Pooling = false,
            MaxAutoPrepare = 10,
            AutoPrepareMinUsages = 1
        };
        using var conn = OpenConnection(csb);
        using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn))
        using (var reader = cmd.ExecuteReader())
        {
            reader.Read();
            // Both statements should get prepared. However, purposefully skip processing the
            // second resultset and make sure the second statement got prepared correctly.
        }
        Assert.That(conn.ExecuteScalar("SELECT 2"), Is.EqualTo(2));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1700")]
    public void Bug1700()
    {
        Assert.That(() =>
        {
            using var conn = OpenConnection();
            using var tx = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT 1";
            var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                // Simulate exception whilst processing the data reader...
                throw new InvalidOperationException("Some problem parsing the returned data");

                // As this exception unwinds the stack, it calls Dispose on the NpgsqlTransaction
                // which then throws a NpgsqlOperationInProgressException as it tries to rollback
                // the transaction. This hides the underlying cause of the problem (in this case
                // our InvalidOperationException exception)
            }

            // Note, we never get here
            tx.Commit();
        }, Throws.InvalidOperationException.With.Message.EqualTo("Some problem parsing the returned data"));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1964")]
    public void Bug1964()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("INVALID SQL", conn);
        cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Direction = ParameterDirection.Output });
        Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception.TypeOf<PostgresException>()
            .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.SyntaxError));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1986")]
    public void Bug1986()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT 'hello', 'goodbye'", conn);
        using var reader = cmd.ExecuteReader();
        reader.Read();
        using (var textReader1 = reader.GetTextReader(0))
        {

        }
        using (var textReader2 = reader.GetTextReader(1))
        {

        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1987")]
    public async Task Bug1987()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} AS ENUM ('sad', 'ok', 'happy')");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapEnum<Mood>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        for (var i = 0; i < 2; i++)
        {
            await using var cmd = new NpgsqlCommand("SELECT @p", connection);
            cmd.Parameters.AddWithValue("p", Mood.Happy);
            Assert.That(await cmd.ExecuteScalarAsync(), Is.EqualTo(Mood.Happy));
        }
    }

    enum Mood { Sad, Ok, Happy };

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2003")]
    public void Bug2003()
    {
        // A big RowDescription (larger than buffer size) causes an oversize buffer allocation, but which isn't
        // picked up by sequential reader which continues to read from the original buffer.
        using var conn = OpenConnection();
        var longFieldName = new string('x', conn.Settings.ReadBufferSize);
        using var cmd = new NpgsqlCommand($"SELECT 8 AS {longFieldName}", conn);
        using var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess);
        reader.Read();
        Assert.That(reader.GetInt32(0), Is.EqualTo(8));
    }

    [Test]
    public async Task Bug2046()
    {
        var expected = 64.27245f;
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT @p = 64.27245::real, 64.27245::real, @p", conn);
        cmd.Parameters.AddWithValue("p", expected);
        using var rdr = await cmd.ExecuteReaderAsync();
        rdr.Read();
        Assert.That(rdr.GetFieldValue<bool>(0));
        Assert.That(rdr.GetFieldValue<float>(1), Is.EqualTo(expected));
        Assert.That(rdr.GetFieldValue<float>(2), Is.EqualTo(expected));
    }

    [Test]
    public void Bug1761()
    {
        var connString = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Enlist = true,
            Pooling = true,
            MinPoolSize = 1,
            MaxPoolSize = 1
        }.ConnectionString;

        for (var i = 0; i < 2; i++)
        {
            try
            {
                using var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMilliseconds(100));
                Thread.Sleep(1000);

                // Ambient transaction is now unusable, attempts to enlist to it will fail. We should recover
                // properly from this failure.

                using (var connection = OpenConnection(connString))
                using (var cmd = new NpgsqlCommand("SELECT 1", connection))
                {
                    cmd.CommandText = "select 1;";
                    cmd.ExecuteNonQuery();
                }

                scope.Complete();
            }
            catch (TransactionException)
            {
                //do nothing
            }
        }
    }

    [Test]
    public void Bug2274()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT 1", conn);
        cmd.Parameters.Add(new NpgsqlParameter
        {
            ParameterName = "p",
            Direction = ParameterDirection.Output
        });
        using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
        Assert.That(() => reader.GetInt32(0), Throws.Exception.TypeOf<InvalidOperationException>());
        Assert.That(reader.Read(), Is.True);
        Assert.That(reader.GetInt32(0), Is.EqualTo(1));
        Assert.That(reader.Read(), Is.False);
    }

    [Test]
    public async Task Bug2278()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var enumType = await GetTempTypeName(adminConnection);
        var domainType = await GetTempTypeName(adminConnection);
        var compositeType = await GetTempTypeName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($@"
CREATE TYPE {enumType} AS ENUM ('left', 'right');
CREATE DOMAIN {domainType} AS {enumType} NOT NULL;
CREATE TYPE {compositeType} AS (value {domainType})");
        var table = await CreateTempTable(adminConnection, $"value {compositeType}");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<Bug2278CompositeType>(compositeType);
        dataSourceBuilder.MapEnum<Bug2278EnumType>(enumType);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await connection.ExecuteScalarAsync($"SELECT * FROM {table} AS d");
    }

    class Bug2278CompositeType
    {
        public Bug2278EnumType Value { get; set; }
    }

    enum Bug2278EnumType
    {
        Left,
        Right
    }


    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/2178")]
    public void Bug2178()
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            AutoPrepareMinUsages = 2,
            MaxAutoPrepare = 2
        };
        using var conn = new NpgsqlConnection(builder.ConnectionString);
        using var cmd = new NpgsqlCommand();
        conn.Open();
        cmd.Connection = conn;

        cmd.CommandText = "SELECT 1";
        cmd.ExecuteScalar();
        cmd.ExecuteScalar();
        Assert.That(cmd.IsPrepared);

        // Now executing a faulty command multiple times
        cmd.CommandText = "SELECT * FROM public.dummy_table_name";
        for (var i = 0; i < 3; ++i)
        {
            Assert.Throws<PostgresException>(() => cmd.ExecuteScalar());
        }

        cmd.CommandText = "SELECT 1";
        cmd.ExecuteScalar();
        Assert.That(cmd.IsPrepared);
    }

    [Test]
    public async Task Bug2296()
    {
        await using var conn = await OpenConnectionAsync();
        // Note that the type has to be named boolean
        await conn.ExecuteNonQueryAsync("DROP TYPE IF EXISTS \"boolean\" CASCADE");
        await conn.ExecuteNonQueryAsync("CREATE DOMAIN pg_temp.\"boolean\" AS bool");
        conn.ReloadTypes();
        var tableName = await CreateTempTable(conn, $"mybool \"boolean\"");
        await conn.ExecuteNonQueryAsync($"INSERT INTO {tableName} (mybool) VALUES (TRUE)");

        await conn.ExecuteScalarAsync($"SELECT mybool FROM {tableName}");
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2660")]
    public void Standard_conforming_strings()
    {
        using var conn = OpenConnection();

        var sql = @"
SELECT table_name
FROM information_schema.views
WHERE table_name LIKE @p0 escape '\' AND (is_updatable = 'NO') = @p1";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@p0", "%trig%");
        cmd.Parameters.AddWithValue("@p1", true);
        using var reader = cmd.ExecuteReader();
        reader.Read();
    }

    #region Bug1285

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1285")]
    public void Bug1285()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand { Connection = conn };
        cmd.CommandText = Bug1285CreateStatement;
        cmd.ExecuteNonQuery();

        cmd.CommandText = Bug1285SelectStatement;
        cmd.Parameters.Add(new NpgsqlParameter("@1", Guid.NewGuid()));
        cmd.ExecuteNonQuery();
    }

    const string Bug1285SelectStatement =
        "select " +
        " \"Afbeelding\"" +
        ", \"Afbeelding#Id\"" +
        ", \"Omschrijving\"" +
        ", \"Website\"" +
        ", \"EMailadres\"" +
        ", \"SocialMediaVastleggen\"" +
        ", \"Facebook\"" +
        ", \"Linkedin\"" +
        ", \"Twitter\"" +
        ", \"Youtube\"" +
        ", \"Branche\"" +
        ", \"Branche#Id\"" +
        ", \"Branche#ComponentId\"" +
        ", \"Telefoonnummer\"" +
        ", \"Overheidsidentificatienummer\"" +
        ", \"Adres\"" +
        ", \"Adres#Id\"" +
        ", \"Adres#ComponentId\"" +
        ", \"2gIKz62kaTdGxw82_OrzTc4ANSM_\"" +
        ", \"OnderdeelVanOrganisatie\"" +
        ", \"OnderdeelVanOrganisatie#Id\"" +
        ", \"OnderdeelVanOrganisatie#ComponentId\"" +
        ", \"Profit\"" +
        ", \"Profit#Id\"" +
        ", \"Profit#ComponentId\"" +
        ", \"Taal\"" +
        ", \"Taal#Id\"" +
        ", \"Taal#ComponentId\"" +
        ", \"KlantSinds\"" +
        ", \"BarcodeKlant\"" +
        ", \"BarcodeKlant#BarcodeType\"" +
        ", \"BarcodeKlant#Value\"" +
        ", \"Postadres\"" +
        ", \"Postadres#Id\"" +
        ", \"Postadres#ComponentId\"" +
        ", \"LeverancierSinds\"" +
        ", \"BarcodeLeverancier\"" +
        ", \"BarcodeLeverancier#BarcodeType\"" +
        ", \"BarcodeLeverancier#Value\"" +
        ", \"Zoeknaam\"" +
        ", \"TitelEnAanhef\"" +
        ", \"TitelEnAanhef#Id\"" +
        ", \"TitelEnAanhef#ComponentId\"" +
        ", \"Rechtsvorm\"" +
        ", \"Rechtsvorm#Id\"" +
        ", \"Rechtsvorm#ComponentId\"" +
        ", \"LandVanVestiging\"" +
        ", \"LandVanVestiging#Id\"" +
        ", \"LandVanVestiging#ComponentId\"" +
        ", \"AantalMedewerkers\"" +
        ", \"NaamStatutair\"" +
        ", \"VestigingStatutair\"" +
        ", \"Correspondentie\"" +
        ", \"Medium\"" +
        ", \"FiscaalNummer\"" +
        ", \"AangebrachtDoor\"" +
        ", \"AangebrachtDoor#Id\"" +
        ", \"AangebrachtDoor#ComponentId\"" +
        ", \"Status\"" +
        ", \"NummerKamerVanKoophandel\"" +
        ", \"zII9SOHbwUPS_jKSlcRrQzuEr6A_\"" +
        ", \"Code\"" +
        ", \"OrganisatorischeEenheid\"" +
        ", \"OrganisatorischeEenheid#Id\"" +
        ", \"OrganisatorischeEenheid#ComponentId\"" +
        ", \"PJlr3asVdeHVoqmAyHIF2fF1gVM_\"" +
        ", \"IsUwv\"" +
        ", \"Uwv\"" +
        ", \"Uwv#Id\"" +
        ", \"Uwv#ComponentId\"" +
        ", \"IsVerzekeraarVoorWerkgever\"" +
        ", \"VerzekeraarVoorWerkgever\"" +
        ", \"VerzekeraarVoorWerkgever#Id\"" +
        ", \"VerzekeraarVoorWerkgever#ComponentId\"" +
        ", \"IsAbonnementsadministratie\"" +
        ", \"Abonnementsadministratie\"" +
        ", \"Abonnementsadministratie#Id\"" +
        ", \"Abonnementsadministratie#ComponentId\"" +
        ", \"IsPensioenfonds\"" +
        ", \"Pensioenfonds\"" +
        ", \"Pensioenfonds#Id\"" +
        ", \"Pensioenfonds#ComponentId\"" +
        ", \"IsProfit\"" +
        ", \"Profit1\"" +
        ", \"Profit1#Id\"" +
        ", \"Profit1#ComponentId\"" +
        ", \"Verantwoordelijke\"" +
        ", \"Verantwoordelijke#Id\"" +
        ", \"Verantwoordelijke#ComponentId\"" +
        ", \"IsKlant\"" +
        ", \"Klant\"" +
        ", \"Klant#Id\"" +
        ", \"Klant#ComponentId\"" +
        ", \"IsFactureringsadministratie\"" +
        ", \"Factureringsadministratie\"" +
        ", \"Factureringsadministratie#Id\"" +
        ", \"Factureringsadministratie#ComponentId\"" +
        ", \"IsLeninggever\"" +
        ", \"Leninggever\"" +
        ", \"Leninggever#Id\"" +
        ", \"Leninggever#ComponentId\"" +
        ", \"IsProjectadministratie\"" +
        ", \"Projectadministratie\"" +
        ", \"Projectadministratie#Id\"" +
        ", \"Projectadministratie#ComponentId\"" +
        ", \"IsVervangingsfonds\"" +
        ", \"Vervangingsfonds\"" +
        ", \"Vervangingsfonds#Id\"" +
        ", \"Vervangingsfonds#ComponentId\"" +
        ", \"IsPensioen\"" +
        ", \"Pensioen\"" +
        ", \"Pensioen#Id\"" +
        ", \"Pensioen#ComponentId\"" +
        ", \"IsVasteActivaAdministratie\"" +
        ", \"VasteActivaAdministratie\"" +
        ", \"VasteActivaAdministratie#Id\"" +
        ", \"VasteActivaAdministratie#ComponentId\"" +
        ", \"IsBelastingdienst\"" +
        ", \"Belastingdienst\"" +
        ", \"Belastingdienst#Id\"" +
        ", \"Belastingdienst#ComponentId\"" +
        ", \"IsCursusadministratie\"" +
        ", \"Cursusadministratie\"" +
        ", \"Cursusadministratie#Id\"" +
        ", \"Cursusadministratie#ComponentId\"" +
        ", \"IsUitvoerderVervangingsfonds\"" +
        ", \"UitvoerderVervangingsfonds\"" +
        ", \"UitvoerderVervangingsfonds#Id\"" +
        ", \"UitvoerderVervangingsfonds#ComponentId\"" +
        ", \"IsAssemblageadministratie\"" +
        ", \"Assemblageadministratie\"" +
        ", \"Assemblageadministratie#Id\"" +
        ", \"Assemblageadministratie#ComponentId\"" +
        ", \"IsLeasemaatschappij\"" +
        ", \"Leasemaatschappij\"" +
        ", \"Leasemaatschappij#Id\"" +
        ", \"Leasemaatschappij#ComponentId\"" +
        ", \"IsBeslaglegger\"" +
        ", \"Beslaglegger\"" +
        ", \"Beslaglegger#Id\"" +
        ", \"Beslaglegger#ComponentId\"" +
        ", \"IsVerzekeraarVoorMedewerker\"" +
        ", \"VerzekeraarVoorMedewerker\"" +
        ", \"VerzekeraarVoorMedewerker#Id\"" +
        ", \"VerzekeraarVoorMedewerker#ComponentId\"" +
        ", \"IsWoningverhuur\"" +
        ", \"Woningverhuur\"" +
        ", \"Woningverhuur#Id\"" +
        ", \"Woningverhuur#ComponentId\"" +
        ", \"9cSTu7fkjOZm08FFQUHvclQHSWY_\"" +
        ", \"Goederenstroomadministratie\"" +
        ", \"Goederenstroomadministratie#Id\"" +
        ", \"Goederenstroomadministratie#ComponentId\"" +
        ", \"IsConcurrent\"" +
        ", \"Concurrent\"" +
        ", \"Concurrent#Id\"" +
        ", \"Concurrent#ComponentId\"" +
        ", \"IsArbodienst\"" +
        ", \"Arbodienst\"" +
        ", \"Arbodienst#Id\"" +
        ", \"Arbodienst#ComponentId\"" +
        ", \"IsUitvoerderSociaalFonds\"" +
        ", \"UitvoerderSociaalFonds\"" +
        ", \"UitvoerderSociaalFonds#Id\"" +
        ", \"UitvoerderSociaalFonds#ComponentId\"" +
        ", \"IsPensioenuitvoerder\"" +
        ", \"Pensioenuitvoerder\"" +
        ", \"Pensioenuitvoerder#Id\"" +
        ", \"Pensioenuitvoerder#ComponentId\"" +
        ", \"IsProspect\"" +
        ", \"Prospect\"" +
        ", \"Prospect#Id\"" +
        ", \"Prospect#ComponentId\"" +
        ", \"IsProspectadministratie\"" +
        ", \"Prospectadministratie\"" +
        ", \"Prospectadministratie#Id\"" +
        ", \"Prospectadministratie#ComponentId\"" +
        ", \"IsArtikelbeheeradministratie\"" +
        ", \"Artikelbeheeradministratie\"" +
        ", \"Artikelbeheeradministratie#Id\"" +
        ", \"Artikelbeheeradministratie#ComponentId\"" +
        ", \"v1J4Rq2eNZy9GBvGhuCBKqga0Rg_\"" +
        ", \"g4i3gYZGL0yu0T6UwmiZTaUDI8Y_\"" +
        ", \"g4i3gYZGL0yu0T6UwmiZTaUDI8Y_#Id\"" +
        ", \"g4i3gYZGL0yu0T6UwmiZTaUDI8Y_#ComponentId\"" +
        ", \"IsSociaalFonds\"" +
        ", \"SociaalFonds\"" +
        ", \"SociaalFonds#Id\"" +
        ", \"SociaalFonds#ComponentId\"" +
        ", \"IsWagenparkadministratie\"" +
        ", \"Wagenparkadministratie\"" +
        ", \"Wagenparkadministratie#Id\"" +
        ", \"Wagenparkadministratie#ComponentId\"" +
        ", \"IsLeverancier\"" +
        ", \"Leverancier\"" +
        ", \"Leverancier#Id\"" +
        ", \"Leverancier#ComponentId\"" +
        ", \"IsWagenparkAfnemer\"" +
        ", \"WagenparkAfnemer\"" +
        ", \"WagenparkAfnemer#Id\"" +
        ", \"WagenparkAfnemer#ComponentId\"" +
        ", \"IsEvenementadministratie\"" +
        ", \"Evenementadministratie\"" +
        ", \"Evenementadministratie#Id\"" +
        ", \"Evenementadministratie#ComponentId\"" +
        ", \"IsCrediteur\"" +
        ", \"Crediteur\"" +
        ", \"Crediteur#Id\"" +
        ", \"Crediteur#ComponentId\"" +
        ", \"IsFinancieleAdministratie\"" +
        ", \"FinancieleAdministratie\"" +
        ", \"FinancieleAdministratie#Id\"" +
        ", \"FinancieleAdministratie#ComponentId\"" +
        ", \"IsSociaalSecretariaat\"" +
        ", \"SociaalSecretariaat\"" +
        ", \"SociaalSecretariaat#Id\"" +
        ", \"SociaalSecretariaat#ComponentId\"" +
        ", \"IsBank\"" +
        ", \"Bank\"" +
        ", \"Bank#Id\"" +
        ", \"Bank#ComponentId\"" +
        ", \"IsWerkgever\"" +
        ", \"Werkgever\"" +
        ", \"Werkgever#Id\"" +
        ", \"Werkgever#ComponentId\"" +
        ", \"IsVerkoopadministratie\"" +
        ", \"Verkoopadministratie\"" +
        ", \"Verkoopadministratie#Id\"" +
        ", \"Verkoopadministratie#ComponentId\"" +
        ", \"IsInkoopadministratie\"" +
        ", \"Inkoopadministratie\"" +
        ", \"Inkoopadministratie#Id\"" +
        ", \"Inkoopadministratie#ComponentId\"" +
        ", \"IsSubsidient\"" +
        ", \"Subsidient\"" +
        ", \"Subsidient#Id\"" +
        ", \"Subsidient#ComponentId\"" +
        ", \"IsEnqueteadministratie\"" +
        ", \"Enqueteadministratie\"" +
        ", \"Enqueteadministratie#Id\"" +
        ", \"Enqueteadministratie#ComponentId\"" +
        ", \"XWiQaVjEbD041r7QN0kj2aKeCys_\"" +
        ", \"X_TVE5FRBaQ97JJQbT7LmX4HBVY_\"" +
        ", \"BrancheDescription\"" +
        ", \"AdresDescription\"" +
        ", \"PicMWY7oCqeiZMTdDqwFqi1U508_\"" +
        ", \"ProfitDescription\"" +
        ", \"TaalDescription\"" +
        ", \"PostadresDescription\"" +
        ", \"TitelEnAanhefDescription\"" +
        ", \"RechtsvormDescription\"" +
        ", \"LandVanVestigingDescription\"" +
        ", \"AangebrachtDoorDescription\"" +
        ", \"KFEDo6ZYK_ffNCeHuujBCshlPVs_\"" +
        ", \"VerantwoordelijkeDescription\"" +
        ", \"FunctionalState\"" +
        ", \"KlantFinancieleAdministratie\"" +
        ", \"KlantFinancieleAdministratie#Id\"" +
        ", \"KlantFinancieleAdministratie#ComponentId\"" +
        ", \"KlantVerkoopadministratie\"" +
        ", \"KlantVerkoopadministratie#Id\"" +
        ", \"KlantVerkoopadministratie#ComponentId\"" +
        ", \"NQwaM_lfzxIm_JPrPhwruL0YOXY_\"" +
        ", \"NQwaM_lfzxIm_JPrPhwruL0YOXY_#Id\"" +
        ", \"NQwaM_lfzxIm_JPrPhwruL0YOXY_#ComponentId\"" +
        ", \"uSf1yseW4YQ1K6EoHNlzCxPofm0_\"" +
        ", \"uSf1yseW4YQ1K6EoHNlzCxPofm0_#Id\"" +
        ", \"uSf1yseW4YQ1K6EoHNlzCxPofm0_#ComponentId\"" +
        ", \"KlantEvenementadministratie\"" +
        ", \"KlantEvenementadministratie#Id\"" +
        ", \"KlantEvenementadministratie#ComponentId\"" +
        ", \"KlantCursusadministratie\"" +
        ", \"KlantCursusadministratie#Id\"" +
        ", \"KlantCursusadministratie#ComponentId\"" +
        ", \"KlantProspectadministratie\"" +
        ", \"KlantProspectadministratie#Id\"" +
        ", \"KlantProspectadministratie#ComponentId\"" +
        ", \"KlantInkoopadministratie\"" +
        ", \"KlantInkoopadministratie#Id\"" +
        ", \"KlantInkoopadministratie#ComponentId\"" +
        ", \"KlantProjectadministratie\"" +
        ", \"KlantProjectadministratie#Id\"" +
        ", \"KlantProjectadministratie#ComponentId\"" +
        ", \"P0gBTGcrSbl2kK8BM4_24fIeMvk_\"" +
        ", \"P0gBTGcrSbl2kK8BM4_24fIeMvk_#Id\"" +
        ", \"P0gBTGcrSbl2kK8BM4_24fIeMvk_#ComponentId\"" +
        ", \"KlantMain\"" +
        ", \"KlantMain#Id\"" +
        ", \"KlantMain#ComponentId\"" +
        ", \"8awXARDcCdVtrvN6IRlwAk5UcrI_\"" +
        ", \"8awXARDcCdVtrvN6IRlwAk5UcrI_#Id\"" +
        ", \"8awXARDcCdVtrvN6IRlwAk5UcrI_#ComponentId\"" +
        ", \"MjgFAhRfH64O3M9Ts_5b0ENQDBE_\"" +
        ", \"MjgFAhRfH64O3M9Ts_5b0ENQDBE_#Id\"" +
        ", \"MjgFAhRfH64O3M9Ts_5b0ENQDBE_#ComponentId\"" +
        ", \"LeverancierMain\"" +
        ", \"LeverancierMain#Id\"" +
        ", \"LeverancierMain#ComponentId\"" +
        ", \"2CaNQvGgtPNHP2XCsoKBQEpwmYA_\"" +
        ", \"2CaNQvGgtPNHP2XCsoKBQEpwmYA_#Id\"" +
        ", \"2CaNQvGgtPNHP2XCsoKBQEpwmYA_#ComponentId\"" +
        ", \"kleo39GG1utTUtP0F15mWkXZBFQ_\"" +
        ", \"kleo39GG1utTUtP0F15mWkXZBFQ_#Id\"" +
        ", \"kleo39GG1utTUtP0F15mWkXZBFQ_#ComponentId\"" +
        ", \"d9jEYARbghWBrZU6jKbtZPyZUAk_\"" +
        ", \"d9jEYARbghWBrZU6jKbtZPyZUAk_#Id\"" +
        ", \"d9jEYARbghWBrZU6jKbtZPyZUAk_#ComponentId\"" +
        ", \"CrediteurMain\"" +
        ", \"CrediteurMain#Id\"" +
        ", \"CrediteurMain#ComponentId\"" +
        ", \"TTStart\"" +
        ", \"TTEnd\"" +
        ", \"InstanceId\"" +
        ", \"StartDate\"" +
        ", \"EndDate\"" +
        ", \"UserId\"" +
        ", \"Id\"" +
        " from \"OrganisatieQmo_Organisatie_QueryModelObjects_Imp\" WHERE (\"InstanceId\" = @1) AND ((\"StartDate\" IS NULL) AND (\"TTEnd\" IS NULL)) ORDER BY \"Id\" ASC NULLS FIRST OFFSET 0 ROWS FETCH NEXT 2 ROWS ONLY;";

    const string Bug1285CreateStatement = @"
CREATE TEMP TABLE ""OrganisatieQmo_Organisatie_QueryModelObjects_Imp""
(
  ""Id"" uuid NOT NULL,
  ""InstanceId"" uuid,
  ""UserId"" text,
  ""StartDate"" timestamp(3) without time zone,
  ""EndDate"" timestamp(3) without time zone,
  ""TTStart"" timestamp(3) without time zone,
  ""TTEnd"" timestamp(3) without time zone,


  ""Afbeelding#Id"" uuid,
  ""Afbeelding"" boolean,
  ""Omschrijving"" character varying(250),
  ""Website"" text,
  ""EMailadres"" character varying(255),
  ""SocialMediaVastleggen"" boolean,
  ""Facebook"" text,
  ""Linkedin"" text,
  ""Twitter"" text,
  ""Youtube"" text,
  ""Branche#Id"" uuid,
  ""Branche#ComponentId"" uuid,
  ""Branche"" boolean,
  ""Telefoonnummer"" character varying(18),
  ""Overheidsidentificatienummer"" character varying(40),
  ""Adres#Id"" uuid,
  ""Adres#ComponentId"" uuid,
  ""Adres"" boolean,
  ""2gIKz62kaTdGxw82_OrzTc4ANSM_"" boolean,
  ""OnderdeelVanOrganisatie#Id"" uuid,
  ""OnderdeelVanOrganisatie#ComponentId"" uuid,
  ""OnderdeelVanOrganisatie"" boolean,
  ""Profit#Id"" uuid,
  ""Profit#ComponentId"" uuid,
  ""Profit"" boolean,
  ""Taal#Id"" uuid,
  ""Taal#ComponentId"" uuid,
  ""Taal"" boolean,
  ""KlantSinds"" timestamp(3) without time zone,
  ""BarcodeKlant#BarcodeType"" uuid,
  ""BarcodeKlant#Value"" text,
  ""BarcodeKlant"" boolean,
  ""Postadres#Id"" uuid,
  ""Postadres#ComponentId"" uuid,
  ""Postadres"" boolean,
  ""LeverancierSinds"" timestamp(3) without time zone,
  ""BarcodeLeverancier#BarcodeType"" uuid,
  ""BarcodeLeverancier#Value"" text,
  ""BarcodeLeverancier"" boolean,
  ""Zoeknaam"" character varying(255),
  ""TitelEnAanhef#Id"" uuid,
  ""TitelEnAanhef#ComponentId"" uuid,
  ""TitelEnAanhef"" boolean,
  ""Rechtsvorm#Id"" uuid,
  ""Rechtsvorm#ComponentId"" uuid,
  ""Rechtsvorm"" boolean,
  ""LandVanVestiging#Id"" uuid,
  ""LandVanVestiging#ComponentId"" uuid,
  ""LandVanVestiging"" boolean,
  ""AantalMedewerkers"" character varying(50),
  ""NaamStatutair"" character varying(255),
  ""VestigingStatutair"" character varying(255),
  ""Correspondentie"" boolean,
  ""Medium"" character varying(255),
  ""FiscaalNummer"" character varying(9),
  ""AangebrachtDoor#Id"" uuid,
  ""AangebrachtDoor#ComponentId"" uuid,
  ""AangebrachtDoor"" boolean,
  ""Status"" character varying(255),
  ""NummerKamerVanKoophandel"" character varying(30),
  ""zII9SOHbwUPS_jKSlcRrQzuEr6A_"" timestamp(3) without time zone,
  ""Code"" character varying(255),
  ""OrganisatorischeEenheid#Id"" uuid,
  ""OrganisatorischeEenheid#ComponentId"" uuid,
  ""OrganisatorischeEenheid"" boolean,
  ""PJlr3asVdeHVoqmAyHIF2fF1gVM_"" uuid,
  ""IsUwv"" boolean,
  ""Uwv#Id"" uuid,
  ""Uwv#ComponentId"" uuid,
  ""Uwv"" boolean,
  ""IsVerzekeraarVoorWerkgever"" boolean,
  ""VerzekeraarVoorWerkgever#Id"" uuid,
  ""VerzekeraarVoorWerkgever#ComponentId"" uuid,
  ""VerzekeraarVoorWerkgever"" boolean,
  ""IsAbonnementsadministratie"" boolean,
  ""Abonnementsadministratie#Id"" uuid,
  ""Abonnementsadministratie#ComponentId"" uuid,
  ""Abonnementsadministratie"" boolean,
  ""IsPensioenfonds"" boolean,
  ""Pensioenfonds#Id"" uuid,
  ""Pensioenfonds#ComponentId"" uuid,
  ""Pensioenfonds"" boolean,
  ""IsProfit"" boolean,
  ""Profit1#Id"" uuid,
  ""Profit1#ComponentId"" uuid,
  ""Profit1"" boolean,
  ""Verantwoordelijke#Id"" uuid,
  ""Verantwoordelijke#ComponentId"" uuid,
  ""Verantwoordelijke"" boolean,
  ""IsKlant"" boolean,
  ""Klant#Id"" uuid,
  ""Klant#ComponentId"" uuid,
  ""Klant"" boolean,
  ""IsFactureringsadministratie"" boolean,
  ""Factureringsadministratie#Id"" uuid,
  ""Factureringsadministratie#ComponentId"" uuid,
  ""Factureringsadministratie"" boolean,
  ""IsLeninggever"" boolean,
  ""Leninggever#Id"" uuid,
  ""Leninggever#ComponentId"" uuid,
  ""Leninggever"" boolean,
  ""IsProjectadministratie"" boolean,
  ""Projectadministratie#Id"" uuid,
  ""Projectadministratie#ComponentId"" uuid,
  ""Projectadministratie"" boolean,
  ""IsVervangingsfonds"" boolean,
  ""Vervangingsfonds#Id"" uuid,
  ""Vervangingsfonds#ComponentId"" uuid,
  ""Vervangingsfonds"" boolean,
  ""IsPensioen"" boolean,
  ""Pensioen#Id"" uuid,
  ""Pensioen#ComponentId"" uuid,
  ""Pensioen"" boolean,
  ""IsVasteActivaAdministratie"" boolean,
  ""VasteActivaAdministratie#Id"" uuid,
  ""VasteActivaAdministratie#ComponentId"" uuid,
  ""VasteActivaAdministratie"" boolean,
  ""IsBelastingdienst"" boolean,
  ""Belastingdienst#Id"" uuid,
  ""Belastingdienst#ComponentId"" uuid,
  ""Belastingdienst"" boolean,
  ""IsCursusadministratie"" boolean,
  ""Cursusadministratie#Id"" uuid,
  ""Cursusadministratie#ComponentId"" uuid,
  ""Cursusadministratie"" boolean,
  ""IsUitvoerderVervangingsfonds"" boolean,
  ""UitvoerderVervangingsfonds#Id"" uuid,
  ""UitvoerderVervangingsfonds#ComponentId"" uuid,
  ""UitvoerderVervangingsfonds"" boolean,
  ""IsAssemblageadministratie"" boolean,
  ""Assemblageadministratie#Id"" uuid,
  ""Assemblageadministratie#ComponentId"" uuid,
  ""Assemblageadministratie"" boolean,
  ""IsLeasemaatschappij"" boolean,
  ""Leasemaatschappij#Id"" uuid,
  ""Leasemaatschappij#ComponentId"" uuid,
  ""Leasemaatschappij"" boolean,
  ""IsBeslaglegger"" boolean,
  ""Beslaglegger#Id"" uuid,
  ""Beslaglegger#ComponentId"" uuid,
  ""Beslaglegger"" boolean,
  ""IsVerzekeraarVoorMedewerker"" boolean,
  ""VerzekeraarVoorMedewerker#Id"" uuid,
  ""VerzekeraarVoorMedewerker#ComponentId"" uuid,
  ""VerzekeraarVoorMedewerker"" boolean,
  ""IsWoningverhuur"" boolean,
  ""Woningverhuur#Id"" uuid,
  ""Woningverhuur#ComponentId"" uuid,
  ""Woningverhuur"" boolean,
  ""9cSTu7fkjOZm08FFQUHvclQHSWY_"" boolean,
  ""Goederenstroomadministratie#Id"" uuid,
  ""Goederenstroomadministratie#ComponentId"" uuid,
  ""Goederenstroomadministratie"" boolean,
  ""IsConcurrent"" boolean,
  ""Concurrent#Id"" uuid,
  ""Concurrent#ComponentId"" uuid,
  ""Concurrent"" boolean,
  ""IsArbodienst"" boolean,
  ""Arbodienst#Id"" uuid,
  ""Arbodienst#ComponentId"" uuid,
  ""Arbodienst"" boolean,
  ""IsUitvoerderSociaalFonds"" boolean,
  ""UitvoerderSociaalFonds#Id"" uuid,
  ""UitvoerderSociaalFonds#ComponentId"" uuid,
  ""UitvoerderSociaalFonds"" boolean,
  ""IsPensioenuitvoerder"" boolean,
  ""Pensioenuitvoerder#Id"" uuid,
  ""Pensioenuitvoerder#ComponentId"" uuid,
  ""Pensioenuitvoerder"" boolean,
  ""IsProspect"" boolean,
  ""Prospect#Id"" uuid,
  ""Prospect#ComponentId"" uuid,
  ""Prospect"" boolean,
  ""IsProspectadministratie"" boolean,
  ""Prospectadministratie#Id"" uuid,
  ""Prospectadministratie#ComponentId"" uuid,
  ""Prospectadministratie"" boolean,
  ""IsArtikelbeheeradministratie"" boolean,
  ""Artikelbeheeradministratie#Id"" uuid,
  ""Artikelbeheeradministratie#ComponentId"" uuid,
  ""Artikelbeheeradministratie"" boolean,
  ""v1J4Rq2eNZy9GBvGhuCBKqga0Rg_"" boolean,
  ""g4i3gYZGL0yu0T6UwmiZTaUDI8Y_#Id"" uuid,
  ""g4i3gYZGL0yu0T6UwmiZTaUDI8Y_#ComponentId"" uuid,
  ""g4i3gYZGL0yu0T6UwmiZTaUDI8Y_"" boolean,
  ""IsSociaalFonds"" boolean,
  ""SociaalFonds#Id"" uuid,
  ""SociaalFonds#ComponentId"" uuid,
  ""SociaalFonds"" boolean,
  ""IsWagenparkadministratie"" boolean,
  ""Wagenparkadministratie#Id"" uuid,
  ""Wagenparkadministratie#ComponentId"" uuid,
  ""Wagenparkadministratie"" boolean,
  ""IsLeverancier"" boolean,
  ""Leverancier#Id"" uuid,
  ""Leverancier#ComponentId"" uuid,
  ""Leverancier"" boolean,
  ""IsWagenparkAfnemer"" boolean,
  ""WagenparkAfnemer#Id"" uuid,
  ""WagenparkAfnemer#ComponentId"" uuid,
  ""WagenparkAfnemer"" boolean,
  ""IsEvenementadministratie"" boolean,
  ""Evenementadministratie#Id"" uuid,
  ""Evenementadministratie#ComponentId"" uuid,
  ""Evenementadministratie"" boolean,
  ""IsCrediteur"" boolean,
  ""Crediteur#Id"" uuid,
  ""Crediteur#ComponentId"" uuid,
  ""Crediteur"" boolean,
  ""IsFinancieleAdministratie"" boolean,
  ""FinancieleAdministratie#Id"" uuid,
  ""FinancieleAdministratie#ComponentId"" uuid,
  ""FinancieleAdministratie"" boolean,
  ""IsSociaalSecretariaat"" boolean,
  ""SociaalSecretariaat#Id"" uuid,
  ""SociaalSecretariaat#ComponentId"" uuid,
  ""SociaalSecretariaat"" boolean,
  ""IsBank"" boolean,
  ""Bank#Id"" uuid,
  ""Bank#ComponentId"" uuid,
  ""Bank"" boolean,
  ""IsWerkgever"" boolean,
  ""Werkgever#Id"" uuid,
  ""Werkgever#ComponentId"" uuid,
  ""Werkgever"" boolean,
  ""IsVerkoopadministratie"" boolean,
  ""Verkoopadministratie#Id"" uuid,
  ""Verkoopadministratie#ComponentId"" uuid,
  ""Verkoopadministratie"" boolean,
  ""IsInkoopadministratie"" boolean,
  ""Inkoopadministratie#Id"" uuid,
  ""Inkoopadministratie#ComponentId"" uuid,
  ""Inkoopadministratie"" boolean,
  ""IsSubsidient"" boolean,
  ""Subsidient#Id"" uuid,
  ""Subsidient#ComponentId"" uuid,
  ""Subsidient"" boolean,
  ""IsEnqueteadministratie"" boolean,
  ""Enqueteadministratie#Id"" uuid,
  ""Enqueteadministratie#ComponentId"" uuid,
  ""Enqueteadministratie"" boolean,
  ""XWiQaVjEbD041r7QN0kj2aKeCys_"" boolean,
  ""X_TVE5FRBaQ97JJQbT7LmX4HBVY_"" boolean,
  ""BrancheDescription"" character varying(250),
  ""AdresDescription"" character varying(250),
  ""PicMWY7oCqeiZMTdDqwFqi1U508_"" character varying(250),
  ""ProfitDescription"" character varying(100),
  ""TaalDescription"" character varying(250),
  ""PostadresDescription"" character varying(250),
  ""TitelEnAanhefDescription"" character varying(250),
  ""RechtsvormDescription"" character varying(250),
  ""LandVanVestigingDescription"" character varying(250),
  ""AangebrachtDoorDescription"" character varying(250),
  ""KFEDo6ZYK_ffNCeHuujBCshlPVs_"" character varying(250),
  ""VerantwoordelijkeDescription"" character varying(250),
  ""FunctionalState"" integer,
  ""KlantFinancieleAdministratie#Id"" uuid,
  ""KlantFinancieleAdministratie#ComponentId"" uuid,
  ""KlantFinancieleAdministratie"" boolean,
  ""KlantVerkoopadministratie#Id"" uuid,
  ""KlantVerkoopadministratie#ComponentId"" uuid,
  ""KlantVerkoopadministratie"" boolean,
  ""NQwaM_lfzxIm_JPrPhwruL0YOXY_#Id"" uuid,
  ""NQwaM_lfzxIm_JPrPhwruL0YOXY_#ComponentId"" uuid,
  ""NQwaM_lfzxIm_JPrPhwruL0YOXY_"" boolean,
  ""uSf1yseW4YQ1K6EoHNlzCxPofm0_#Id"" uuid,
  ""uSf1yseW4YQ1K6EoHNlzCxPofm0_#ComponentId"" uuid,
  ""uSf1yseW4YQ1K6EoHNlzCxPofm0_"" boolean,
  ""KlantEvenementadministratie#Id"" uuid,
  ""KlantEvenementadministratie#ComponentId"" uuid,
  ""KlantEvenementadministratie"" boolean,
  ""KlantCursusadministratie#Id"" uuid,
  ""KlantCursusadministratie#ComponentId"" uuid,
  ""KlantCursusadministratie"" boolean,
  ""KlantProspectadministratie#Id"" uuid,
  ""KlantProspectadministratie#ComponentId"" uuid,
  ""KlantProspectadministratie"" boolean,
  ""KlantInkoopadministratie#Id"" uuid,
  ""KlantInkoopadministratie#ComponentId"" uuid,
  ""KlantInkoopadministratie"" boolean,
  ""KlantProjectadministratie#Id"" uuid,
  ""KlantProjectadministratie#ComponentId"" uuid,
  ""KlantProjectadministratie"" boolean,
  ""P0gBTGcrSbl2kK8BM4_24fIeMvk_#Id"" uuid,
  ""P0gBTGcrSbl2kK8BM4_24fIeMvk_#ComponentId"" uuid,
  ""P0gBTGcrSbl2kK8BM4_24fIeMvk_"" boolean,
  ""KlantMain#Id"" uuid,
  ""KlantMain#ComponentId"" uuid,
  ""KlantMain"" boolean,
  ""8awXARDcCdVtrvN6IRlwAk5UcrI_#Id"" uuid,
  ""8awXARDcCdVtrvN6IRlwAk5UcrI_#ComponentId"" uuid,
  ""8awXARDcCdVtrvN6IRlwAk5UcrI_"" boolean,
  ""MjgFAhRfH64O3M9Ts_5b0ENQDBE_#Id"" uuid,
  ""MjgFAhRfH64O3M9Ts_5b0ENQDBE_#ComponentId"" uuid,
  ""MjgFAhRfH64O3M9Ts_5b0ENQDBE_"" boolean,
  ""LeverancierMain#Id"" uuid,
  ""LeverancierMain#ComponentId"" uuid,
  ""LeverancierMain"" boolean,
  ""2CaNQvGgtPNHP2XCsoKBQEpwmYA_#Id"" uuid,
  ""2CaNQvGgtPNHP2XCsoKBQEpwmYA_#ComponentId"" uuid,
  ""2CaNQvGgtPNHP2XCsoKBQEpwmYA_"" boolean,
  ""kleo39GG1utTUtP0F15mWkXZBFQ_#Id"" uuid,
  ""kleo39GG1utTUtP0F15mWkXZBFQ_#ComponentId"" uuid,
  ""kleo39GG1utTUtP0F15mWkXZBFQ_"" boolean,
  ""d9jEYARbghWBrZU6jKbtZPyZUAk_#Id"" uuid,
  ""d9jEYARbghWBrZU6jKbtZPyZUAk_#ComponentId"" uuid,
  ""d9jEYARbghWBrZU6jKbtZPyZUAk_"" boolean,
  ""CrediteurMain#Id"" uuid,
  ""CrediteurMain#ComponentId"" uuid,
  ""CrediteurMain"" boolean,
  CONSTRAINT ""pk_OrganisatieQmo_Organisatie_QueryModelObjects_Imp"" PRIMARY KEY (""Id"")
)";
    #endregion Bug1285

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2849")]
    public async Task Chunked_string_write_buffer_encoding_space()
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString);
        // write buffer size must be 8192 for this test to work
        // so guard against changes to the default / a change in the test harness
        builder.WriteBufferSize = 8192;
        await using var conn = await OpenConnectionAsync(builder.ConnectionString);

        var tableName = await CreateTempTable(conn, "col1 text, col2 text");

        await using var binaryImporter = await conn.BeginBinaryImportAsync($"COPY {tableName} FROM STDIN (FORMAT BINARY);");
        // 8163 writespace left
        await binaryImporter.StartRowAsync();

        // we need to almost fill the write buffer - we need one byte left in the buffer before we chunk the string for the column after this one!
        var almostBufferFillingString = new string('a', 8152);
        await binaryImporter.WriteAsync(almostBufferFillingString, NpgsqlDbType.Text);

        var unicodeCharacterThatEncodesToThreeBytesInUtf8 = '\uD55C';
        // This string needs to be long enough to be eligible for chunking, and start with a unicode character that will
        // get encoded to multiple bytes
        var longStringStartingWithAforementionedUnicodeCharacter = unicodeCharacterThatEncodesToThreeBytesInUtf8 + new string('a', 10000);
        await binaryImporter.WriteAsync(longStringStartingWithAforementionedUnicodeCharacter, NpgsqlDbType.Text);

        await binaryImporter.CompleteAsync();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2849")]
    public async Task Chunked_char_array_write_buffer_encoding_space()
    {
        var builder = new NpgsqlConnectionStringBuilder(ConnectionString);
        // write buffer size must be 8192 for this test to work
        // so guard against changes to the default / a change in the test harness
        builder.WriteBufferSize = 8192;
        await using var conn = await OpenConnectionAsync(builder.ConnectionString);

        var tableName = await CreateTempTable(conn, "col1 text, col2 text");

        await using var binaryImporter = await conn.BeginBinaryImportAsync($"COPY {tableName} FROM STDIN (FORMAT BINARY);");
        // 8163 writespace left
        await binaryImporter.StartRowAsync();

        // we need to almost fill the write buffer - we need one byte left in the buffer before we chunk the string for the column after this one!
        var almostBufferFillingString = new string('a', 8152);
        await binaryImporter.WriteAsync(almostBufferFillingString, NpgsqlDbType.Text);

        var unicodeCharacterThatEncodesToThreeBytesInUtf8 = '\uD55C';
        // This string needs to be long enough to be eligible for chunking, and start with a unicode character that will
        // get encoded to multiple bytes
        var longStringStartingWithAforementionedUnicodeCharacter = unicodeCharacterThatEncodesToThreeBytesInUtf8 + new string('a', 10000);
        await binaryImporter.WriteAsync(longStringStartingWithAforementionedUnicodeCharacter.ToCharArray(), NpgsqlDbType.Text);

        await binaryImporter.CompleteAsync();
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2371")]
    public async Task NRE_in_BeginTextExport()
    {
        await using var conn = await OpenConnectionAsync();
        var funcName = await GetTempFunctionName(conn);
        await using var transaction = await conn.BeginTransactionAsync();
        await conn.ExecuteNonQueryAsync($"CREATE OR REPLACE FUNCTION {funcName}() RETURNS TABLE (i INT) AS $$ BEGIN RETURN QUERY SELECT s.a FROM pg_stat_activity p; end; $$ LANGUAGE plpgsql;");
        using var reader = await conn.BeginTextExportAsync($"copy (select * FROM {funcName}())  TO STDOUT WITH (format csv)");
        Assert.That(() => reader.ReadLine(), Throws.Exception
            .TypeOf<PostgresException>()
            .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.UndefinedTable)
        );
    }

    public enum TestEnum
    {
        One,
        Two
    }

    class SomeComposite
    {
        /// <summary>
        /// An enum without proper handler
        /// </summary>
        public TestEnum Test { get; set; }
        public int X { get; set; }
        public string SomeText { get; set; } = "";
    }

    [Test]
    public async Task CompositePostgresType()
    {
        await using var adminConnection = await OpenConnectionAsync();
        var type = await GetTempTypeName(adminConnection);
        var func = await GetTempFunctionName(adminConnection);
        await adminConnection.ExecuteNonQueryAsync($"CREATE TYPE {type} as (x int, some_text text, test int)");

        var dataSourceBuilder = CreateDataSourceBuilder();
        dataSourceBuilder.MapComposite<SomeComposite>(type);
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        await connection.ExecuteNonQueryAsync(@$"
CREATE OR REPLACE FUNCTION {func}(id int, out comp1 {type}, OUT comp2 {type}[])
LANGUAGE plpgsql AS
$$
BEGIN
    comp1 = ROW(9, 'bar', 1)::{type};
    comp2 = ARRAY[ROW(9, 'bar', 1)::{type}];
END;
$$;");

        Assert.ThrowsAsync<InvalidCastException>(async () => await connection.ExecuteScalarAsync($"SELECT {func}(0)"));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3117")]
    public void Bug3117()
    {
        const string OkCommand = "SELECT 1";
        const string ErrorCommand = "SELECT * FROM public.imnotexist";
        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            conn.Open();
            var okCommand = new NpgsqlCommand(OkCommand, conn);
            okCommand.Prepare();
            using (okCommand.ExecuteReader()) { }

            var errorCommand = new NpgsqlCommand(ErrorCommand, conn);
            Assert.That(() => errorCommand.Prepare(), Throws.Exception
                .TypeOf<PostgresException>()
                .With.Property(nameof(PostgresException.SqlState)).EqualTo(PostgresErrorCodes.UndefinedTable));
        }

        using (var conn = new NpgsqlConnection(ConnectionString))
        {
            conn.Open();
            var okCommand = new NpgsqlCommand(OkCommand, conn);
            okCommand.Prepare();
            using (okCommand.ExecuteReader()) { }
            conn.UnprepareAll();
        }
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3209")]
    public async Task Bug3209()
    {
        await using var conn = CreateConnection();
        await conn.CloseAsync();
        await conn.OpenAsync();
        await conn.CloseAsync();
        Assert.That(conn.State, Is.EqualTo(ConnectionState.Closed));
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3373")]
    public async Task Bug3373()
    {
        await using var conn = await OpenConnectionAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT repeat('1', 10000); SELECT * from pg_sleep(3)";
        cmd.CommandTimeout = 0;

        await using var reader = await cmd.ExecuteReaderAsync();
        Assert.DoesNotThrowAsync(async () => await reader.NextResultAsync());
    }

    [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3649")]
    public async Task Bug3649()
    {
        await using var conn = await OpenConnectionAsync();
        var table = await CreateTempTable(conn, "value integer");

        using (var importer = await conn.BeginBinaryImportAsync($"COPY {table} (value) FROM STDIN (FORMAT binary)"))
        {
            await importer.StartRowAsync();
            await importer.WriteAsync(DBNull.Value, NpgsqlDbType.Integer);
            await importer.StartRowAsync();
            await importer.WriteAsync(1, NpgsqlDbType.Integer);
            await importer.StartRowAsync();
            await importer.WriteAsync(2, NpgsqlDbType.Integer);
            await importer.CompleteAsync();
        }

        using (var exporter = await conn.BeginBinaryExportAsync($"COPY {table} (value) TO STDIN (FORMAT binary)"))
        {
            await exporter.StartRowAsync();
            Assert.IsTrue(exporter.IsNull);
            await exporter.SkipAsync();
            await exporter.StartRowAsync();
            Assert.AreEqual(1, await exporter.ReadAsync<int?>());
            await exporter.StartRowAsync();
            Assert.AreEqual(2, await exporter.ReadAsync<int?>());
        }
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3839")]
    public async Task SingleThreadedSynchronizationContext_deadlock()
    {
        var syncContext = new SingleThreadSynchronizationContext(nameof(SingleThreadedSynchronizationContext_deadlock));
        using (var _ = syncContext.Enter())
        {
            // We have to Yield, so the current thread is changed to the one used by SingleThreadSynchronizationContext
            await Task.Yield();
            using var connection = OpenConnection();

            var data = new string('x', 5_000_000);
            using var cmd = new NpgsqlCommand("SELECT generate_series(1, 500000); SELECT @p", connection);
            cmd.Parameters.AddWithValue("p", NpgsqlDbType.Text, data);
            cmd.ExecuteNonQuery();
        }

        // We have to make another Yield to change the current thread from the one used by SingleThreadSynchronizationContext
        await Task.Yield();
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/3924")]
    public async Task Bug3924()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            CommandTimeout = 10,
            KeepAlive = 5,
        };

        await using var postmaster = PgPostmasterMock.Start(csb.ConnectionString);
        await using var conn = await OpenConnectionAsync(postmaster.ConnectionString);
        var serverMock = await postmaster.WaitForServerConnection();

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandTimeout = 1;
            cmd.CommandText = "SELECT 1";
            var queryTask = cmd.ExecuteNonQueryAsync();
            await serverMock.ExpectExtendedQuery();
            _ = serverMock.WriteScalarResponseAndFlush(1);
            await queryTask;
        }

        // Giving some time for keepalive to send a query and wait for a response for a little bit
        await serverMock.ExpectMessage(FrontendMessageCode.Sync);
        await Task.Delay(1000);
        _ = serverMock.WriteReadyForQuery().FlushAsync();

        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandTimeout = 1;
            cmd.CommandText = "SELECT 1";
            var queryTask = cmd.ExecuteNonQueryAsync();
            await serverMock.ExpectExtendedQuery();
            _ = serverMock.WriteScalarResponseAndFlush(1);
            Assert.DoesNotThrowAsync(async () => await queryTask);
        }
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/4099")]
    public async Task Bug4099()
    {
        var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
        {
            Multiplexing = true,
            MaxPoolSize = 1
        };
        await using var postmaster = PgPostmasterMock.Start(csb.ConnectionString);
        await using var firstConn = await OpenConnectionAsync(postmaster.ConnectionString);
        await using var secondConn = await OpenConnectionAsync(postmaster.ConnectionString);

        var byteArrayLength = csb.WriteBufferSize + 100;
        var firstQuery = firstConn.ExecuteScalarAsync("SELECT data");

        var server = await postmaster.WaitForServerConnection();
        await server.ExpectExtendedQuery();

        var secondQuery = secondConn.ExecuteScalarAsync("SELECT other_data");
        await server.ExpectExtendedQuery();

        var data = new byte[10000];
        await server
            .WriteParseComplete()
            .WriteBindComplete()
            .WriteRowDescription(new FieldDescription(PostgresTypeOIDs.Bytea))
            .WriteDataRowWithFlush(data);

        var otherData = new byte[10];
        await server
            .WriteCommandComplete()
            .WriteReadyForQuery()
            .WriteParseComplete()
            .WriteBindComplete()
            .WriteRowDescription(new FieldDescription(PostgresTypeOIDs.Bytea))
            .WriteDataRow(otherData)
            .WriteCommandComplete()
            .WriteReadyForQuery()
            .FlushAsync();

        Assert.That(data, Is.EquivalentTo((byte[])(await firstQuery)!));
        Assert.That(otherData, Is.EquivalentTo((byte[])(await secondQuery)!));
    }

    [Test]
    [IssueLink("https://github.com/npgsql/npgsql/issues/4123")]
    public async Task Bug4123()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT 1", conn);
        using var rdr = await cmd.ExecuteReaderAsync();

        await rdr.ReadAsync();
        using var stream = await rdr.GetStreamAsync(0);

        Assert.DoesNotThrowAsync(stream.FlushAsync);
        Assert.DoesNotThrow(stream.Flush);
    }
}
