// Npgsql.NpgsqlCommand.cs
//
// Author:
//  Josh Cooley <jbnpgsql@tuxinthebox.net>
//
//  Copyright (C) 2008 The Npgsql Development Team
//  npgsql-devel@pgfoundry.org
//  http://project.npgsql.org
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
using System;
using System.Data;

using NpgsqlTypes;
using NUnit.Framework;

namespace NpgsqlTests
{
    /// <summary>
    /// Tests NpgsqlTypes.* independent of a database
    /// </summary>
    [TestFixture]
	public class TypesTests
	{
        [Test]
        public void NpgsqlIntervalParse()
        {
            string input;
            NpgsqlInterval test;

            input = "1 day";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(1).Ticks, test.TotalTicks, input);

            input = "2 days";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(2).Ticks, test.TotalTicks, input);

            input = "2 days 3:04:05";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(new TimeSpan(2, 3, 4, 5).Ticks, test.TotalTicks, input);

            input = "-2 days";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(-2).Ticks, test.TotalTicks, input);

            input = "-2 days -3:04:05";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(new TimeSpan(-2, -3, -4, -5).Ticks, test.TotalTicks, input);

            input = "-2 days -0:01:02";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(new TimeSpan(-2, 0, -1, -2).Ticks, test.TotalTicks, input);

            input = "2 days -12:00";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(new TimeSpan(2, -12, 0, 0).Ticks, test.TotalTicks, input);

            input = "1 mon";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30).Ticks, test.TotalTicks, input);

            input = "2 mons";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(60).Ticks, test.TotalTicks, input);

            input = "1 mon -1 day";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(29).Ticks, test.TotalTicks, input);

            input = "1 mon -2 days";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(28).Ticks, test.TotalTicks, input);

            input = "-1 mon -2 days -3:04:05";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(new TimeSpan(-32, -3, -4, -5).Ticks, test.TotalTicks, input);

            input = "1 year";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30 * 12).Ticks, test.TotalTicks, input);

            input = "2 years";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30 * 24).Ticks, test.TotalTicks, input);

            input = "1 year -1 mon";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30 * 11).Ticks, test.TotalTicks, input);

            input = "1 year -2 mons";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30 * 10).Ticks, test.TotalTicks, input);

            input = "1 year -1 day";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30 * 12 - 1).Ticks, test.TotalTicks, input);

            input = "1 year -2 days";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30 * 12 - 2).Ticks, test.TotalTicks, input);

            input = "1 year -1 mon -1 day";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30 * 11 - 1).Ticks, test.TotalTicks, input);

            input = "1 year -2 mons -2 days";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30 * 10 - 2).Ticks, test.TotalTicks, input);
        }
	}
}
