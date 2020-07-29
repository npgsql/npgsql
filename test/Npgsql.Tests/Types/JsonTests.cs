using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    [TestFixture(MultiplexingMode.NonMultiplexing, NpgsqlDbType.Json)]
    [TestFixture(MultiplexingMode.NonMultiplexing, NpgsqlDbType.Jsonb)]
    [TestFixture(MultiplexingMode.Multiplexing, NpgsqlDbType.Json)]
    [TestFixture(MultiplexingMode.Multiplexing, NpgsqlDbType.Jsonb)]
    public class JsonTests : MultiplexingTestBase
    {
        [Test]
        public async Task RoundtripString()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                const string value = @"{""Key"": ""Value""}";
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType) { Value = value });
                cmd.Parameters.Add(new NpgsqlParameter<string>("p2", NpgsqlDbType) { TypedValue = value });
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    for (var i = 0; i < 2; i++)
                    {
                        Assert.That(reader.GetString(i), Is.EqualTo(value));
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(string)));

                        using (var textReader = reader.GetTextReader(i))
                            Assert.That(textReader.ReadToEnd(), Is.EqualTo(value));
                    }
                }
            }
        }

        [Test]
        public async Task RoundtripLongString()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                var sb = new StringBuilder();
                sb.Append(@"{""Key"": """);
                sb.Append('x', conn.Settings.WriteBufferSize);
                sb.Append(@"""}");
                var value = sb.ToString();
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType) { Value = value });
                cmd.Parameters.Add(new NpgsqlParameter<string>("p2", NpgsqlDbType) { TypedValue = value });
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    for (var i = 0; i < 2; i++)
                    {
                        Assert.That(reader.GetString(i), Is.EqualTo(value));
                        Assert.That(reader.GetFieldType(i), Is.EqualTo(typeof(string)));

                        using (var textReader = reader.GetTextReader(i))
                            Assert.That(textReader.ReadToEnd(), Is.EqualTo(value));
                    }
                }
            }
        }

        [Test, IssueLink("https://github.com/npgsql/npgsql/issues/3085")]
        public async Task RoundtripStringTypes()
        {
            var expected = "{\"p\":1}";
            // If we serialize to JSONB, Postgres will not store the Json.NET formatting, and will add a space after ':'
            var expectedString = NpgsqlDbType.Equals(NpgsqlDbType.Jsonb) ? "{\"p\": 1}"
                                    : "{\"p\":1}";

            using var conn = OpenConnection();
            using var cmd = new NpgsqlCommand(@"SELECT @p1, @p2, @p3", conn);

            cmd.Parameters.Add(new NpgsqlParameter<string>("p1", NpgsqlDbType) { Value = expected });
            cmd.Parameters.Add(new NpgsqlParameter<char[]>("p2", NpgsqlDbType) { Value = expected.ToCharArray() });
            cmd.Parameters.Add(new NpgsqlParameter<byte[]>("p3", NpgsqlDbType) { Value = Encoding.ASCII.GetBytes(expected) });

            await using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            Assert.That(reader.GetFieldValue<string>(0), Is.EqualTo(expectedString));
            Assert.That(reader.GetFieldValue<char[]>(1), Is.EqualTo(expectedString.ToCharArray()));
            Assert.That(reader.GetFieldValue<byte[]>(2), Is.EqualTo(Encoding.ASCII.GetBytes(expectedString)));
        }

        [Test, Ignore("INpgsqlTypeHandler<ArraySegment<char>>.Read currently not yet implemented in TextHandler")]
        public async Task RoundtripArraySegment()
        {
            var expected = "{\"p\":1}";
            // If we serialize to JSONB, Postgres will not store the Json.NET formatting, and will add a space after ':'
            var expectedString = NpgsqlDbType.Equals(NpgsqlDbType.Jsonb) ? "{\"p\": 1}"
                                    : "{\"p\":1}";

            using var conn = OpenConnection();
            using var cmd = new NpgsqlCommand(@"SELECT @p1", conn);

            cmd.Parameters.Add(new NpgsqlParameter<ArraySegment<char>>("p1", NpgsqlDbType) { Value = new ArraySegment<char>(expected.ToCharArray()) });

            await using var reader = await cmd.ExecuteReaderAsync();
            reader.Read();
            Assert.That(reader.GetFieldValue<ArraySegment<char>>(0), Is.EqualTo(expectedString));
        }


        [Test]
        public async Task ReadJsonDocument()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                var value = @"{""Date"":""2019-09-01T00:00:00"",""TemperatureC"":10,""Summary"":""Partly cloudy""}";
                cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType) { Value = value });
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Is.EqualTo(PostgresType));
                    var root = reader.GetFieldValue<JsonDocument>(0).RootElement;
                    Assert.That(root.GetProperty("Date").GetDateTime(), Is.EqualTo(new DateTime(2019, 9, 1)));
                    Assert.That(root.GetProperty("Summary").GetString(), Is.EqualTo("Partly cloudy"));
                    Assert.That(root.GetProperty("TemperatureC").GetInt32(), Is.EqualTo(10));
                }
            }
        }

        [Test]
        public async Task WriteJsonDocument()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                var value = JsonDocument.Parse(@"{""Date"": ""2019-09-01T00:00:00"", ""Summary"": ""Partly cloudy"", ""TemperatureC"": 10}");
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType) { Value = value });
                cmd.Parameters.Add(new NpgsqlParameter<JsonDocument>("p2", NpgsqlDbType) { TypedValue = value });
                if (IsJsonb)
                {
                    cmd.CommandText += ", @p3";
                    cmd.Parameters.AddWithValue("p3", value);
                }

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        // Warning: in theory jsonb order and whitespace may change across versions
                        Assert.That(reader.GetString(0), Is.EqualTo(IsJsonb
                            ? @"{""Date"": ""2019-09-01T00:00:00"", ""Summary"": ""Partly cloudy"", ""TemperatureC"": 10}"
                            : @"{""Date"":""2019-09-01T00:00:00"",""Summary"":""Partly cloudy"",""TemperatureC"":10}"));
                    }
                }
            }
        }

        [Test]
        public async Task WriteObject()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                var value = new WeatherForecast
                {
                    Date = new DateTime(2019, 9, 1),
                    Summary = "Partly cloudy",
                    TemperatureC = 10
                };
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType) { Value = value });
                cmd.Parameters.Add(new NpgsqlParameter<WeatherForecast>("p2", NpgsqlDbType) { TypedValue = value });
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    for (var i = 0; i < 2; i++)
                    {
                        // Warning: in theory jsonb order and whitespace may change across versions
                        Assert.That(reader.GetString(0), Is.EqualTo(IsJsonb
                            ? @"{""Date"": ""2019-09-01T00:00:00"", ""Summary"": ""Partly cloudy"", ""TemperatureC"": 10}"
                            : @"{""Date"":""2019-09-01T00:00:00"",""TemperatureC"":10,""Summary"":""Partly cloudy""}"));
                    }
                }
            }
        }

        [Test]
        public async Task ReadObject()
        {
            using (var conn = await OpenConnectionAsync())
            using (var cmd = new NpgsqlCommand("SELECT @p", conn))
            {
                var value = @"{""Date"":""2019-09-01T00:00:00"",""TemperatureC"":10,""Summary"":""Partly cloudy""}";
                cmd.Parameters.Add(new NpgsqlParameter("p", NpgsqlDbType) { Value = value });
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    reader.Read();
                    Assert.That(reader.GetDataTypeName(0), Is.EqualTo(PostgresType));
                    var actual = reader.GetFieldValue<WeatherForecast>(0);
                    Assert.That(actual.Date, Is.EqualTo(new DateTime(2019, 9, 1)));
                    Assert.That(actual.Summary, Is.EqualTo("Partly cloudy"));
                    Assert.That(actual.TemperatureC, Is.EqualTo(10));
                }
            }
        }

        class WeatherForecast
        {
            public DateTime Date { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; } = "";
        }

        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/2811")]
        [IssueLink("https://github.com/npgsql/efcore.pg/issues/1177")]
        [IssueLink("https://github.com/npgsql/efcore.pg/issues/1082")]
        public async Task CanReadTwoJsonDocuments()
        {
            using var conn = await OpenConnectionAsync();

            JsonDocument car;
            using (var cmd = new NpgsqlCommand(@"SELECT '{""key"" : ""foo""}'::jsonb", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                reader.Read();
                car = reader.GetFieldValue<JsonDocument>(0);
            }

            using (var cmd = new NpgsqlCommand(@"SELECT '{""key"" : ""bar""}'::jsonb", conn))
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                reader.Read();
                reader.GetFieldValue<JsonDocument>(0);
            }

            Assert.That(car.RootElement.GetProperty("key").GetString(), Is.EqualTo("foo"));
        }

        public JsonTests(MultiplexingMode multiplexingMode, NpgsqlDbType npgsqlDbType)
            : base(multiplexingMode)
        {
            using (var conn = OpenConnection())
                TestUtil.MinimumPgVersion(conn, "9.4.0", "JSONB data type not yet introduced");
            NpgsqlDbType = npgsqlDbType;
        }

        bool IsJsonb => NpgsqlDbType == NpgsqlDbType.Jsonb;
        string PostgresType => IsJsonb ? "jsonb" : "json";
        readonly NpgsqlDbType NpgsqlDbType;
    }
}
