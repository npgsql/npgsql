using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Npgsql.Tests
{
    [TestFixture]
    public class PgPassEntryTests
    {
        [Test]
        public void ParsesWellFormedEntry()
        {
            var input = "test:1234:test2:test3:test4";
            var entry = PgPassEntry.Create(input);

            Assert.That(entry, Is.Not.Null);
            Assert.That("test", Is.EqualTo(entry.Hostname));
            Assert.That(1234, Is.EqualTo(entry.Port));
            Assert.That("test2", Is.EqualTo(entry.Database));
            Assert.That("test3", Is.EqualTo(entry.UserName));
            Assert.That("test4", Is.EqualTo(entry.Password));
        }

        [Test]
        [TestCase("test:1234:test2:test3")]
        [TestCase("test:myport:test2:test3:test4")]
        public void ThrowFormatExceptionForBadEntry(string input)
        {
            ActualValueDelegate<object> createDelegate = () => PgPassEntry.Create(input);
            Assert.That(createDelegate, Throws.TypeOf<FormatException>());
        }

        [Test]
        public void HandleEscapedCharacters()
        {
            var input = "t\\:est:1234:test2:test3:test\\\\4";
            var entry = PgPassEntry.Create(input);

            Assert.That(entry, Is.Not.Null);
            Assert.That("t:est", Is.EqualTo(entry.Hostname));
            Assert.That(1234, Is.EqualTo(entry.Port));
            Assert.That("test2", Is.EqualTo(entry.Database));
            Assert.That("test3", Is.EqualTo(entry.UserName));
            Assert.That("test\\4", Is.EqualTo(entry.Password));
        }

        [Test]
        public void MatchTrueForExactMatch()
        {
            var input = "test:1234:test2:test3:test4";
            var entry = PgPassEntry.Create(input);

            var isMatch = entry.IsMatch("test", 1234, "test2", "test3");
            
            Assert.That(isMatch, Is.True);
        }

        [Test]
        public void MatchTrueForWildcardEntry()
        {
            var input = "*:1234:test2:test3:test4";
            var entry = PgPassEntry.Create(input);

            var isMatch = entry.IsMatch("test", 1234, "test2", "test3");

            Assert.That(isMatch, Is.True);
        }

        [Test]
        public void MatchTrueForWildcardQuery()
        {
            var input = "test:1234:test2:test3:test4";
            var entry = PgPassEntry.Create(input);

            var isMatch = entry.IsMatch("*", 1234, "test2", "test3");

            Assert.That(isMatch, Is.True);
        }

        [Test]
        public void MatchFalseForBadQuery()
        {
            var input = "test:1234:test2:test3:test4";
            var entry = PgPassEntry.Create(input);

            var isMatch = entry.IsMatch("notamatch", 1234, "test2", "test3");

            Assert.That(isMatch, Is.False);
        }

        [Test]
        public void MatchTrueForNullQuery()
        {
            var input = "test:1234:test2:test3:test4";
            var entry = PgPassEntry.Create(input);

            var isMatch = entry.IsMatch(null, 1234, "test2", "test3");

            Assert.That(isMatch, Is.True);
        }
    }
}
