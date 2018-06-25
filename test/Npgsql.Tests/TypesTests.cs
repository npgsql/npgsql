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
using System.Data;
using System.Globalization;
using System.Net;
using System.Threading;
using NpgsqlTypes;
using NUnit.Framework;
using Npgsql;

namespace Npgsql.Tests
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
            NpgsqlTimeSpan test;

            input = "1 day";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(1).Ticks, test.TotalTicks, input);

            input = "2 days";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(2).Ticks, test.TotalTicks, input);

            input = "2 days 3:04:05";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(new TimeSpan(2, 3, 4, 5).Ticks, test.TotalTicks, input);

            input = "-2 days";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(-2).Ticks, test.TotalTicks, input);

            input = "-2 days -3:04:05";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(new TimeSpan(-2, -3, -4, -5).Ticks, test.TotalTicks, input);

            input = "-2 days -0:01:02";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(new TimeSpan(-2, 0, -1, -2).Ticks, test.TotalTicks, input);

            input = "2 days -12:00";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(new TimeSpan(2, -12, 0, 0).Ticks, test.TotalTicks, input);

            input = "1 mon";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30).Ticks, test.TotalTicks, input);

            input = "2 mons";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(60).Ticks, test.TotalTicks, input);

            input = "1 mon -1 day";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(29).Ticks, test.TotalTicks, input);

            input = "1 mon -2 days";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(28).Ticks, test.TotalTicks, input);

            input = "-1 mon -2 days -3:04:05";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(new TimeSpan(-32, -3, -4, -5).Ticks, test.TotalTicks, input);

            input = "1 year";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*12).Ticks, test.TotalTicks, input);

            input = "2 years";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*24).Ticks, test.TotalTicks, input);

            input = "1 year -1 mon";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*11).Ticks, test.TotalTicks, input);

            input = "1 year -2 mons";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*10).Ticks, test.TotalTicks, input);

            input = "1 year -1 day";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*12 - 1).Ticks, test.TotalTicks, input);

            input = "1 year -2 days";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*12 - 2).Ticks, test.TotalTicks, input);

            input = "1 year -1 mon -1 day";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*11 - 1).Ticks, test.TotalTicks, input);

            input = "1 year -2 mons -2 days";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(TimeSpan.FromDays(30*10 - 2).Ticks, test.TotalTicks, input);

            input = "1 day 2:3:4.005";
            test = NpgsqlTimeSpan.Parse(input);
            Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5).Ticks, test.TotalTicks, input);

            var testCulture = new CultureInfo("fr-FR");
            Assert.AreEqual(",", testCulture.NumberFormat.NumberDecimalSeparator, "decimal seperator");
            using (new CultureSetter(testCulture))
            {
                input = "1 day 2:3:4.005";
                test = NpgsqlTimeSpan.Parse(input);
                Assert.AreEqual(new TimeSpan(1, 2, 3, 4, 5).Ticks, test.TotalTicks, input);
            }
        }

        [Test]
        public void NpgsqlIntervalConstructors()
        {
            NpgsqlTimeSpan test;

            test = new NpgsqlTimeSpan();
            Assert.AreEqual(0, test.Months, "Months");
            Assert.AreEqual(0, test.Days, "Days");
            Assert.AreEqual(0, test.Hours, "Hours");
            Assert.AreEqual(0, test.Minutes, "Minutes");
            Assert.AreEqual(0, test.Seconds, "Seconds");
            Assert.AreEqual(0, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(0, test.Microseconds, "Microseconds");

            test = new NpgsqlTimeSpan(1234567890);
            Assert.AreEqual(0, test.Months, "Months");
            Assert.AreEqual(0, test.Days, "Days");
            Assert.AreEqual(0, test.Hours, "Hours");
            Assert.AreEqual(2, test.Minutes, "Minutes");
            Assert.AreEqual(3, test.Seconds, "Seconds");
            Assert.AreEqual(456, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(456789, test.Microseconds, "Microseconds");

            test = new NpgsqlTimeSpan(new TimeSpan(1, 2, 3, 4, 5)).JustifyInterval();
            Assert.AreEqual(0, test.Months, "Months");
            Assert.AreEqual(1, test.Days, "Days");
            Assert.AreEqual(2, test.Hours, "Hours");
            Assert.AreEqual(3, test.Minutes, "Minutes");
            Assert.AreEqual(4, test.Seconds, "Seconds");
            Assert.AreEqual(5, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(5000, test.Microseconds, "Microseconds");

            test = new NpgsqlTimeSpan(3, 2, 1234567890);
            Assert.AreEqual(3, test.Months, "Months");
            Assert.AreEqual(2, test.Days, "Days");
            Assert.AreEqual(0, test.Hours, "Hours");
            Assert.AreEqual(2, test.Minutes, "Minutes");
            Assert.AreEqual(3, test.Seconds, "Seconds");
            Assert.AreEqual(456, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(456789, test.Microseconds, "Microseconds");

            test = new NpgsqlTimeSpan(1, 2, 3, 4);
            Assert.AreEqual(0, test.Months, "Months");
            Assert.AreEqual(1, test.Days, "Days");
            Assert.AreEqual(2, test.Hours, "Hours");
            Assert.AreEqual(3, test.Minutes, "Minutes");
            Assert.AreEqual(4, test.Seconds, "Seconds");
            Assert.AreEqual(0, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(0, test.Microseconds, "Microseconds");

            test = new NpgsqlTimeSpan(1, 2, 3, 4, 5);
            Assert.AreEqual(0, test.Months, "Months");
            Assert.AreEqual(1, test.Days, "Days");
            Assert.AreEqual(2, test.Hours, "Hours");
            Assert.AreEqual(3, test.Minutes, "Minutes");
            Assert.AreEqual(4, test.Seconds, "Seconds");
            Assert.AreEqual(5, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(5000, test.Microseconds, "Microseconds");

            test = new NpgsqlTimeSpan(1, 2, 3, 4, 5, 6);
            Assert.AreEqual(1, test.Months, "Months");
            Assert.AreEqual(2, test.Days, "Days");
            Assert.AreEqual(3, test.Hours, "Hours");
            Assert.AreEqual(4, test.Minutes, "Minutes");
            Assert.AreEqual(5, test.Seconds, "Seconds");
            Assert.AreEqual(6, test.Milliseconds, "Milliseconds");
            Assert.AreEqual(6000, test.Microseconds, "Microseconds");

            test = new NpgsqlTimeSpan(1, 2, 3, 4, 5, 6, 7);
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
            Assert.AreEqual("00:00:00", new NpgsqlTimeSpan().ToString());

            Assert.AreEqual("00:02:03.456789", new NpgsqlTimeSpan(1234567890).ToString());

            Assert.AreEqual("00:02:03.456789", new NpgsqlTimeSpan(1234567891).ToString());

            Assert.AreEqual("1 day 02:03:04.005",
                            new NpgsqlTimeSpan(new TimeSpan(1, 2, 3, 4, 5)).JustifyInterval().ToString());

            Assert.AreEqual("3 mons 2 days 00:02:03.456789", new NpgsqlTimeSpan(3, 2, 1234567890).ToString());

            Assert.AreEqual("1 day 02:03:04", new NpgsqlTimeSpan(1, 2, 3, 4).ToString());

            Assert.AreEqual("1 day 02:03:04.005", new NpgsqlTimeSpan(1, 2, 3, 4, 5).ToString());

            Assert.AreEqual("1 mon 2 days 03:04:05.006", new NpgsqlTimeSpan(1, 2, 3, 4, 5, 6).ToString());

            Assert.AreEqual("14 mons 3 days 04:05:06.007", new NpgsqlTimeSpan(1, 2, 3, 4, 5, 6, 7).ToString());

            Assert.AreEqual(new NpgsqlTimeSpan(0, 2, 3, 4, 5).ToString(), new NpgsqlTimeSpan(new TimeSpan(0, 2, 3, 4, 5)).ToString());

            Assert.AreEqual(new NpgsqlTimeSpan(1, 2, 3, 4, 5).ToString(), new NpgsqlTimeSpan(new TimeSpan(1, 2, 3, 4, 5)).ToString());
            const long moreThanAMonthInTicks = TimeSpan.TicksPerDay*40;
            Assert.AreEqual(new NpgsqlTimeSpan(moreThanAMonthInTicks).ToString(), new NpgsqlTimeSpan(new TimeSpan(moreThanAMonthInTicks)).ToString());

            var testCulture = new CultureInfo("fr-FR");
            Assert.AreEqual(",", testCulture.NumberFormat.NumberDecimalSeparator, "decimal seperator");
            using (new CultureSetter(testCulture))
            {
                Assert.AreEqual("14 mons 3 days 04:05:06.007", new NpgsqlTimeSpan(1, 2, 3, 4, 5, 6, 7).ToString());
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

            var testCulture = new CultureInfo("fr-FR");
            Assert.AreEqual(",", testCulture.NumberFormat.NumberDecimalSeparator, "decimal seperator");
            using (new CultureSetter(testCulture))
                Assert.AreEqual("2009-05-31", new NpgsqlDate(2009, 5, 31).ToString());
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
            date = new NpgsqlDate() + new NpgsqlTimeSpan(0, 1, 0);
            Assert.AreEqual(2, date.Day);
            Assert.AreEqual(DayOfWeek.Tuesday, date.DayOfWeek);
            Assert.AreEqual(2, date.DayOfYear);
            Assert.AreEqual(false, date.IsLeapYear);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(1, date.Year);

            // add a day the same value as the empty constructor
            date = new NpgsqlDate(1, 1, 1) + new NpgsqlTimeSpan(0, 1, 0);
            Assert.AreEqual(2, date.Day);
            Assert.AreEqual(DayOfWeek.Tuesday, date.DayOfWeek);
            Assert.AreEqual(2, date.DayOfYear);
            Assert.AreEqual(false, date.IsLeapYear);
            Assert.AreEqual(1, date.Month);
            Assert.AreEqual(1, date.Year);

            var diff = new NpgsqlDate(1, 1, 1) - new NpgsqlDate(-1, 12, 31);
            Assert.AreEqual(new NpgsqlTimeSpan(0, 1, 0), diff);

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

            query = NpgsqlTsQuery.Parse(@"a <-> b");
            Assert.AreEqual("'a' <-> 'b'", query.ToString());

            query = NpgsqlTsQuery.Parse("((a & b) <5> c) <-> !d <0> e");
            Assert.AreEqual("( ( 'a' & 'b' <5> 'c' ) <-> !'d' ) <0> 'e'", query.ToString());

            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("a b c & &"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("&"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("|"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("!"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("("));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse(")"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("()"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("<"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("<-"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("<->"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("a <->"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("<>"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("a <a> b"));
            Assert.Throws(typeof(FormatException), () => NpgsqlTsQuery.Parse("a <-1> b"));
        }

        [Test]
        public void TsQueryOperatorPrecedence()
        {
            var query = NpgsqlTsQuery.Parse("!a <-> b & c | d & e");
            var expectedGrouping = NpgsqlTsQuery.Parse("((!(a) <-> b) & c) | (d & e)");
            Assert.AreEqual(expectedGrouping.ToString(), query.ToString());
        }

        [Test]
        public void Bug1011018()
        {
            var p = new NpgsqlParameter();
            p.NpgsqlDbType = NpgsqlDbType.Time;
            p.Value = DateTime.Now;
            var o = p.Value;
        }

#pragma warning disable 618
        [Test]
        [IssueLink("https://github.com/npgsql/npgsql/issues/750")]
        public void NpgsqlInet()
        {
            var v = new NpgsqlInet(IPAddress.Parse("2001:1db8:85a3:1142:1000:8a2e:1370:7334"), 32);
            Assert.That(v.ToString(), Is.EqualTo("2001:1db8:85a3:1142:1000:8a2e:1370:7334/32"));

            Assert.That(v != null);  // #776
        }
#pragma warning restore 618
    }
}
