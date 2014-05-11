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
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Common;
using System.Xml;
using System.IO;

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


        [Test]
        public void TestSchemaInformation()
        {
            // Obtain Npgsql.NpgsqlServices, Npgsql.EntityFramework via EntityFramework 6.0
            DbProviderServices providerServices = DbProviderServices.GetProviderServices(Conn);
            Assert.IsNotNull(providerServices);

            // 9.3.1 or such
            string providerManifestToken = providerServices.GetProviderManifestToken(Conn);
            Assert.IsNotNullOrEmpty(providerManifestToken);

            // Obtain NpgsqlProviderManifest
            DbProviderManifest providerManifest = providerServices.GetProviderManifest(providerManifestToken) as DbProviderManifest;
            Assert.IsNotNull(providerManifest);

            // ssdl v1 from Npgsql.EntityFramework
            XmlReader xmlSsdl = providerManifest.GetInformation("StoreSchemaDefinition") as XmlReader;
            Assert.IsNotNull(xmlSsdl);

            // msl v1 from Npgsql.EntityFramework
            XmlReader xmlMsl = providerManifest.GetInformation("StoreSchemaMapping") as XmlReader;
            Assert.IsNotNull(xmlMsl);

            // csdl v1 from EntityFramework 6.0
            XmlReader xmlCsdl = DbProviderServices.GetConceptualSchemaDefinition(DbProviderManifest.ConceptualSchemaDefinition);
            Assert.IsNotNull(xmlCsdl);

            // Create temp xml files. need cleaning!
            string tmp = Guid.NewGuid().ToString("N");
            string csdl = Path.Combine(Path.GetTempPath(), tmp + ".csdl");
            string ssdl = Path.Combine(Path.GetTempPath(), tmp + ".ssdl");
            string msl = Path.Combine(Path.GetTempPath(), tmp + ".msl");

            XmlDocument xmldom = new XmlDocument();
            xmldom.Load(xmlCsdl);
            xmldom.Save(csdl);

            xmldom.Load(xmlSsdl);
            xmldom.Save(ssdl);

            xmldom.Load(xmlMsl);
            xmldom.Save(msl);

            // http://msdn.microsoft.com/en-US/library/bb738533(v=vs.110).aspx

            EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();

            entityBuilder.Provider = "Npgsql";
            entityBuilder.ProviderConnectionString = ConnectionStringEF;
            entityBuilder.Metadata = csdl + "|" + ssdl + "|" + msl;

            using (var context = new Store.SchemaInformation(new EntityConnection(entityBuilder.ToString())))
            {
                var tables = context.Tables;
                Assert.Contains("Blogs", tables.Select(p => p.Name).ToArray());
                Assert.Contains("Posts", tables.Select(p => p.Name).ToArray());

                //var functions = context.Functions;
                //Assert.Contains("pass_thru_int", functions.Select(p => p.Name).ToArray());
                //Assert.Contains("pass_thru_str", functions.Select(p => p.Name).ToArray());

                var tableConstraints = context.TableConstraints;
                Assert.Contains("Blogs.BlogId", tableConstraints.OfType<Store.PrimaryKeyConstraint>().SelectMany(p => p.Columns).Select(p => p.Parent.Name + "." + p.Name).ToArray());
                Assert.Contains("Posts.PostId", tableConstraints.OfType<Store.PrimaryKeyConstraint>().SelectMany(p => p.Columns).Select(p => p.Parent.Name + "." + p.Name).ToArray());

                Assert.Contains("Posts.BlogId->Blogs.BlogId", tableConstraints.OfType<Store.ForeignKeyConstraint>().SelectMany(p => p.ForeignKeys).Select(p => p.FromColumn.Parent.Name + "." + p.FromColumn.Name + "->" + p.ToColumn.Parent.Name + "." + p.ToColumn.Name).ToArray());
            }
        }


        //Hunting season is open Happy hunting on OrderBy,GroupBy,Min,Max,Skip,Take,ThenBy... and all posible combinations
    }
}
#endif