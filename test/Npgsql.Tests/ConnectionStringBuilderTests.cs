using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Npgsql.Tests
{
    class ConnectionStringBuilderTests
    {
        [Test]
        public void Basic()
        {
            Assert.That(Builder.ConnectionString, Is.EqualTo(""));
            Assert.That(Builder.Count, Is.EqualTo(0));
            Assert.That(Builder.ContainsKey("server"), Is.True);
            Builder.Host = "myhost";
            Assert.That(Builder["host"], Is.EqualTo("myhost"));
            Assert.That(Builder.Count, Is.EqualTo(1));
            Assert.That(Builder.ConnectionString, Is.EqualTo("Host=myhost"));
            Builder.Remove("HOST");
            Assert.That(Builder["host"], Is.EqualTo(""));
            Assert.That(Builder.Count, Is.EqualTo(0));
        }

        [Test]
        public void FromString()
        {
            Builder.ConnectionString = "Host=myhost;EF Template Database=foo";
            Assert.That(Builder.Host, Is.EqualTo("myhost"));
            Assert.That(Builder.EntityTemplateDatabase, Is.EqualTo("foo"));
        }

        [Test]
        public void TryGetValue()
        {
            object value;
            Builder.ConnectionString = "Host=myhost";

            Assert.That(Builder.TryGetValue("Host", out value), Is.True);
            Assert.That(value, Is.EqualTo("myhost"));

            Assert.That(Builder.TryGetValue("SomethingUnknown", out value), Is.False);
        }

        [Test]
        public void Clear()
        {
            Builder.Host = "myhost";
            Builder.Clear();
            Assert.That(Builder.Count, Is.EqualTo(0));
            Assert.That(Builder["host"], Is.EqualTo(""));
            Assert.That(Builder.Host, Is.Null);
        }

        [Test]
        public void Default()
        {
            Assert.That(Builder.Port, Is.EqualTo(NpgsqlConnection.DefaultPort));
            Builder.Port = 8;
            Builder.Remove("Port");
            Assert.That(Builder.Port, Is.EqualTo(NpgsqlConnection.DefaultPort));
        }

        [Test]
        public void Enum()
        {
            Builder.ConnectionString = "SslMode=Prefer";
            Assert.That(Builder.SslMode, Is.EqualTo(SslMode.Prefer));
            Assert.That(Builder.Count, Is.EqualTo(1));
        }

        [Test]
        public void Clone()
        {
            Builder.Host = "myhost";
            var builder2 = Builder.Clone();
            Assert.That(builder2.Host, Is.EqualTo("myhost"));
            Assert.That(builder2["Host"], Is.EqualTo("myhost"));
            Assert.That(Builder.Port, Is.EqualTo(NpgsqlConnection.DefaultPort));
        }

        [Test]
        public void ConversionError()
        {
            Assert.That(() => Builder["Port"] = "hello",
                Throws.Exception.TypeOf<ArgumentException>().With.Message.Contains("Port"));
        }

        [Test]
        public void InvalidConnectionString()
        {
            Assert.That(() => Builder.ConnectionString = "Server=127.0.0.1;User Id=npgsql_tests;Pooling:false",
                Throws.Exception.TypeOf<ArgumentException>());
        }

        #region Setup

        NpgsqlConnectionStringBuilder Builder { get; set; }

        [SetUp]
        public void SetUp()
        {
            Builder = new NpgsqlConnectionStringBuilder();
        }

        #endregion
    }
}
