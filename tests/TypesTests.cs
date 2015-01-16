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
using Npgsql;

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
            Assert.AreEqual(TimeSpan.FromDays(30*12).Ticks, test.TotalTicks, input);

            input = "2 years";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*24).Ticks, test.TotalTicks, input);

            input = "1 year -1 mon";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*11).Ticks, test.TotalTicks, input);

            input = "1 year -2 mons";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*10).Ticks, test.TotalTicks, input);

            input = "1 year -1 day";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*12 - 1).Ticks, test.TotalTicks, input);

            input = "1 year -2 days";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*12 - 2).Ticks, test.TotalTicks, input);

            input = "1 year -1 mon -1 day";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*11 - 1).Ticks, test.TotalTicks, input);

            input = "1 year -2 mons -2 days";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*10 - 2).Ticks, test.TotalTicks, input);

            input = "1 day 2:3:4.005";
            test = NpgsqlInterval.Parse(input);
            Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5).Ticks, test.TotalTicks, input);

            var oldCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var testCulture = new System.Globalization.CultureInfo("fr-FR");
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

            Assert.AreEqual("00:02:03.456789", new NpgsqlInterval(1234567891).ToString());

            Assert.AreEqual("1 day 02:03:04.005",
                            new NpgsqlInterval(new TimeSpan(1, 2, 3, 4, 5)).JustifyInterval().ToString());

            Assert.AreEqual("3 mons 2 days 00:02:03.456789", new NpgsqlInterval(3, 2, 1234567890).ToString());

            Assert.AreEqual("1 day 02:03:04", new NpgsqlInterval(1, 2, 3, 4).ToString());

            Assert.AreEqual("1 day 02:03:04.005", new NpgsqlInterval(1, 2, 3, 4, 5).ToString());

            Assert.AreEqual("1 mon 2 days 03:04:05.006", new NpgsqlInterval(1, 2, 3, 4, 5, 6).ToString());

            Assert.AreEqual("14 mons 3 days 04:05:06.007", new NpgsqlInterval(1, 2, 3, 4, 5, 6, 7).ToString());

            Assert.AreEqual(new NpgsqlInterval(0, 2, 3, 4, 5).ToString(), new NpgsqlInterval(new TimeSpan(0, 2, 3, 4, 5)).ToString());
            
            Assert.AreEqual(new NpgsqlInterval(1, 2, 3, 4, 5).ToString(), new NpgsqlInterval(new TimeSpan(1, 2, 3, 4, 5)).ToString());
            const long moreThanAMonthInTicks = TimeSpan.TicksPerDay*40;
            Assert.AreEqual(new NpgsqlInterval(moreThanAMonthInTicks).ToString(), new NpgsqlInterval(new TimeSpan(moreThanAMonthInTicks)).ToString());
            
            var oldCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var testCulture = new System.Globalization.CultureInfo("fr-FR");
            Assert.AreEqual(",", testCulture.NumberFormat.NumberDecimalSeparator, "decimal seperator");
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = testCulture;
                Assert.AreEqual("14 mons 3 days 04:05:06.007", new NpgsqlInterval(1, 2, 3, 4, 5, 6, 7).ToString());
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = oldCulture;
            }
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

            Assert.AreEqual("00:02:03.456789", new NpgsqlTime(1234567890).ToString());

            Assert.AreEqual("00:02:03.456789", new NpgsqlTime(1234567891).ToString());

            var oldCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var testCulture = new System.Globalization.CultureInfo("fr-FR");
            Assert.AreEqual(",", testCulture.NumberFormat.NumberDecimalSeparator, "decimal seperator");
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = testCulture;
                Assert.AreEqual("00:02:03.456789", new NpgsqlTime(1234567891).ToString());
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }

        [Test]
        public void NpgsqlDateConstructors()
        {
            NpgsqlDate date;
            DateTime dateTime;
            System.Globalization.Calendar calendar = new System.Globalization.GregorianCalendar();

            date = new NpgsqlDate();
            Assert.AreEqual(1, date.Day);
            Assert.AreEqual(DayOfWeek.Monday, date.DayOfWeek);
            Assert.AreEqual(1, date.DayOfYear);
            Assert.AreEqual(false, date.IsLeapYear);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(1, date.Year);

            dateTime = new DateTime(2009, 5, 31);
            date = new NpgsqlDate(dateTime);
            Assert.AreEqual(dateTime.Day, date.Day);
            Assert.AreEqual(dateTime.DayOfWeek, date.DayOfWeek);
            Assert.AreEqual(dateTime.DayOfYear, date.DayOfYear);
            Assert.AreEqual(calendar.IsLeapYear(2009), date.IsLeapYear);
            Assert.AreEqual(dateTime.Month, date.Month);
            Assert.AreEqual(dateTime.Year, date.Year);

            //Console.WriteLine(new DateTime(2009, 5, 31).Ticks);
            //Console.WriteLine((new DateTime(2009, 5, 31) - new DateTime(1, 1, 1)).TotalDays);
            // 2009-5-31
            dateTime = new DateTime(633793248000000000); // ticks since 1 Jan 1
            date = new NpgsqlDate(733557); // days since 1 Jan 1
            Assert.AreEqual(dateTime.Day, date.Day);
            Assert.AreEqual(dateTime.DayOfWeek, date.DayOfWeek);
            Assert.AreEqual(dateTime.DayOfYear, date.DayOfYear);
            Assert.AreEqual(calendar.IsLeapYear(2009), date.IsLeapYear);
            Assert.AreEqual(dateTime.Month, date.Month);
            Assert.AreEqual(dateTime.Year, date.Year);

            // copy previous value.  should get same result
            date = new NpgsqlDate(date);
            Assert.AreEqual(dateTime.Day, date.Day);
            Assert.AreEqual(dateTime.DayOfWeek, date.DayOfWeek);
            Assert.AreEqual(dateTime.DayOfYear, date.DayOfYear);
            Assert.AreEqual(calendar.IsLeapYear(2009), date.IsLeapYear);
            Assert.AreEqual(dateTime.Month, date.Month);
            Assert.AreEqual(dateTime.Year, date.Year);
        }

        [Test]
        public void NpgsqlDateToString()
        {
            Assert.AreEqual("2009-05-31", new NpgsqlDate(2009, 5, 31).ToString());

            Assert.AreEqual("0001-05-07 BC", new NpgsqlDate(-1, 5, 7).ToString());

            var oldCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var testCulture = new System.Globalization.CultureInfo("fr-FR");
            Assert.AreEqual(",", testCulture.NumberFormat.NumberDecimalSeparator, "decimal seperator");
            try
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = testCulture;
                Assert.AreEqual("2009-05-31", new NpgsqlDate(2009, 5, 31).ToString());
            }
            finally
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = oldCulture;
            }
        }

        [Test]
        public void SpecialDates()
        {
            NpgsqlDate date;
            DateTime dateTime;
            System.Globalization.Calendar calendar = new System.Globalization.GregorianCalendar();

            // a date after a leap year.
            dateTime = new DateTime(2008, 5, 31);
            date = new NpgsqlDate(dateTime);
            Assert.AreEqual(dateTime.Day, date.Day);
            Assert.AreEqual(dateTime.DayOfWeek, date.DayOfWeek);
            Assert.AreEqual(dateTime.DayOfYear, date.DayOfYear);
            Assert.AreEqual(calendar.IsLeapYear(2008), date.IsLeapYear);
            Assert.AreEqual(dateTime.Month, date.Month);
            Assert.AreEqual(dateTime.Year, date.Year);

            // A date that is a leap year day.
            dateTime = new DateTime(2000, 2, 29);
            date = new NpgsqlDate(2000, 2, 29);
            Assert.AreEqual(dateTime.Day, date.Day);
            Assert.AreEqual(dateTime.DayOfWeek, date.DayOfWeek);
            Assert.AreEqual(dateTime.DayOfYear, date.DayOfYear);
            Assert.AreEqual(calendar.IsLeapYear(2000), date.IsLeapYear);
            Assert.AreEqual(dateTime.Month, date.Month);
            Assert.AreEqual(dateTime.Year, date.Year);

            // A date that is not in a leap year.
            dateTime = new DateTime(1900, 3, 1);
            date = new NpgsqlDate(1900, 3, 1);
            Assert.AreEqual(dateTime.Day, date.Day);
            Assert.AreEqual(dateTime.DayOfWeek, date.DayOfWeek);
            Assert.AreEqual(dateTime.DayOfYear, date.DayOfYear);
            Assert.AreEqual(calendar.IsLeapYear(1900), date.IsLeapYear);
            Assert.AreEqual(dateTime.Month, date.Month);
            Assert.AreEqual(dateTime.Year, date.Year);

            // a date after a leap year.
            date = new NpgsqlDate(-1, 12, 31);
            Assert.AreEqual(31, date.Day);
            Assert.AreEqual(DayOfWeek.Sunday, date.DayOfWeek);
            Assert.AreEqual(366, date.DayOfYear);
            Assert.AreEqual(true, date.IsLeapYear);
            Assert.AreEqual(12, date.Month);
            Assert.AreEqual(-1, date.Year);
        }

        [Test]
        public void NpgsqlDateMath()
        {
            NpgsqlDate date;

            // add a day to the empty constructor
            date = new NpgsqlDate() + new NpgsqlInterval(0, 1, 0);
            Assert.AreEqual(2, date.Day);
            Assert.AreEqual(DayOfWeek.Tuesday, date.DayOfWeek);
            Assert.AreEqual(2, date.DayOfYear);
            Assert.AreEqual(false, date.IsLeapYear);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(1, date.Year);

            // add a day the same value as the empty constructor
            date = new NpgsqlDate(1, 1, 1) + new NpgsqlInterval(0, 1, 0);
            Assert.AreEqual(2, date.Day);
            Assert.AreEqual(DayOfWeek.Tuesday, date.DayOfWeek);
            Assert.AreEqual(2, date.DayOfYear);
            Assert.AreEqual(false, date.IsLeapYear);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(1, date.Year);

            var diff = new NpgsqlDate(1, 1, 1) - new NpgsqlDate(-1, 12, 31);
            Assert.AreEqual(new NpgsqlInterval(0, 1, 0), diff);

            // Test of the addMonths method (positive values added)
            var dateForTestMonths = new NpgsqlDate(2008, 1, 1);
            Assert.AreEqual(dateForTestMonths.AddMonths(0), dateForTestMonths);
            Assert.AreEqual(dateForTestMonths.AddMonths(4), new NpgsqlDate(2008, 5, 1));
            Assert.AreEqual(dateForTestMonths.AddMonths(11), new NpgsqlDate(2008, 12, 1));
            Assert.AreEqual(dateForTestMonths.AddMonths(12), new NpgsqlDate(2009, 1, 1));
            Assert.AreEqual(dateForTestMonths.AddMonths(14), new NpgsqlDate(2009, 3, 1));
            dateForTestMonths = new NpgsqlDate(2008, 1, 31);
            Assert.AreEqual(dateForTestMonths.AddMonths(1), new NpgsqlDate(2008, 2, 29));
            Assert.AreEqual(dateForTestMonths.AddMonths(13), new NpgsqlDate(2009, 2, 28));

            // Test of the addMonths method (negative values added)
            dateForTestMonths = new NpgsqlDate(2009, 1, 1);
            Assert.AreEqual(dateForTestMonths.AddMonths(0), dateForTestMonths);
            Assert.AreEqual(dateForTestMonths.AddMonths(-4), new NpgsqlDate(2008, 9, 1));
            Assert.AreEqual(dateForTestMonths.AddMonths(-12), new NpgsqlDate(2008, 1, 1));
            Assert.AreEqual(dateForTestMonths.AddMonths(-13), new NpgsqlDate(2007, 12, 1));
            dateForTestMonths = new NpgsqlDate(2009, 3, 31);
            Assert.AreEqual(dateForTestMonths.AddMonths(-1), new NpgsqlDate(2009, 2, 28));
            Assert.AreEqual(dateForTestMonths.AddMonths(-13), new NpgsqlDate(2008, 2, 29));
        }

        [Test]
        public void NpgsqlTimeZoneParse()
        {
            string input;
            NpgsqlTimeZone test;

            input = "+2";
            test = NpgsqlTimeZone.Parse(input);
            Assert.AreEqual(2, test.Hours);
            Assert.AreEqual(0, test.Minutes);
            Assert.AreEqual(0, test.Seconds);

            input = "-2";
            test = NpgsqlTimeZone.Parse(input);
            Assert.AreEqual(-2, test.Hours);
            Assert.AreEqual(0, test.Minutes);
            Assert.AreEqual(0, test.Seconds);

            input = "+3:45";
            test = NpgsqlTimeZone.Parse(input);
            Assert.AreEqual(3, test.Hours);
            Assert.AreEqual(45, test.Minutes);
            Assert.AreEqual(0, test.Seconds);

            input = "-3:45";
            test = NpgsqlTimeZone.Parse(input);
            Assert.AreEqual(-3, test.Hours);
            Assert.AreEqual(-45, test.Minutes);
            Assert.AreEqual(0, test.Seconds);

            input = "+4:30:01";
            test = NpgsqlTimeZone.Parse(input);
            Assert.AreEqual(4, test.Hours);
            Assert.AreEqual(30, test.Minutes);
            Assert.AreEqual(1, test.Seconds);

            input = "-4:30:01";
            test = NpgsqlTimeZone.Parse(input);
            Assert.AreEqual(-4, test.Hours);
            Assert.AreEqual(-30, test.Minutes);
            Assert.AreEqual(-1, test.Seconds);

            input = "+4:30:10";
            test = NpgsqlTimeZone.Parse(input);
            Assert.AreEqual(4, test.Hours);
            Assert.AreEqual(30, test.Minutes);
            Assert.AreEqual(10, test.Seconds);

            input = "-4:30:10";
            test = NpgsqlTimeZone.Parse(input);
            Assert.AreEqual(-4, test.Hours);
            Assert.AreEqual(-30, test.Minutes);
            Assert.AreEqual(-10, test.Seconds);

            input = "-13";
            test = NpgsqlTimeZone.Parse(input);
            Assert.AreEqual(-13, test.Hours);
            Assert.AreEqual(0, test.Minutes);
            Assert.AreEqual(0, test.Seconds);
        }

        [Test]
        public void NpgsqlTimeTzConvert()
        {
            var timetz = new NpgsqlTimeTZ(13, 3, 45.001, new NpgsqlTimeZone(-5, 0));

            Assert.AreEqual(13, timetz.Hours);
            Assert.AreEqual(3, timetz.Minutes);
            Assert.AreEqual(45, timetz.Seconds);
            Assert.AreEqual(1, timetz.Milliseconds);
            Assert.AreEqual(-5, timetz.TimeZone.Hours);
            Assert.AreEqual(0, timetz.TimeZone.Minutes);
            Assert.AreEqual(0, timetz.TimeZone.Seconds);

            Assert.AreEqual(18, timetz.UTCTime.Hours);
            Assert.AreEqual(3, timetz.UTCTime.Minutes);
            Assert.AreEqual(45, timetz.UTCTime.Seconds);
            Assert.AreEqual(1, timetz.UTCTime.Milliseconds);

            var utcOffset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
            // add utc time as timespan to get local time as a timespan
            var localTime = utcOffset + new TimeSpan(0, 18, 3, 45, 1);
            var localDateTime = (DateTime) timetz;

            Assert.AreEqual(localTime.Hours, localDateTime.Hour);
            Assert.AreEqual(localTime.Minutes, localDateTime.Minute);
            Assert.AreEqual(localTime.Seconds, timetz.LocalTime.Seconds);
            Assert.AreEqual(localTime.Milliseconds, timetz.LocalTime.Milliseconds);

            // LocalTime is really time local to the timezone of the TimeTZ.
            Assert.AreEqual(13, timetz.LocalTime.Hours);
            Assert.AreEqual(3, timetz.LocalTime.Minutes);
            Assert.AreEqual(45, timetz.LocalTime.Seconds);
            Assert.AreEqual(1, timetz.LocalTime.Milliseconds);

        }

        [Test]
        public void TsVector()
        {
            NpgsqlTsVector vec;
            
            vec = NpgsqlTsVector.Parse("a");
            Assert.AreEqual("'a'", vec.ToString());

            vec = NpgsqlTsVector.Parse("a ");
            Assert.AreEqual("'a'", vec.ToString());

            vec = NpgsqlTsVector.Parse("a:1A");
            Assert.AreEqual("'a':1A", vec.ToString());

            vec = NpgsqlTsVector.Parse(@"\abc\def:1a ");
            Assert.AreEqual("'abcdef':1A", vec.ToString());

            vec = NpgsqlTsVector.Parse(@"abc:3A 'abc' abc:4B 'hello''yo' 'meh\'\\':5");
            Assert.AreEqual(@"'abc':3A,4B 'hello''yo' 'meh''\\':5", vec.ToString());

            vec = NpgsqlTsVector.Parse(" a:12345C  a:24D a:25B b c d 1 2 a:25A,26B,27,28");
            Assert.AreEqual("'1' '2' 'a':24,25A,26B,27,28,12345C 'b' 'c' 'd'", vec.ToString());
        }

        [Test]
        public void TsQuery()
        {
            NpgsqlTsQuery query;

            query = new NpgsqlTsQueryLexeme("a", NpgsqlTsQueryLexeme.Weight.A | NpgsqlTsQueryLexeme.Weight.B);
            query = new NpgsqlTsQueryOr(query, query);
            query = new NpgsqlTsQueryOr(query, query);

            var str = query.ToString();

            query = NpgsqlTsQuery.Parse("a & b | c");
            Assert.AreEqual("'a' & 'b' | 'c'", query.ToString());

            query = NpgsqlTsQuery.Parse("'a''':*ab&d:d&!c");
            Assert.AreEqual("'a''':*AB & 'd':D & !'c'", query.ToString());

            query = NpgsqlTsQuery.Parse("(a & !(c | d)) & (!!a&b) | c | d | e");
            Assert.AreEqual("( ( 'a' & !( 'c' | 'd' ) & !( !'a' ) & 'b' | 'c' ) | 'd' ) | 'e'", query.ToString());
            Assert.AreEqual(query.ToString(), NpgsqlTsQuery.Parse(query.ToString()).ToString());

            query = NpgsqlTsQuery.Parse("(((a:*)))");
            Assert.AreEqual("'a':*", query.ToString());

            query = NpgsqlTsQuery.Parse(@"'a\\b''cde'");
            Assert.AreEqual(@"a\b'cde", ((NpgsqlTsQueryLexeme)query).Text);
            Assert.AreEqual(@"'a\\b''cde'", query.ToString());

            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("a b c & &"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("&"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("|"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("!"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("("));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse(")"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("()"));
        }

        [Test]
        public void Bug1011018()
        {
            var p = new NpgsqlParameter();
            p.NpgsqlDbType = NpgsqlDbType.Time;
            p.Value = DateTime.Now;
            Object o = p.Value;
        }
    }
}
