#region License
// The PostgreSQL License
//
// Copyright (C) 2016 The Npgsql Development Team
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

using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using Npgsql.Tests;
using NpgsqlTypes;

namespace EntityFramework6.Npgsql.Tests
{
    class FullTextSearchTests : EntityFrameworkTestBase
    {
        [Test]
        public void ConversionToTsVector()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                context.Blogs.Add(new Blog { Name = "_" });
                context.SaveChanges();

                const string expected = "'a':5 'b':10";
                var casted = context.Blogs.Select(x => NpgsqlTextFunctions.AsTsVector(expected)).First();
                Assert.That(
                    NpgsqlTsVector.Parse(casted).ToString(),
                    Is.EqualTo(NpgsqlTsVector.Parse(expected).ToString()));

                var converted = context.Blogs.Select(x => NpgsqlTextFunctions.ToTsVector("banana car")).First();
                Assert.That(
                    NpgsqlTsVector.Parse(converted).ToString(),
                    Is.EqualTo(NpgsqlTsVector.Parse("'banana':1 'car':2").ToString()));

                converted = context.Blogs.Select(x => NpgsqlTextFunctions.ToTsVector("english", "banana car")).First();
                Assert.That(
                    NpgsqlTsVector.Parse(converted).ToString(),
                    Is.EqualTo(NpgsqlTsVector.Parse("'banana':1 'car':2").ToString()));
            }
        }

        [Test]
        public void ConversionToTsQuery()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                context.Blogs.Add(new Blog { Name = "_" });
                context.SaveChanges();

                const string expected = "'b' & 'c'";
                var casted = context.Blogs.Select(x => NpgsqlTextFunctions.AsTsQuery(expected)).First();
                Assert.That(
                    NpgsqlTsQuery.Parse(casted).ToString(),
                    Is.EqualTo(NpgsqlTsQuery.Parse(expected).ToString()));

                var converted = context.Blogs.Select(x => NpgsqlTextFunctions.ToTsQuery("b & c")).First();
                Assert.That(
                    NpgsqlTsQuery.Parse(converted).ToString(),
                    Is.EqualTo(NpgsqlTsQuery.Parse(expected).ToString()));

                converted = context.Blogs.Select(x => NpgsqlTextFunctions.ToTsQuery("english", "b & c")).First();
                Assert.That(
                    NpgsqlTsQuery.Parse(converted).ToString(),
                    Is.EqualTo(NpgsqlTsQuery.Parse(expected).ToString()));

                converted = context.Blogs.Select(x => NpgsqlTextFunctions.PlainToTsQuery("b & c")).First();
                Assert.That(
                    NpgsqlTsQuery.Parse(converted).ToString(),
                    Is.EqualTo(NpgsqlTsQuery.Parse(expected).ToString()));

                converted = context.Blogs.Select(x => NpgsqlTextFunctions.PlainToTsQuery("english", "b & c")).First();
                Assert.That(
                    NpgsqlTsQuery.Parse(converted).ToString(),
                    Is.EqualTo(NpgsqlTsQuery.Parse(expected).ToString()));
            }
        }

        [Test]
        public void TsVectorConcat()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                context.Blogs.Add(new Blog { Name = "_" });
                context.SaveChanges();

                var result = context.Blogs.Select(
                    x => NpgsqlTextFunctions.AsTsVector("a:1 b:2")
                         + NpgsqlTextFunctions.AsTsVector("c:1 d:2 b:3")).First();

                Assert.That(
                    NpgsqlTsVector.Parse(result).ToString(),
                    Is.EqualTo(NpgsqlTsVector.Parse("'a':1 'b':2,5 'c':3 'd':4").ToString()));
            }
        }

        [Test]
        public void TsQueryAnd()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                context.Blogs.Add(new Blog { Name = "_" });
                context.SaveChanges();

                var result = context.Blogs.Select(
                    x => NpgsqlTextFunctions.QueryAnd(
                        NpgsqlTextFunctions.AsTsQuery("fat | rat"),
                        NpgsqlTextFunctions.AsTsQuery("cat"))).First();

                Assert.That(
                    NpgsqlTsQuery.Parse(result).ToString(),
                    Is.EqualTo(NpgsqlTsQuery.Parse("( 'fat' | 'rat' ) & 'cat'").ToString()));
            }
        }

        [Test]
        public void TsQueryOr()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                context.Blogs.Add(new Blog { Name = "_" });
                context.SaveChanges();

                var result = context.Blogs.Select(
                    x => NpgsqlTextFunctions.QueryOr(
                        NpgsqlTextFunctions.AsTsQuery("fat | rat"),
                        NpgsqlTextFunctions.AsTsQuery("cat"))).First();

                Assert.That(
                    NpgsqlTsQuery.Parse(result).ToString(),
                    Is.EqualTo(NpgsqlTsQuery.Parse("( 'fat' | 'rat' ) | 'cat'").ToString()));

                result = context.Blogs.Select(
                    x => NpgsqlTextFunctions.AsTsQuery("fat | rat")
                         + NpgsqlTextFunctions.AsTsQuery("cat")).First();

                Assert.That(
                    NpgsqlTsQuery.Parse(result).ToString(),
                    Is.EqualTo(NpgsqlTsQuery.Parse("( 'fat' | 'rat' ) | 'cat'").ToString()));
            }
        }

        [Test]
        public void TsQueryNot()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                context.Blogs.Add(new Blog { Name = "_" });
                context.SaveChanges();

                var result = context.Blogs.Select(
                    x => NpgsqlTextFunctions.QueryNot(NpgsqlTextFunctions.AsTsQuery("cat"))).First();

                Assert.That(
                    NpgsqlTsQuery.Parse(result).ToString(),
                    Is.EqualTo(NpgsqlTsQuery.Parse("! 'cat'").ToString()));
            }
        }

        [Test]
        public void TsContains()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                context.Blogs.Add(new Blog { Name = "_" });
                context.SaveChanges();

                var result = context.Blogs.Select(
                    x =>
                        NpgsqlTextFunctions.QueryContains(
                            NpgsqlTextFunctions.AsTsQuery("cat"),
                            NpgsqlTextFunctions.AsTsQuery("cat & rat"))).First();

                Assert.That(result, Is.False);
            }
        }

        [Test]
        public void TsIsContained()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                context.Blogs.Add(new Blog { Name = "_" });
                context.SaveChanges();

                var result = context.Blogs.Select(
                    x =>
                        NpgsqlTextFunctions.QueryIsContained(
                            NpgsqlTextFunctions.AsTsQuery("cat"),
                            NpgsqlTextFunctions.AsTsQuery("cat & rat"))).First();

                Assert.That(result, Is.True);
            }
        }

        [Test]
        public void Match()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                var blog1 = new Blog
                {
                    Name = "The quick brown fox jumps over the lazy dog."
                };
                var blog2 = new Blog
                {
                    Name = "Jackdaws loves my big sphinx of quartz."
                };
                context.Blogs.Add(blog1);
                context.Blogs.Add(blog2);
                context.SaveChanges();

                var foundBlog = context
                    .Blogs
                    .FirstOrDefault(
                        x =>
                            NpgsqlTextFunctions.Match(
                                NpgsqlTextFunctions.ToTsVector(x.Name),
                                NpgsqlTextFunctions.ToTsQuery("jump & dog")));

                Assert.That(foundBlog != null);
                Assert.That(foundBlog.Name, Is.EqualTo(blog1.Name));
            }
        }

        [Test]
        public void SetWeight()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                var blog1 = new Blog
                {
                    Name = "The quick brown fox jumps over the lazy dog."
                };
                context.Blogs.Add(blog1);

                var post1 = new Post
                {
                    Blog = blog1,
                    Title = "Lorem ipsum",
                    Content = "Dolor sit amet",
                    Rating = 5
                };
                context.Posts.Add(post1);

                var post2 = new Post
                {
                    Blog = blog1,
                    Title = "consectetur adipiscing elit",
                    Content = "Sed sed rhoncus",
                    Rating = 4
                };
                context.Posts.Add(post2);
                context.SaveChanges();

                var foundPost = context.Posts.FirstOrDefault(
                    x => NpgsqlTextFunctions.Match(
                        NpgsqlTextFunctions.SetWeight(
                            NpgsqlTextFunctions.ToTsVector(x.Title ?? string.Empty),
                            NpgsqlWeightLabel.D)
                        + NpgsqlTextFunctions.SetWeight(
                            NpgsqlTextFunctions.ToTsVector(x.Content ?? string.Empty),
                            NpgsqlWeightLabel.C),
                        NpgsqlTextFunctions.PlainToTsQuery("dolor")));

                Assert.That(foundPost != null);
                Assert.That(foundPost.Title, Is.EqualTo(post1.Title));
            }
        }

        [Test]
        public void Length()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                var blog = new Blog
                {
                    Name = "cooky cookie cookies piano pianos"
                };
                context.Blogs.Add(blog);
                context.SaveChanges();

                var lexemeCount = context
                    .Blogs
                    .Select(x => NpgsqlTextFunctions.Length(NpgsqlTextFunctions.ToTsVector(x.Name)))
                    .FirstOrDefault();

                Assert.That(lexemeCount, Is.EqualTo(2));
            }
        }

        [Test]
        public void NumNode()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                var blog = new Blog
                {
                    Name = "_"
                };
                context.Blogs.Add(blog);
                context.SaveChanges();

                var nodeCount = context
                    .Blogs
                    .Select(x => NpgsqlTextFunctions.NumNode(NpgsqlTextFunctions.ToTsQuery("(fat & rat) | cat")))
                    .FirstOrDefault();

                Assert.That(nodeCount, Is.EqualTo(5));
            }
        }

        [Test]
        public void Strip()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                var blog = new Blog
                {
                    Name = "cooky cookie cookies piano pianos"
                };
                context.Blogs.Add(blog);
                context.SaveChanges();

                var strippedTsVector = context
                    .Blogs
                    .Select(x => NpgsqlTextFunctions.Strip(NpgsqlTextFunctions.ToTsVector(x.Name)))
                    .FirstOrDefault();

                Assert.That(
                    NpgsqlTsVector.Parse(strippedTsVector).ToString(),
                    Is.EqualTo(NpgsqlTsVector.Parse("'cooki' 'piano'").ToString()));
            }
        }

        [Test]
        public void QueryTree()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                var blog = new Blog
                {
                    Name = "_"
                };
                context.Blogs.Add(blog);
                context.SaveChanges();

                var queryTree = context
                    .Blogs
                    .Select(x => NpgsqlTextFunctions.QueryTree(NpgsqlTextFunctions.ToTsQuery("foo & ! bar")))
                    .FirstOrDefault();

                Assert.That(
                    NpgsqlTsQuery.Parse(queryTree).ToString(),
                    Is.EqualTo(NpgsqlTsQuery.Parse("'foo'").ToString()));
            }
        }

        [Test]
        public void TsHeadline()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                var blog1 = new Blog
                {
                    Name = "cooky cookie piano pianos"
                };
                context.Blogs.Add(blog1);

                var blog2 = new Blog
                {
                    Name = "blue crab denominates elephant"
                };
                context.Blogs.Add(blog2);
                context.SaveChanges();

                var headlines = context
                    .Blogs
                    .Select(
                        x => NpgsqlTextFunctions.TsHeadline(
                            x.Name,
                            NpgsqlTextFunctions.ToTsQuery("cookie"),
                            "StartSel=<i> StopSel=</i>"))
                    .ToList();

                Assert.That(headlines.Count, Is.EqualTo(2));
                Assert.That(headlines[0], Is.EqualTo("<i>cooky</i> <i>cookie</i> piano pianos"));
                Assert.That(headlines[1], Is.EqualTo(blog2.Name));

                headlines = context
                    .Blogs
                    .Select(
                        x => NpgsqlTextFunctions.TsHeadline(
                            "english",
                            x.Name,
                            NpgsqlTextFunctions.ToTsQuery("piano"),
                            "StartSel=<i> StopSel=</i>"))
                    .ToList();

                Assert.That(headlines.Count, Is.EqualTo(2));
                Assert.That(headlines[0], Is.EqualTo("cooky cookie <i>piano</i> <i>pianos</i>"));
                Assert.That(headlines[1], Is.EqualTo(blog2.Name));
            }
        }

        [Test]
        public void TsRank()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                var blog1 = new Blog
                {
                    Name = "cooky cookie piano pianos"
                };
                context.Blogs.Add(blog1);
                context.SaveChanges();

                var rank = context
                    .Blogs
                    .Select(
                        x => NpgsqlTextFunctions.TsRank(
                            NpgsqlTextFunctions.ToTsVector(x.Name),
                            NpgsqlTextFunctions.PlainToTsQuery("cookie")))
                    .FirstOrDefault();
                Assert.That(rank, Is.GreaterThan(0));

                rank = context
                    .Blogs
                    .Select(
                        x => NpgsqlTextFunctions.TsRank(
                            NpgsqlTextFunctions.ToTsVector(x.Name),
                            NpgsqlTextFunctions.PlainToTsQuery("cookie"),
                            NpgsqlRankingNormalization.DivideByLength
                            | NpgsqlRankingNormalization.DivideByUniqueWordCount))
                    .FirstOrDefault();
                Assert.That(rank, Is.GreaterThan(0));

                rank = context
                    .Blogs
                    .Select(
                        x => NpgsqlTextFunctions.TsRank(
                            0.1f, 0.2f, 0.4f, 1.0f,
                            NpgsqlTextFunctions.ToTsVector(x.Name),
                            NpgsqlTextFunctions.PlainToTsQuery("cookie")))
                    .FirstOrDefault();
                Assert.That(rank, Is.GreaterThan(0));

                rank = context
                    .Blogs
                    .Select(
                        x => NpgsqlTextFunctions.TsRank(
                            0.1f, 0.2f, 0.4f, 1.0f,
                            NpgsqlTextFunctions.ToTsVector(x.Name),
                            NpgsqlTextFunctions.PlainToTsQuery("cookie"),
                            NpgsqlRankingNormalization.DivideByLength
                            | NpgsqlRankingNormalization.DivideByUniqueWordCount))
                    .FirstOrDefault();
                Assert.That(rank, Is.GreaterThan(0));
            }
        }

        [Test]
        public void TsRankCd()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                var blog1 = new Blog
                {
                    Name = "cooky cookie piano pianos"
                };
                context.Blogs.Add(blog1);
                context.SaveChanges();

                var rank = context
                    .Blogs
                    .Select(
                        x => NpgsqlTextFunctions.TsRankCd(
                            NpgsqlTextFunctions.ToTsVector(x.Name),
                            NpgsqlTextFunctions.PlainToTsQuery("cookie")))
                    .FirstOrDefault();
                Assert.That(rank, Is.GreaterThan(0));

                rank = context
                    .Blogs
                    .Select(
                        x => NpgsqlTextFunctions.TsRankCd(
                            NpgsqlTextFunctions.ToTsVector(x.Name),
                            NpgsqlTextFunctions.PlainToTsQuery("cookie"),
                            NpgsqlRankingNormalization.DivideByLength
                            | NpgsqlRankingNormalization.DivideByUniqueWordCount))
                    .FirstOrDefault();
                Assert.That(rank, Is.GreaterThan(0));

                rank = context
                    .Blogs
                    .Select(
                        x => NpgsqlTextFunctions.TsRankCd(
                            0.1f, 0.2f, 0.4f, 1.0f,
                            NpgsqlTextFunctions.ToTsVector(x.Name),
                            NpgsqlTextFunctions.PlainToTsQuery("cookie")))
                    .FirstOrDefault();
                Assert.That(rank, Is.GreaterThan(0));

                rank = context
                    .Blogs
                    .Select(
                        x => NpgsqlTextFunctions.TsRankCd(
                            0.1f, 0.2f, 0.4f, 1.0f,
                            NpgsqlTextFunctions.ToTsVector(x.Name),
                            NpgsqlTextFunctions.PlainToTsQuery("cookie"),
                            NpgsqlRankingNormalization.DivideByLength
                            | NpgsqlRankingNormalization.DivideByUniqueWordCount))
                    .FirstOrDefault();
                Assert.That(rank, Is.GreaterThan(0));
            }
        }

        [Test]
        public void TsRewrite()
        {
            using (var context = new BloggingContext(ConnectionStringEF))
            {
                context.Database.Log = Console.Out.WriteLine;

                var blog1 = new Blog
                {
                    Name = "_"
                };
                context.Blogs.Add(blog1);
                context.SaveChanges();

                var newQuery = context
                    .Blogs.Select(
                        x =>
                            NpgsqlTextFunctions.TsRewrite(
                                "a & b",
                                "a",
                                "foo|bar"))
                    .FirstOrDefault();

                Assert.That(
                    NpgsqlTsQuery.Parse(newQuery).ToString(),
                    Is.EqualTo(NpgsqlTsQuery.Parse("'b' & ( 'foo' | 'bar' )").ToString()));
            }
        }
    }
}
