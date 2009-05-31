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

            input = "1 day 2:3:4.005";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5).Ticks, test.TotalTicks, input);

            var oldCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Globalization.CultureInfo testCulture = new System.Globalization.CultureInfo("fr-FR");
            Assert.AreEqual(",", testCulture.NumberFormat.NumberDecimalSeparator, "decimal seperator");
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = testCulture;
                input = "1 day 2:3:4.005";
                test = NpgsqlInterval.Parse(input);
                Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5).Ticks, test.TotalTicks, input);
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }

        [Test]
        public void NpgsqlIntervalConstructors()
        {
            NpgsqlInterval test;

            test = new NpgsqlInterval();
            Assert.AreEqual(0, test.Months, "Months");
            Assert.AreEqual(0, test.Days, "Days");
            Assert.AreEqual(0, test.Hours, "Hours");
            Assert.AreEqual(0, test.Minutes, "Minutes");
            Assert.AreEqual(0, test.Seconds, "Seconds");
            Assert.AreEqual(0, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(0, test.Microseconds, "Microseconds");

            test = new NpgsqlInterval(1234567890);
            Assert.AreEqual(0, test.Months, "Months");
            Assert.AreEqual(0, test.Days, "Days");
            Assert.AreEqual(0, test.Hours, "Hours");
            Assert.AreEqual(2, test.Minutes, "Minutes");
            Assert.AreEqual(3, test.Seconds, "Seconds");
            Assert.AreEqual(456, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(456789, test.Microseconds, "Microseconds");

            test = new NpgsqlInterval(new TimeSpan(1, 2, 3, 4, 5)).JustifyInterval();
            Assert.AreEqual(0, test.Months, "Months");
            Assert.AreEqual(1, test.Days, "Days");
            Assert.AreEqual(2, test.Hours, "Hours");
            Assert.AreEqual(3, test.Minutes, "Minutes");
            Assert.AreEqual(4, test.Seconds, "Seconds");
            Assert.AreEqual(5, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(5000, test.Microseconds, "Microseconds");

            test = new NpgsqlInterval(3, 2, 1234567890);
            Assert.AreEqual(3, test.Months, "Months");
            Assert.AreEqual(2, test.Days, "Days");
            Assert.AreEqual(0, test.Hours, "Hours");
            Assert.AreEqual(2, test.Minutes, "Minutes");
            Assert.AreEqual(3, test.Seconds, "Seconds");
            Assert.AreEqual(456, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(456789, test.Microseconds, "Microseconds");

            test = new NpgsqlInterval(1, 2, 3, 4);
            Assert.AreEqual(0, test.Months, "Months");
            Assert.AreEqual(1, test.Days, "Days");
            Assert.AreEqual(2, test.Hours, "Hours");
            Assert.AreEqual(3, test.Minutes, "Minutes");
            Assert.AreEqual(4, test.Seconds, "Seconds");
            Assert.AreEqual(0, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(0, test.Microseconds, "Microseconds");

            test = new NpgsqlInterval(1, 2, 3, 4, 5);
            Assert.AreEqual(0, test.Months, "Months");
            Assert.AreEqual(1, test.Days, "Days");
            Assert.AreEqual(2, test.Hours, "Hours");
            Assert.AreEqual(3, test.Minutes, "Minutes");
            Assert.AreEqual(4, test.Seconds, "Seconds");
            Assert.AreEqual(5, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(5000, test.Microseconds, "Microseconds");

            test = new NpgsqlInterval(1, 2, 3, 4, 5, 6);
            Assert.AreEqual(1, test.Months, "Months");
            Assert.AreEqual(2, test.Days, "Days");
            Assert.AreEqual(3, test.Hours, "Hours");
            Assert.AreEqual(4, test.Minutes, "Minutes");
            Assert.AreEqual(5, test.Seconds, "Seconds");
            Assert.AreEqual(6, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(6000, test.Microseconds, "Microseconds");

            test = new NpgsqlInterval(1, 2, 3, 4, 5, 6, 7);
            Assert.AreEqual(14, test.Months, "Months");
            Assert.AreEqual(3, test.Days, "Days");
            Assert.AreEqual(4, test.Hours, "Hours");
            Assert.AreEqual(5, test.Minutes, "Minutes");
            Assert.AreEqual(6, test.Seconds, "Seconds");
            Assert.AreEqual(7, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(7000, test.Microseconds, "Microseconds");
        }

        [Test]
        public void NpgsqlIntervalToString()
        {
            Assert.AreEqual("00:00:00", new NpgsqlInterval().ToString());

            Assert.AreEqual("00:02:03.456789", new NpgsqlInterval(1234567890).ToString());

            Assert.AreEqual("1 day 02:03:04.005", new NpgsqlInterval(new TimeSpan(1, 2, 3, 4, 5)).JustifyInterval().ToString());

            Assert.AreEqual("3 mons 2 days 00:02:03.456789", new NpgsqlInterval(3, 2, 1234567890).ToString());

            Assert.AreEqual("1 day 02:03:04", new NpgsqlInterval(1, 2, 3, 4).ToString());

            Assert.AreEqual("1 day 02:03:04.005", new NpgsqlInterval(1, 2, 3, 4, 5).ToString());

            Assert.AreEqual("1 mon 2 days 03:04:05.006", new NpgsqlInterval(1, 2, 3, 4, 5, 6).ToString());

            Assert.AreEqual("14 mons 3 days 04:05:06.007", new NpgsqlInterval(1, 2, 3, 4, 5, 6, 7).ToString());
        }

        [Test]
        public void NpgsqlTimeConstructors()
        {
            NpgsqlTime test;

            test = new NpgsqlTime();
            Assert.AreEqual(0, test.Hours, "Hours");
            Assert.AreEqual(0, test.Minutes, "Minutes");
            Assert.AreEqual(0, test.Seconds, "Seconds");
            Assert.AreEqual(0, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(0, test.Microseconds, "Microseconds");

            test = new NpgsqlTime(1234567890);
            Assert.AreEqual(0, test.Hours, "Hours");
            Assert.AreEqual(2, test.Minutes, "Minutes");
            Assert.AreEqual(3, test.Seconds, "Seconds");
            Assert.AreEqual(456, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(456789, test.Microseconds, "Microseconds");

            test = new NpgsqlTime(new NpgsqlInterval(0, 1, 2, 3, 4));
            Assert.AreEqual(1, test.Hours, "Hours");
            Assert.AreEqual(2, test.Minutes, "Minutes");
            Assert.AreEqual(3, test.Seconds, "Seconds");
            Assert.AreEqual(4, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(4000, test.Microseconds, "Microseconds");

            test = new NpgsqlTime(new NpgsqlTime(1, 2, 3, 4));
            Assert.AreEqual(1, test.Hours, "Hours");
            Assert.AreEqual(2, test.Minutes, "Minutes");
            Assert.AreEqual(3, test.Seconds, "Seconds");
            Assert.AreEqual(0, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(4, test.Microseconds, "Microseconds");

            test = new NpgsqlTime(new TimeSpan(0, 1, 2, 3, 4));
            Assert.AreEqual(1, test.Hours, "Hours");
            Assert.AreEqual(2, test.Minutes, "Minutes");
            Assert.AreEqual(3, test.Seconds, "Seconds");
            Assert.AreEqual(4, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(4000, test.Microseconds, "Microseconds");

            test = new NpgsqlTime(11, 45, 55.003m);
            Assert.AreEqual(11, test.Hours, "Hours");
            Assert.AreEqual(45, test.Minutes, "Minutes");
            Assert.AreEqual(55, test.Seconds, "Seconds");
            Assert.AreEqual(3, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(3000, test.Microseconds, "Microseconds");

            test = new NpgsqlTime(11, 45, 55.003);
            Assert.AreEqual(11, test.Hours, "Hours");
            Assert.AreEqual(45, test.Minutes, "Minutes");
            Assert.AreEqual(55, test.Seconds, "Seconds");
            Assert.AreEqual(3, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(3000, test.Microseconds, "Microseconds");

            test = new NpgsqlTime(4, 38, 53);
            Assert.AreEqual(4, test.Hours, "Hours");
            Assert.AreEqual(38, test.Minutes, "Minutes");
            Assert.AreEqual(53, test.Seconds, "Seconds");
            Assert.AreEqual(0, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(0, test.Microseconds, "Microseconds");

            test = new NpgsqlTime(4, 38, 53, 123456);
            Assert.AreEqual(4, test.Hours, "Hours");
            Assert.AreEqual(38, test.Minutes, "Minutes");
            Assert.AreEqual(53, test.Seconds, "Seconds");
            Assert.AreEqual(123, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(123456, test.Microseconds, "Microseconds");
        }

        [Test]
        public void NpgsqlTimeToString()
        {
            Assert.AreEqual("11:45:55.003", new NpgsqlTime(11, 45, 55.003m).ToString());
        }
	}
}
