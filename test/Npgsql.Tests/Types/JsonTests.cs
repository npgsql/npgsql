using System;
using System.Text;
using NpgsqlTypes;
using NUnit.Framework;

namespace Npgsql.Tests.Types
{
    [TestFixture(NpgsqlDbType.Jsonb)]
    [TestFixture(NpgsqlDbType.Json)]
    public class JsonTests : TestBase
    {
        [Test]
        public void RoundtripString()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                const string value = @"{""Key"": ""Value""}";
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType) { Value = value });
                cmd.Parameters.Add(new NpgsqlParameter<string>("p2", NpgsqlDbType) { TypedValue = value });
                using (var reader = cmd.ExecuteReader())
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
        public void RoundtripLongString()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                var sb = new StringBuilder();
                sb.Append(@"{""Key"": """);
                sb.Append('x', conn.Settings.WriteBufferSize);
                sb.Append(@"""}");
                var value = sb.ToString();
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType) { Value = value });
                cmd.Parameters.Add(new NpgsqlParameter<string>("p2", NpgsqlDbType) { TypedValue = value });
                using (var reader = cmd.ExecuteReader())
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
        public void WriteObject()
        {
            using (var conn = OpenConnection())
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
                using (var reader = cmd.ExecuteReader())
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
        public void ReadObject()
        {
            using (var conn = OpenConnection())
            using (var cmd = new NpgsqlCommand("SELECT @p1, @p2", conn))
            {
                var value = @"{""Date"":""2019-09-01T00:00:00"",""TemperatureC"":10,""Summary"":""Partly cloudy""}";
                cmd.Parameters.Add(new NpgsqlParameter("p1", NpgsqlDbType) { Value = value });
                cmd.Parameters.Add(new NpgsqlParameter<string>("p2", NpgsqlDbType) { TypedValue = value });
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    for (var i = 0; i < 2; i++)
                    {
                        var actual = reader.GetFieldValue<WeatherForecast>(i);
                        Assert.That(actual.Date, Is.EqualTo(new DateTime(2019, 9, 1)));
                        Assert.That(actual.Summary, Is.EqualTo("Partly cloudy"));
                        Assert.That(actual.TemperatureC, Is.EqualTo(10));
                    }
                }
            }
        }

        class WeatherForecast
        {
            public DateTime Date { get; set; }
            public int TemperatureC { get; set; }
            public string Summary { get; set; } = "";
        }

        public JsonTests(NpgsqlDbType npgsqlDbType)
        {
            using (var conn = OpenConnection())
                TestUtil.MinimumPgVersion(conn, "9.4.0", "JSONB data type not yet introduced");
            NpgsqlDbType = npgsqlDbType;
        }

        bool IsJsonb => NpgsqlDbType == NpgsqlDbType.Jsonb;
        readonly NpgsqlDbType NpgsqlDbType;
    }
}
