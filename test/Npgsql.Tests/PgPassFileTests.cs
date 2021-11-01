using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Npgsql.Tests
{
    [TestFixture]
    public class PgPassFileTests
    {
        [Test]
        public void Should_parse_all_entries()
        {
            var file = new PgPassFile(_pgpassFile);
            var entries = file.Entries.ToList();
            Assert.That(entries.Count, Is.EqualTo(3));
        }

        [Test]
        public void Should_find_first_entry_when_multiple_match()
        {
            var file = new PgPassFile(_pgpassFile);
            var entry = file.GetFirstMatchingEntry("testhost")!;
            Assert.That(entry.Password, Is.EqualTo("testpass"));
        }

        [Test]
        public void Should_find_default_for_no_matches()
        {
            var file = new PgPassFile(_pgpassFile);
            var entry = file.GetFirstMatchingEntry("notarealhost")!;
            Assert.That(entry.Password, Is.EqualTo("defaultpass"));
        }

        readonly string _pgpassFile = Path.GetTempFileName();

        [OneTimeSetUp]
        public void CreateTestFile()
        {
            // set up pgpass file with fake content that can be used for this test
            const string content = @"testhost:1234:testdatabase:testuser:testpass
testhost:*:*:*:testdefaultpass
# helpful comment goes here
*:*:*:*:defaultpass";

            File.WriteAllText(_pgpassFile, content);
        }

        [OneTimeTearDown]
        public void DeleteTestFile()
        {
            if (File.Exists(_pgpassFile))
                File.Delete(_pgpassFile);
        }
    }
}
