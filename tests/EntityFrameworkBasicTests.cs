using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;

namespace NpgsqlTests
{
    [TestFixture]
    public class EntityFrameworkBasicTests : TestBase
    {
        public EntityFrameworkBasicTests(string backendVersion)
            : base(backendVersion)
        {
        }

        [TestFixtureSetUp]
        public override void TestFixtureSetup()
        {
            base.TestFixtureSetup();
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                if (context.Database.Exists())
                    context.Database.Delete();//We delete to be 100% schema is synced
                context.Database.Create();
            }

            // Create sequence for the IntComputedValue property.
            using (var createSequenceConn = new NpgsqlConnection(ConnectionStringEF))
            {
                createSequenceConn.Open();
                ExecuteNonQuery("create sequence blog_int_computed_value_seq", createSequenceConn);
                ExecuteNonQuery("alter table \"dbo\".\"Blogs\" alter column \"IntComputedValue\" set default nextval('blog_int_computed_value_seq');", createSequenceConn);

            }
            

        }

        /// <summary>
        /// Clean any previous entites before our test
        /// </summary>
        [SetUp]
        protected override void SetUp()
        {
            base.SetUp();
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Blogs.RemoveRange(context.Blogs);
                context.Posts.RemoveRange(context.Posts);
                context.SaveChanges();
            }
        }

        public class Blog
        {
            public int BlogId { get; set; }
            public string Name { get; set; }

            public virtual List<Post> Posts { get; set; }

            [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
            public int IntComputedValue { get; set; }
        }

        public class Post
        {
            public int PostId { get; set; }
            public string Title { get; set; }
            public string Content { get; set; }
            public byte Rating { get; set; }
            public DateTime CreationDate { get; set; }
            public int BlogId { get; set; }
            public virtual Blog Blog { get; set; }
        }

        public class NoColumnsEntity
        {
            public int Id { get; set; }
        }

        public class BloggingContext : DbContext
        {
            public BloggingContext(string connection)
                : base(new NpgsqlConnection(connection), true)
            {
            }

            public DbSet<Blog> Blogs { get; set; }
            public DbSet<Post> Posts { get; set; }
            public DbSet<NoColumnsEntity> NoColumnsEntities { get; set; }
        }

        [Test]
        public void InsertAndSelect()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                var blog = new Blog()
                {
                    Name = "Some blog name"
                };
                blog.Posts = new List<Post>();
                for (int i = 0; i < 5; i++)
                    blog.Posts.Add(new Post()
                    {
                        Content = "Some post content " + i,
                        Rating = (byte)i,
                        Title = "Some post Title " + i
                    });
                context.Blogs.Add(blog);
                context.NoColumnsEntities.Add(new NoColumnsEntity());
                context.SaveChanges();
            }

            using (var context = new BloggingContext(ConnectionStringEF))
            {
                var posts = from p in context.Posts
                            select p;
                Assert.AreEqual(5, posts.Count());
                foreach (var post in posts)
                {
                    StringAssert.StartsWith("Some post Title ", post.Title);
                }
                Assert.AreEqual(1, context.NoColumnsEntities.Count());
            }
        }

        [Test]
        public void SelectWithWhere()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                var blog = new Blog()
                {
                    Name = "Some blog name"
                };
                blog.Posts = new List<Post>();
                for (int i = 0; i < 5; i++)
                    blog.Posts.Add(new Post()
                    {
                        Content = "Some post content " + i,
                        Rating = (byte)i,
                        Title = "Some post Title " + i
                    });
                context.Blogs.Add(blog);
                context.SaveChanges();
            }

            using (var context = new BloggingContext(ConnectionStringEF))
            {
                var posts = from p in context.Posts
                            where p.Rating < 3
                            select p;
                Assert.AreEqual(3, posts.Count());
                foreach (var post in posts)
                {
                    Assert.Less(post.Rating, 3);
                }
            }
        }
        
        [Test]
        public void SelectWithWhere_Ef_TruncateTime()
        {
            DateTime createdOnDate = new DateTime(2014, 05, 08);
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                var blog = new Blog()
                {
                    Name = "Some blog name"
                };
                blog.Posts = new List<Post>();

                for (int i = 0; i < 5; i++)
                    blog.Posts.Add(new Post()
                    {
                        Content = "Some post content " + i,
                        Rating = (byte)i,
                        Title = "Some post Title " + i,
                        CreationDate = createdOnDate.AddHours(i)
                    });
                context.Blogs.Add(blog);
                context.SaveChanges();
            }

            using (var context = new BloggingContext(ConnectionStringEF))
            {
                var posts = from p in context.Posts
                            let datePosted = DbFunctions.TruncateTime(p.CreationDate)
                            where p.Rating < 3 && datePosted == createdOnDate
                            select p;
                Assert.AreEqual(3, posts.Count());
                foreach (var post in posts)
                {
                    Assert.Less(post.Rating, 3);
                }
            }
        }

		[Test]
		public void SelectWithLike_SpecialCharacters()
		{
			DateTime createdOnDate = new DateTime(2014, 05, 08);
			using (var context = new BloggingContext(ConnectionStringEF))
			{
				var blog = new Blog()
				{
					Name = "Special Characters Test"
				};
				blog.Posts = new List<Post>();

				blog.Posts.Add(new Post()
				{
					Content = "C:\\blog\\Some_post_title%",
					Rating = (byte)1,
					Title = "Some post Title ",
					CreationDate = createdOnDate.AddHours(1)
				});
				blog.Posts.Add(new Post()
				{
					Content = "C:\\blog\\Some_post_title\\",
					Rating = (byte)2,
					Title = "Some post Title ",
					CreationDate = createdOnDate.AddHours(2)
				});
				blog.Posts.Add(new Post()
				{
					Content = "%Test",
					Rating = (byte)3,
					Title = "Some post Title ",
					CreationDate = createdOnDate.AddHours(3)
				});
				context.Blogs.Add(blog);
				context.SaveChanges();
			}

			using (var context = new BloggingContext(ConnectionStringEF))
			{
				var posts1 = from p in context.Posts
				             where p.Content.Contains("_")
				             select p;
				Assert.AreEqual(2, posts1.Count());

				var posts2 = from p in context.Posts
				             where p.Content.EndsWith("\\")
				             select p;
				Assert.AreEqual(1, posts2.Count());

				var posts3 = from p in context.Posts
				             where p.Content.StartsWith("%")
				             select p;
				Assert.AreEqual(1, posts3.Count());
			}
		}

        [Test]
        public void OrderBy()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                Random random = new Random();
                var blog = new Blog()
                {
                    Name = "Some blog name"
                };

                blog.Posts = new List<Post>();
                for (int i = 0; i < 10; i++)
                    blog.Posts.Add(new Post()
                    {
                        Content = "Some post content " + i,
                        Rating = (byte)random.Next(0, 255),
                        Title = "Some post Title " + i
                    });
                context.Blogs.Add(blog);
                context.SaveChanges();
            }

            using (var context = new BloggingContext(ConnectionStringEF))
            {
                var posts = from p in context.Posts
                            orderby p.Rating
                            select p;
                Assert.AreEqual(10, posts.Count());
                byte previousValue = 0;
                foreach (var post in posts)
                {
                    Assert.GreaterOrEqual(post.Rating, previousValue);
                    previousValue = post.Rating;
                }
            }
        }

        [Test]
        public void OrderByThenBy()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                Random random = new Random();
                var blog = new Blog()
                {
                    Name = "Some blog name"
                };

                blog.Posts = new List<Post>();
                for (int i = 0; i < 10; i++)
                    blog.Posts.Add(new Post()
                    {
                        Content = "Some post content " + i,
                        Rating = (byte)random.Next(0, 255),
                        Title = "Some post Title " + (i % 3)
                    });
                context.Blogs.Add(blog);
                context.SaveChanges();
            }

            using (var context = new BloggingContext(ConnectionStringEF))
            {
                var posts = context.Posts.AsQueryable<Post>().OrderBy((p) => p.Title).ThenByDescending((p) => p.Rating);
                Assert.AreEqual(10, posts.Count());
                foreach (var post in posts)
                {
                    //TODO: Check outcome
                    Console.WriteLine(post.Title + " " + post.Rating);
                }
            }
        }

        [Test]
        public void TestComputedValue()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                var blog = new Blog()
                {
                    Name = "Some blog name"
                };

                context.Blogs.Add(blog);
                context.SaveChanges();

                Assert.Greater(blog.BlogId, 0);
                Assert.Greater(blog.IntComputedValue, 0);
            }

        }

        [Test]
        public void Operators()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                int one = 1, two = 2, three = 3, four = 4;
                bool True = true, False = false;
                bool[] boolArr = { true, false };
                IQueryable<int> oneRow = context.Posts.Where(p => false).Select(p => 1).Concat(new int[] { 1 });
                Assert.AreEqual(oneRow.Select(p => one & (two ^ three)).First(), 1);
                Assert.AreEqual(oneRow.Select(p => ~(one & two)).First(), ~(one & two));
                Assert.AreEqual(oneRow.Select(p => one + ~(two * three) + ~(two ^ ~three) - one ^ three * ~two / three | four).First(),
                                                   one + ~(two * three) + ~(two ^ ~three) - one ^ three * ~two / three | four);
                Assert.AreEqual(oneRow.Select(p => one - (two - three) - four - (- one - two) - (- three)).First(),
                                                   one - (two - three) - four - (- one - two) - (- three));
                Assert.AreEqual(oneRow.Select(p => boolArr.Contains(True == true)).First(), true);
                Assert.AreEqual(oneRow.Select(p => !boolArr.Contains(False == true)).First(), false);
                Assert.AreEqual(oneRow.Select(p => !boolArr.Contains(False != true)).First(), false);
            }
        }

        [Test]
        public void DataTypes()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                IQueryable<int> oneRow = context.Posts.Where(p => false).Select(p => 1).Concat(new int[] { 1 });

                Assert.AreEqual((byte)1, oneRow.Select(p => (byte)1).First());
                Assert.AreEqual((sbyte)1, oneRow.Select(p => (sbyte)1).First());
                Assert.AreEqual((short)1, oneRow.Select(p => (short)1).First());
                Assert.AreEqual((long)1, oneRow.Select(p => (long)1).First());
                Assert.AreEqual(1.25M, oneRow.Select(p => 1.25M).First());
                Assert.AreEqual(double.NaN, oneRow.Select(p => double.NaN).First());
                Assert.AreEqual(double.PositiveInfinity, oneRow.Select(p => double.PositiveInfinity).First());
                Assert.AreEqual(double.NegativeInfinity, oneRow.Select(p => double.NegativeInfinity).First());
                Assert.AreEqual(1.12e+12, oneRow.Select(p => 1.12e+12).First());
                Assert.AreEqual(1.12e-12, oneRow.Select(p => 1.12e-12).First());
                Assert.AreEqual(float.NaN, oneRow.Select(p => float.NaN).First());
                Assert.AreEqual(float.PositiveInfinity, oneRow.Select(p => float.PositiveInfinity).First());
                Assert.AreEqual(float.NegativeInfinity, oneRow.Select(p => float.NegativeInfinity).First());
                Assert.AreEqual(1.12e+12f, oneRow.Select(p => 1.12e+12f).First());
                Assert.AreEqual(1.12e-12f, oneRow.Select(p => 1.12e-12f).First());
                Assert.AreEqual((short)-32768, oneRow.Select(p => (short)-32768).First());

                // A literal TimeSpan is written as an interval
                Assert.AreEqual(new TimeSpan(1, 2, 3, 4), oneRow.Select(p => new TimeSpan(1, 2, 3, 4)).First());
                var val1 = new TimeSpan(1, 2, 3, 4);
                Assert.AreEqual(val1, oneRow.Select(p => new TimeSpan(1, 2, 3, 4)).First());
                Assert.AreEqual(val1, oneRow.Select(p => val1).First());

                // DateTimeOffset -> timestamptz
                Assert.AreEqual(new DateTimeOffset(2014, 2, 3, 4, 5, 6, 0, TimeSpan.Zero), oneRow.Select(p => new DateTimeOffset(2014, 2, 3, 4, 5, 6, 0, TimeSpan.Zero)).First());
                var val2 = new DateTimeOffset(2014, 2, 3, 4, 5, 6, 0, TimeSpan.Zero);
                Assert.AreEqual(val2, oneRow.Select(p => new DateTimeOffset(2014, 2, 3, 4, 5, 6, 0, TimeSpan.Zero)).First());
                Assert.AreEqual(val2, oneRow.Select(p => val2).First());

                // DateTime -> timestamp
                Assert.AreEqual(new DateTime(2014, 2, 3, 4, 5, 6, 0), oneRow.Select(p => new DateTime(2014, 2, 3, 4, 5, 6, 0)).First());
                var val3 = new DateTime(2014, 2, 3, 4, 5, 6, 0);
                Assert.AreEqual(val3, oneRow.Select(p => new DateTime(2014, 2, 3, 4, 5, 6, 0)).First());
                Assert.AreEqual(val3, oneRow.Select(p => val3).First());

                var val4 = new Guid("1234567890abcdef1122334455667788");
                Assert.AreEqual(val4, oneRow.Select(p => new Guid("1234567890abcdef1122334455667788")).First());
                Assert.AreEqual(val4, oneRow.Select(p => val4).First());

                // String
                Assert.AreEqual(@"a'b\c", oneRow.Select(p => @"a'b\c").First());
            }
        }

        [Test]
        public void DateFunctions()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                IQueryable<int> oneRow = context.Posts.Where(p => false).Select(p => 1).Concat(new int[] { 1 });

                var dateAdds = oneRow.Select(p => new List<DateTime?>
                {
                    DbFunctions.AddDays(new DateTime(2014, 2, 28), 1),
                    DbFunctions.AddHours(new DateTime(2014, 2, 28, 23, 0, 0), 1),
                    DbFunctions.AddMinutes(new DateTime(2014, 2, 28, 23, 59, 0), 1),
                    DbFunctions.AddSeconds(new DateTime(2014, 2, 28, 23, 59, 59), 1),
                    DbFunctions.AddMilliseconds(new DateTime(2014, 2, 28, 23, 59, 59, 999), 2 - p),
                    DbFunctions.AddMicroseconds(DbFunctions.AddMicroseconds(new DateTime(2014, 2, 28, 23, 59, 59, 999), 500), 500),
                    DbFunctions.AddNanoseconds(new DateTime(2014, 2, 28, 23, 59, 59, 999), 999999 + p),
                    DbFunctions.AddMonths(new DateTime(2014, 2, 1), 1),
                    DbFunctions.AddYears(new DateTime(2013, 3, 1), 1)
                }).First();
                foreach (var result in dateAdds)
                {
                    Assert.IsTrue(result.Value == new DateTime(2014, 3, 1, 0, 0, 0));
                }

                var dateDiffs = oneRow.Select(p => new {
                    a = DbFunctions.DiffDays(new DateTime(1999, 12, 31, 23, 59, 59, 999), new DateTime(2000, 1, 1, 0, 0, 0)),
                    b = DbFunctions.DiffHours(new DateTime(1999, 12, 31, 23, 59, 59, 999), new DateTime(2000, 1, 1, 0, 0, 0)),
                    c = DbFunctions.DiffMinutes(new DateTime(1999, 12, 31, 23, 59, 59, 999), new DateTime(2000, 1, 1, 0, 0, 0)),
                    d = DbFunctions.DiffSeconds(new DateTime(1999, 12, 31, 23, 59, 59, 999), new DateTime(2000, 1, 1, 0, 0, 0)),
                    e = DbFunctions.DiffMilliseconds(new DateTime(1999, 12, 31, 23, 59, 59, 999), new DateTime(2000, 1, 1, 0, 0, 0)),
                    f = DbFunctions.DiffMicroseconds(new DateTime(1999, 12, 31, 23, 59, 59, 999), new DateTime(2000, 1, 1, 0, 0, 0)),
                    g = DbFunctions.DiffNanoseconds(new DateTime(1999, 12, 31, 23, 59, 59, 999), new DateTime(2000, 1, 1, 0, 0, 0)),
                    h = DbFunctions.DiffMonths(new DateTime(1999, 12, 31, 23, 59, 59, 999), new DateTime(3000, 1, 1, 0, 0, 0)),
                    i = DbFunctions.DiffYears(new DateTime(1999, 12, 31, 23, 59, 59, 999), new DateTime(3000, 1, 1, 0, 0, 0)),
                    j = DbFunctions.DiffYears(null, new DateTime(2000, 1, 1)),
                    k = DbFunctions.DiffMinutes(new TimeSpan(1, 2, 3), new TimeSpan(4, 5, 6)),
                    l = DbFunctions.DiffMinutes(new TimeSpan(1, 2, 3), null)
                }).First();
                Assert.AreEqual(dateDiffs.a, 1);
                Assert.AreEqual(dateDiffs.b, 1);
                Assert.AreEqual(dateDiffs.c, 1);
                Assert.AreEqual(dateDiffs.d, 1);
                Assert.AreEqual(dateDiffs.e, 1);
                Assert.AreEqual(dateDiffs.f, 1000);
                Assert.AreEqual(dateDiffs.g, 1000000);
                Assert.AreEqual(dateDiffs.h, 12001);
                Assert.AreEqual(dateDiffs.i, 1001);
                Assert.AreEqual(dateDiffs.j, null);
                Assert.AreEqual(dateDiffs.k, 183);
                Assert.AreEqual(dateDiffs.l, null);
            }
        }

        //Hunting season is open Happy hunting on OrderBy,GroupBy,Min,Max,Skip,Take,ThenBy... and all posible combinations
        
        [Test]
        public void TestComplicatedQueries()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                // Test that the subqueries are evaluated in the correct order
                context.Posts.Select(p => p.Content).Distinct().OrderBy(l => l).ToArray();
                context.Posts.OrderByDescending(p => p.BlogId).Take(2).OrderBy(p => p.BlogId).Skip(1).Take(1).Select(l => l.BlogId).ToArray();
                context.Posts.Take(3).Take(4).ToArray();
                context.Posts.OrderByDescending(p => p.BlogId).Take(3).OrderBy(p => p.BlogId).Take(2).ToArray();
                context.Posts.OrderByDescending(p => p.BlogId).Take(3).OrderBy(p => p.BlogId).ToArray();

                // Test that lhs and rhs of UNION ALL is wrapped in parentheses
                context.Blogs.Take(3).Concat(context.Blogs.Take(4)).ToArray();

                // Flatten set ops
                context.Blogs.Concat(context.Blogs).Concat(context.Blogs.Concat(context.Blogs)).ToArray();
                context.Blogs.Intersect(context.Blogs).Intersect(context.Blogs).ToArray();
                // But not except
                context.Blogs.Concat(context.Blogs.Except(context.Blogs)).ToArray();

                // In
                int[] arr = {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40};
                context.Blogs.Where(b => arr.Contains(b.BlogId)).ToArray();

                // Subquery as a select column
                context.Blogs.Select(b => new { b.Name, b.BlogId, c = context.Posts.Count(p => p.BlogId == b.BlogId) + 1 }).ToArray();
                context.Blogs.Where(b => b.Name == context.Blogs.FirstOrDefault().Name).ToArray();

                context.Blogs.Where(b => b.Name == context.Blogs.Where(b2 => b2.BlogId < 100).FirstOrDefault().Name).ToArray();

                // Similar to https://github.com/npgsql/Npgsql/issues/156 However EF is turning the GroupBy into a Distinct here
                context.Posts.OrderBy(p => p.Title).ThenBy(p => p.Content).Take(100).GroupBy(p => p.Title).Select(p => p.Key).ToArray();

                // Check precedence for ||
                // http://stackoverflow.com/questions/21908464/wrong-query-generated-by-postgresql-provider-to-entity-framework-for-contains-an
                context.Posts.Where(p => "a" != string.Concat("a", "b")).ToArray();

                Action<string> elinq = (string query) => {
                    new System.Data.Entity.Core.Objects.ObjectQuery<System.Data.Common.DbDataRecord>(query, ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext).ToArray();
                };

                elinq("Select a, max(b) from {1,2,3,3,4} as b group by b as a");
                elinq("Select (select count(a.Name) from ((select a1.Name from Blogs as a1) union all (select b2.Name from Blogs as b2)) as a) as cnt, b.BlogId from Posts as b");

                elinq("Select a.ID from (select distinct c.ID, c.ID2 From (select b.Name as ID, b.Name = '' as ID2 From Blogs as b order by ID asc limit 3) as c) as a where a.ID = 'a'");

                // a LEFT JOIN b LEFT JOIN c ON x ON y => Parsed as: a LEFT JOIN (b LEFT JOIN c ON x) ON y, which is correct
                elinq("Select a.BlogId, d.id2, d.id3 from Blogs as a left outer join (Select b.BlogId as id2, c.BlogId as id3 From Blogs as b left outer join Blogs as c on true) as d on true");
                // Aliasing
                elinq("Select a.BlogId, d.id2, d.id3 from Blogs as a left outer join (Select top(1) b.BlogId as id2, c.BlogId as id3 From Blogs as b left outer join Blogs as c on true) as d on true");

                // Anyelement (creates DbNewInstanceExpressions)
                elinq("Anyelement (Select Blogs.BlogId, Blogs.Name from Blogs)");
                elinq("Select a.BlogId, Anyelement (Select value b.BlogId + 1 from Blogs as b) as c from Blogs as a");
            }
        }

        [Test]
        [MonoIgnore("Probably bug in mono. See https://github.com/npgsql/Npgsql/issues/289.")]
        public void TestComplicatedQueriesMonoFails()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                // Similar to https://github.com/npgsql/Npgsql/issues/216
                (from d in context.Posts
                 group d by new { d.Content, d.Title }).FirstOrDefault();

                // NewInstance(Column(Element(Limit(Sort(Project(...))))))
                // https://github.com/npgsql/Npgsql/issues/280
                (from postsGrouped in context.Posts.GroupBy(x => x.BlogId)
                 let lastPostDate = postsGrouped.OrderByDescending(x => x.CreationDate)
                                                                 .Select(x => x.CreationDate)
                                                                  .FirstOrDefault()
                 select new {
                     LastPostDate = lastPostDate
                 }).ToArray();
            }
        }

        [Test]
        public void TestComplicatedQueriesWithApply()
        {
            if ((BackendVersion.Major > 9) || (BackendVersion.Major == 9 && BackendVersion.Minor >= 3))
            {
                using (var context = new BloggingContext(ConnectionStringEF))
                {
                    context.Database.Log = Console.Out.WriteLine;

                    // Test Apply
                    (from t1 in context.Blogs
                     from t2 in context.Posts.Where(p => p.BlogId == t1.BlogId).Take(1)
                     select new { t1, t2 }).ToArray();

                    Action<string> elinq = (string query) => {
                        new System.Data.Entity.Core.Objects.ObjectQuery<System.Data.Common.DbDataRecord>(query, ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext).ToArray();
                    };

                    // Joins, apply
                    elinq("Select value Blogs.BlogId From Blogs outer apply (Select p1.BlogId as bid, p1.PostId as bid2 from Posts as p1 left outer join (Select value p.PostId from Posts as p where p.PostId < Blogs.BlogId)) as b outer apply (Select p.PostId from Posts as p where p.PostId < b.bid)");

                    // Just some really crazy query that results in an apply as well
                    context.Blogs.Select(b => new { b, b.BlogId, n = b.Posts.Select(p => new { t = p.Title + b.Name, n = p.Blog.Posts.Count(p2 => p2.BlogId < 4) }).Take(2) }).ToArray();
                }   
            }
        }
    }
}
