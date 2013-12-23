using Npgsql;
using NpgsqlTests;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace NpgsqlTests
{
    public class EntityFrameworkTests : TestBase
    {
        public EntityFrameworkTests(string backendVersion) : base(backendVersion) { }

        protected override void SetUp()
        {
            base.SetUp();
            // If this is the first (or only) test being run, the connection has already been opened
            // in the fixture setup. Save the extra connecting time.
            ExecuteNonQuery(@"  -- Table: posts
                                -- DROP TABLE posts;

                                CREATE TABLE posts
                                (
                                    id serial NOT NULL,
                                    title character varying,
                                    body character varying,
                                    user_name character varying,
                                    search_vector tsvector,
                                    CONSTRAINT posts_pkey PRIMARY KEY (id)
                                )
                                WITH (
                                    OIDS=FALSE
                                );

                                -- Index: posts_search_idx

                                -- DROP INDEX posts_search_idx;

                                CREATE INDEX posts_search_idx
                                ON posts USING gin (search_vector);

                                -- Trigger: posts_vector_update on posts

                                -- DROP TRIGGER posts_vector_update ON posts;

                                CREATE TRIGGER posts_vector_update
                                BEFORE INSERT OR UPDATE ON posts
                                FOR EACH ROW
                                    EXECUTE PROCEDURE tsvector_update_trigger('search_vector', 'pg_catalog.english', 'title', 'body');
                            ");

            ExecuteNonQuery("INSERT INTO posts (title, body, user_name) VALUES ('Postgres is awesome', '', 'Clark Kent')");
            ExecuteNonQuery("INSERT INTO posts (title, body, user_name) VALUES ('How postgres is differente from MySQL', '', 'Lois Lane')");
            ExecuteNonQuery("INSERT INTO posts (title, body, user_name) VALUES ('Tips for Mysql', '', 'Bruce Wayne')");
            ExecuteNonQuery("INSERT INTO posts (title, body, user_name) VALUES ('SECRET', 'Postgres for the win', 'Dick Grayson')");
            ExecuteNonQuery("INSERT INTO posts (title, body, user_name) VALUES ('Oracle acquires some other database', 'Mysql but no postgres' , 'Oliver Queen')");
            ExecuteNonQuery("INSERT INTO posts (title, body, user_name) VALUES ('No Database', 'Nothing to see here', 'Kyle Ryner')");
        }

        protected override void TearDown()
        {
            ExecuteNonQuery("DROP TABLE posts");
            base.TearDown();
        }

        [Test]
        public void FullTextSearchSimpleTest()
        {
            var conn = Npgsql.NpgsqlFactory.Instance.CreateConnection();
            conn.ConnectionString = ConnectionString;
            using (var ctx = new DbContext(conn, true))
            {
                var query = @"select * 
                              from posts 
                              where search_vector @@ to_tsquery('english', @p0) 
                              order by ts_rank_cd(search_vector, to_tsquery('english', @p0)) desc";
                var p = "postgres";
                var posts = ctx.Database.SqlQuery<Post>(query, p).ToList();

                Assert.AreEqual(4, posts.Count);
            }    
        }

        [Test]
        public void FullTextSearchAndTest()
        {
            var conn = Npgsql.NpgsqlFactory.Instance.CreateConnection();
            conn.ConnectionString = ConnectionString;
            using (var ctx = new DbContext(conn, true))
            {
                var query = @"select * 
                              from posts 
                              where search_vector @@ to_tsquery('english', @p0) 
                              order by ts_rank_cd(search_vector, to_tsquery('english', @p0)) desc";
                var p = "postgres & mysql";
                var posts = ctx.Database.SqlQuery<Post>(query, p).ToList();

                Assert.AreEqual(2, posts.Count);
            }
        }

        [Test]
        public void FullTextSearchOrTest()
        {
            var conn = Npgsql.NpgsqlFactory.Instance.CreateConnection();
            conn.ConnectionString = ConnectionString;
            using (var ctx = new DbContext(conn, true))
            {
                var query = @"select * 
                              from posts 
                              where search_vector @@ to_tsquery('english', @p0) 
                              order by ts_rank_cd(search_vector, to_tsquery('english', @p0)) desc";
                var p = "postgres | mysql";
                var posts = ctx.Database.SqlQuery<Post>(query, p).ToList();

                Assert.AreEqual(5, posts.Count);
            }
        }

        [Test]
        public void FirstOrDefaultTest()
        {
            var conn = Npgsql.NpgsqlFactory.Instance.CreateConnection();
            conn.ConnectionString = ConnectionString;
            using (var ctx = new TestDbContext(conn, true))
            {
                var post = ctx.Posts.FirstOrDefault();

                Assert.AreEqual("Clark Kent", post.UserName);
            }
        }
    }

    public class TestDbContext : DbContext
    {
        public TestDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
        }

        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Post>().ToTable("posts", "public");
            modelBuilder.Entity<Post>().Property(c => c.Id).HasColumnName("id").IsOptional();
            modelBuilder.Entity<Post>().Property(c => c.Title).HasColumnName("title").IsOptional();
            modelBuilder.Entity<Post>().Property(c => c.Body).HasColumnName("body").IsOptional();
            modelBuilder.Entity<Post>().Property(c => c.UserName).HasColumnName("user_name").IsOptional();
        }
    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string UserName { get; set; }
    }
}
