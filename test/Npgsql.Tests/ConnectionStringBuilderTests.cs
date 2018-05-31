#region License
// The PostgreSQL License
//
// Copyright (C) 2018 The Npgsql Development Team
//
// Permission to use, copy, modify, and distribute this software and its
// documentation for any purpose, without fee, and without a written
// agreement is hereby granted, provided that the above copyright notice
// and this paragraph and the following two paragraphs appear in all copies.
//
// IN NO EVENT SHALL THE NPGSQL DEVELOPMENT TEAM BE LIABLE TO ANY PARTY
// FOR DIRECT, INDIRECT, SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES,
// INCLUDING LOST PROFITS, ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS
// DOCUMENTATION, EVEN IF THE NPGSQL DEVELOPMENT TEAM HAS BEEN ADVISED OF
// THE POSSIBILITY OF SUCH DAMAGE.
//
// THE NPGSQL DEVELOPMENT TEAM SPECIFICALLY DISCLAIMS ANY WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS
// ON AN "AS IS" BASIS, AND THE NPGSQL DEVELOPMENT TEAM HAS NO OBLIGATIONS
// TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS, OR MODIFICATIONS.
#endregion

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
            Builder.ConnectionString = "Host=myhost";

            Assert.That(Builder.TryGetValue("Host", out var value), Is.True);
            Assert.That(value, Is.EqualTo("myhost"));

            Assert.That(Builder.TryGetValue("SomethingUnknown", out value), Is.False);
        }

        [Test]
        public void Remove()
        {
            Assert.That(Builder.ConnectionString, Is.EqualTo(""));
            Builder.SslMode = SslMode.Prefer;
            Assert.That(Builder["SSL Mode"], Is.EqualTo(SslMode.Prefer));
            Builder.Remove("SSL Mode");
            Assert.That(Builder.ConnectionString, Is.EqualTo(""));
            Builder.CommandTimeout = 120;
            Assert.That(Builder["Command Timeout"], Is.EqualTo(120));
            Builder.Remove("Command Timeout");
            Assert.That(Builder.ConnectionString, Is.EqualTo(""));
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
