#if CODE_FIRST
using System;
using System.Linq;
using CodeFirst.DataModel;
using NUnit.Framework;

namespace CodeFirst
{
    [TestFixture]
	public class CodeFirstTests
	{
        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            var context = new CodeFirstTestContext();

            context.Database.ExecuteSqlCommand("drop schema if exists code_first cascade;");
            context.Database.ExecuteSqlCommand("create schema code_first;");
            context.Database.ExecuteSqlCommand(@"
create table code_first.entity01
(
   id bigserial not null, 
   field_timestamp   timestamp,
   field_time        time,
   field_interval    interval, 
   primary key (id)
) ");
            context.SaveChanges();
        }

        [Test]
        public void EntityCreateTest()
        {
            var saveContext = new CodeFirstTestContext();
            var loadContext = new CodeFirstTestContext();
            
            // Round for safe comparision on next steps
            var time = TimeSpan.FromSeconds(Math.Round(DateTime.Now.TimeOfDay.TotalSeconds,7));
            var date = DateTime.Now.Date + time;

            var saveToDb = new Entity01
                {
                    TimeStamp = date,
                    Time =  new DateTime(0u) + time,
                    Interval = time                                                          
                };

            saveContext.Entities01.Add(saveToDb);

            saveContext.SaveChanges();

            var loadFromDb = loadContext.Entities01.First(e => e.Id == saveToDb.Id);

            Assert.AreEqual(true, saveToDb.CompareTo(loadFromDb) == 0);
        }

        [Test]
        public void IntervalFieldTest()
        {
            var saveContext = new CodeFirstTestContext();
            var loadContext = new CodeFirstTestContext();

            var timestampsToCheck = new[]
                                        {
                                            TimeSpan.FromDays(-44),
                                            TimeSpan.FromDays(-4),
                                            TimeSpan.FromHours(-3),
                                            TimeSpan.FromMinutes(-2),
                                            TimeSpan.FromSeconds(-1),
                                            TimeSpan.Zero,
                                            TimeSpan.FromDays(1),
                                            TimeSpan.FromHours(2),
                                            TimeSpan.FromMinutes(3),
                                            TimeSpan.FromSeconds(4),
                                            TimeSpan.FromSeconds(44),
                                            new TimeSpan(-1, -2, -3),
                                            new TimeSpan(-1, -2, -3, -4),
                                            new TimeSpan(-1, -2, -3, -4,-567),
                                            new TimeSpan(1, 2, 3),
                                            new TimeSpan(1, 2, 3, 4),
                                            new TimeSpan(1, 2, 3, 4,567),
                                            new TimeSpan(-1, 2, -3),
                                            new TimeSpan(1, -2, 3, -4),
                                            new TimeSpan(-1, 2, -3, 4,-567),
                                            new TimeSpan(-5, 2, -3, 4,-567)
                                        };

            foreach (var timeSpan in timestampsToCheck)
            {
                var entity = new Entity01 { Interval = timeSpan };
                saveContext.Entities01.Add(entity);
                saveContext.SaveChanges();

                var loadFromDb = loadContext.Entities01.First(e => e.Id == entity.Id);

                Assert.AreEqual(entity.Interval, loadFromDb.Interval);
            }
        }
	}
}
#endif
