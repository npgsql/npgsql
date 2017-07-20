using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Npgsql.Tests
{
    [TestFixture]
    public class PgPassFileTests
    {
        [Test]
        public void ShouldParseAllEntries()
        {
            var file = new PgPassFile(_pgpassFile);
            var fileUnique = new PgPassFile(_pgpassUniqueFile);
            var entries = file.Entries.ToList();
            var entriesUnique = fileUnique.Entries.ToList();

            Assert.Multiple(() =>
            {
                Assert.That(entries.Count, Is.EqualTo(3));
                Assert.That(entriesUnique.Count, Is.EqualTo(3));
            });
        }

        [Test]
        public void ShouldFindFirstEntryWhenMultipleMatch()
        {
            var file = new PgPassFile(_pgpassFile);
            var fileUnique = new PgPassFile(_pgpassUniqueFile);
            var entry = file.GetFirstMatchingEntry("testhost");
            var entryUnique = fileUnique.GetFirstMatchingEntry("testhost");

            Assert.Multiple(() =>
            {
                Assert.That(entry.Password, Is.EqualTo("testpass"));
                Assert.That(entryUnique.Password, Is.EqualTo("testpass"));
            });
        }

        [Test]
        public void ShouldFindDefaultForNoMatches()
        {
            var file = new PgPassFile(_pgpassFile);
            var fileUnique = new PgPassFile(_pgpassUniqueFile);
            var entry = file.GetFirstMatchingEntry("notarealhost");
            var entryUnique = fileUnique.GetFirstMatchingEntry("notarealhost");

            Assert.Multiple(() =>
            {
                Assert.That(entry.Password, Is.EqualTo("defaultpass"));
                Assert.That(entryUnique.Password, Is.EqualTo("defaultpass"));
            });
        }

        string _pgpassFile;
        string _pgpassUniqueFile;

        [OneTimeSetUp]
        public void CreateTestFile()
        {
            // set up pgpass file with fake content that can be used for this test
            const string content = @"testhost:1234:testdatabase:testuser:testpass
testhost:*:*:*:testdefaultpass
# helpful comment goes here
*:*:*:*:defaultpass";

            // create a long filename with unusual allowed characters
            const string uniqueFileName = "!#$%&'()+,.;=@[]^_`{}~FREE!#$%&'()+,.;=@[]^_`{}~FREE!#$%&'()+,.;=@[]^_`{}~FREE!#$%&'()+,.;=@[]^_`{}~FREE!#$%&'()+,.;=@[]^_`{}~FREE!#$%&'()+,.;=@[]^_`{}~FREE!#$%&'()+,.;=@[]^_`{}~FREE!#$%&'()+,.;=@[]^_`{}~FREE.tmp";

            _pgpassFile = Path.GetTempFileName();
            _pgpassUniqueFile = Path.Combine(Path.GetTempPath(), uniqueFileName);

            File.WriteAllText(_pgpassUniqueFile, content);
            File.WriteAllText(_pgpassFile, content);
        }

        [OneTimeTearDown]
        public void DeleteTestFile()
        {
            if (File.Exists(_pgpassFile))
                File.Delete(_pgpassFile);

            if (File.Exists(_pgpassUniqueFile))
                File.Delete(_pgpassUniqueFile);
        }
    }
}
