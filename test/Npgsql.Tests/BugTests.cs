using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using System.Transactions;

namespace Npgsql.Tests
{
    public class BugTests : TestBase
    {
        #region Sequential reader bugs

        [Test, Description("In sequential access, performing a null check on a non-first field would check the first field")]
        public void SequentialNullCheckOnNonFirstField()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 'X', NULL", conn))
            using (var dr = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
            {
                dr.Read();
                Assert.That(dr.IsDBNull(1), Is.True);
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1034")]
        public void SequentialSkipOverFirstRow()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn))
            using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
            {
                Assert.That(reader.NextResult(), Is.True);
                Assert.That(reader.Read(), Is.True);
                Assert.That(reader.GetInt32(0), Is.EqualTo(2));
            }
        }

        [Test]
        public void SequentialConsumeWithNull()
        {
            using (var conn = OpenConnection())
            using (var command = new NpgsqlCommand("SELECT 1, NULL", conn))
            using (var reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
                reader.Read();
        }

        #endregion

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1210")]
        public void ManyParametersWithMixedFormatCode()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                var sb = new StringBuilder("SELECT @text_param");
                cmd.Parameters.AddWithValue("@text_param", "some_text");
                for (var i = 0; i < conn.Settings.WriteBufferSize; i++)
                {
                    var paramName = $"@binary_param{i}";
                    sb.Append(",");
                    sb.Append(paramName);
                    cmd.Parameters.AddWithValue(paramName, 8);
                }
                cmd.CommandText = sb.ToString();

                Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception
                    .TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("54000")
                );
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1238")]
        public void RecordWithNonIntField()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT ('one'::TEXT, 2)", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                var record = reader.GetFieldValue<object[]>(0);
                Assert.That(record[0], Is.EqualTo("one"));
                Assert.That(record[1], Is.EqualTo(2));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1450")]
        public void Bug1450()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "CREATE TEMP TABLE a (a1 int); CREATE TEMP TABLE b (b1 int);";
                cmd.ExecuteNonQuery();

                cmd.CommandText = "CREATE TEMP TABLE c (c1 int);";
                cmd.ExecuteNonQuery();
            }
        }

        [Test]
        public void Bug1645()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (field_text TEXT, field_int2 SMALLINT, field_int4 INTEGER)");
                Assert.That(() =>
                {
                    using (var writer = conn.BeginBinaryImport("COPY data (field_text, field_int4) FROM STDIN BINARY"))
                    {
                        writer.StartRow();
                        writer.Write("foo");
                        writer.Write(8);

                        writer.StartRow();
                        throw new InvalidOperationException("Catch me outside the using statement if you can!");
                    }
                }, Throws.Exception
                    .TypeOf<InvalidOperationException>()
                    .With.Property(nameof(InvalidOperationException.Message)).EqualTo("Catch me outside the using statement if you can!")
                );
                Assert.That(conn.ExecuteScalar("SELECT COUNT(*) FROM data"), Is.Zero);
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1497")]
        public void Bug1497()
        {
            using (var conn = OpenConnection())
            {
                conn.ExecuteNonQuery("CREATE TEMP TABLE data (id INT4)");
                conn.ExecuteNonQuery("INSERT INTO data (id) VALUES (NULL)");
                using (var cmd = new NpgsqlCommand("SELECT * FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(reader);
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1558")]
        public void Bug1558()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                Pooling = false,
                Enlist = true
            };
            using (var tx = new TransactionScope())
            using (var conn = new NpgsqlConnection(csb.ToString()))
            {
                conn.Open();
            }
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
            using (var conn = OpenConnection(csb))
            {
                using (var cmd = new NpgsqlCommand("SELECT 1; SELECT 2", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    // Both statements should get prepared. However, purposefully skip processing the
                    // second resultset and make sure the second statement got prepared correctly.
                }
                Assert.That(conn.ExecuteScalar("SELECT 2"), Is.EqualTo(2));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1700")]
        public void Bug1700()
        {
            Assert.That(() =>
            {
                using (var conn = OpenConnection())
                using (var tx = conn.BeginTransaction())
                using (var cmd = conn.CreateCommand())
                {
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
                }
            }, Throws.InvalidOperationException.With.Message.EqualTo("Some problem parsing the returned data"));
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1964")]
        public void Bug1964()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("INVALID SQL", conn))
            {
                cmd.Parameters.Add(new NpgsqlParameter { ParameterName = "p", Direction = ParameterDirection.Output });
                Assert.That(() => cmd.ExecuteNonQuery(), Throws.Exception.TypeOf<PostgresException>()
                    .With.Property(nameof(PostgresException.SqlState)).EqualTo("42601"));
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1986")]
        public void Bug1986()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT 'hello', 'goodbye'", conn))
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                using (var textReader1 = reader.GetTextReader(0))
                {

                }
                using (var textReader2 = reader.GetTextReader(1))
                {

                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1987")]
        public void Bug1987()
        {
            var csb = new NpgsqlConnectionStringBuilder(ConnectionString)
            {
                MaxAutoPrepare = 10,
                AutoPrepareMinUsages = 2,
                Pooling = false
            };

            using (var conn = OpenConnection(csb))
            {
                conn.ExecuteNonQuery("CREATE TYPE pg_temp.mood AS ENUM ('sad', 'ok', 'happy')");
                conn.ReloadTypes();
                conn.TypeMapper.MapEnum<Mood>("mood");
                for (var i = 0; i < 2; i++)
                {
                    using (var cmd = new NpgsqlCommand("SELECT @p", conn))
                    {
                        cmd.Parameters.AddWithValue("p", Mood.Happy);
                        Assert.That(cmd.ExecuteScalar(), Is.EqualTo(Mood.Happy));
                    }
                }
            }
        }

        enum Mood { Sad, Ok, Happy };

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/2003")]
        public void Bug2003()
        {
            // A big RowDescription (larger than buffer size) causes an oversize buffer allocation, but which isn't
            // picked up by sequential reader which continues to read from the original buffer.
            using (var conn = OpenConnection())
            {
                var longFieldName = new string('x', conn.Settings.ReadBufferSize);
                using (var cmd = new NpgsqlCommand($"SELECT 8 AS {longFieldName}", conn))
                using (var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
                {
                    reader.Read();
                    Assert.That(reader.GetInt32(0), Is.EqualTo(8));
                }
            }
        }

        [Test]
        public void Bug2046()
        {
            var expected = 64.27245f;
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p = 64.27245::real, 64.27245::real, @p", conn))
            {
                cmd.Parameters.AddWithValue("p", expected);
                using (var rdr = cmd.ExecuteRecord())
                {
                    Assert.That(rdr.GetFieldValue<bool>(0));
                    Assert.That(rdr.GetFieldValue<float>(1), Is.EqualTo(expected));
                    Assert.That(rdr.GetFieldValue<float>(2), Is.EqualTo(expected));
                }
            }
        }

        #region Bug1285

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/1285")]
        public void Bug1285()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand { Connection = conn })
            {
                cmd.CommandText = Bug1285CreateStatement;
                cmd.ExecuteNonQuery();

                cmd.CommandText = Bug1285SelectStatement;
                cmd.Parameters.Add(new NpgsqlParameter("@1", Guid.NewGuid()));
                cmd.ExecuteNonQuery();
            }
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
    }
}
