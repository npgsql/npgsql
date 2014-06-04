#if NET40
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

            public int BlogId { get; set; }
            public virtual Blog Blog { get; set; }
        }

        public class BloggingContext : DbContext
        {
            public BloggingContext(string connection)
                : base(new NpgsqlConnection(connection), true)
            {
            }

            public DbSet<Blog> Blogs { get; set; }
            public DbSet<Post> Posts { get; set; }
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


        //Hunting season is open Happy hunting on OrderBy,GroupBy,Min,Max,Skip,Take,ThenBy... and all posible combinations
        
        [Test]
        public void TestComplicatedQueries()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                // Test Apply
                (from t1 in context.Blogs
                 from t2 in context.Posts.Where(p => p.BlogId == t1.BlogId).Take(1)
                 select new { t1, t2 }).ToArray();

                // Test that the subqueries are evaluated in the correct order
                context.Posts.Select(p => p.Content).Distinct().OrderBy(l => l).ToArray();
                context.Posts.OrderByDescending(p => p.BlogId).Take(2).OrderBy(p => p.BlogId).Skip(1).Take(1).Select(l => l.BlogId).ToArray();
                context.Posts.Take(3).Take(4).ToArray();
                context.Posts.OrderByDescending(p => p.BlogId).Take(3).OrderBy(p => p.BlogId).Take(2).ToArray();
                context.Posts.OrderByDescending(p => p.BlogId).Take(3).OrderBy(p => p.BlogId).ToArray();

                // Test that lhs and rhs of UNION ALL is wrapped in parentheses
                context.Blogs.Take(3).Concat(context.Blogs.Take(4)).ToArray();

                // In
                int[] arr = {1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40};
                context.Blogs.Where(b => arr.Contains(b.BlogId)).ToArray();

                // Subquery as a select column
                context.Blogs.Select(b => new { b.Name, b.BlogId, c = context.Posts.Count(p => p.BlogId == b.BlogId) + 1 }).ToArray();
                context.Blogs.Where(b => b.Name == context.Blogs.FirstOrDefault().Name).ToArray();

                context.Blogs.Where(b => b.Name == context.Blogs.Where(b2 => b2.BlogId < 100).FirstOrDefault().Name).ToArray();

                Action<string> elinq = (string query) => {
                    new System.Data.Entity.Core.Objects.ObjectQuery<System.Data.Common.DbDataRecord>(query, ((System.Data.Entity.Infrastructure.IObjectContextAdapter)context).ObjectContext).ToArray();
                };

                elinq("Select a, max(b) from {1,2,3,3,4} as b group by b as a");
                elinq("Select (select count(a.Name) from ((select a1.Name from Blogs as a1) union all (select b2.Name from Blogs as b2)) as a) as cnt, b.BlogId from Posts as b");

                elinq("Select a.ID from (select distinct c.ID, c.ID2 From (select b.Name as ID, b.Name = '' as ID2 From Blogs as b order by ID asc limit 3) as c) as a where a.ID = 'a'");

                // Joins, apply
                elinq("Select value Blogs.BlogId From Blogs outer apply (Select p1.BlogId as bid, p1.PostId as bid2 from Posts as p1 left outer join (Select value p.PostId from Posts as p where p.PostId < Blogs.BlogId)) as b outer apply (Select p.PostId from Posts as p where p.PostId < b.bid)");

                // a LEFT JOIN b LEFT JOIN c ON x ON y => Parsed as: a LEFT JOIN (b LEFT JOIN c ON x) ON y, which is correct
                elinq("Select a.BlogId, d.id2, d.id3 from Blogs as a left outer join (Select b.BlogId as id2, c.BlogId as id3 From Blogs as b left outer join Blogs as c on true) as d on true");
                // Aliasing
                elinq("Select a.BlogId, d.id2, d.id3 from Blogs as a left outer join (Select top(1) b.BlogId as id2, c.BlogId as id3 From Blogs as b left outer join Blogs as c on true) as d on true");

                // Anyelement (creates DbNewInstanceExpressions)
                elinq("Anyelement (Select Blogs.BlogId, Blogs.Name from Blogs)");
                elinq("Select a.BlogId, Anyelement (Select value b.BlogId + 1 from Blogs as b) as c from Blogs as a");

                // Just some really crazy query
                context.Blogs.Select(b => new { b, b.BlogId, n = b.Posts.Select(p => new { t = p.Title + b.Name, n = p.Blog.Posts.Count(p2 => p2.BlogId < 4) }).Take(2) }).ToArray();
            }
        }
    }
}
#endif
