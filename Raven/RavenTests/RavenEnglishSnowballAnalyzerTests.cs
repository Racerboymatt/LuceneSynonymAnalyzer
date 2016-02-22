using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Raven;
using Raven.Client;
using Raven.Tests.Helpers;
using TestCommentsEnglishSnowballAnalyzerIndex = RavenTests.TestIndexes.TestCommentsEnglishSnowballAnalyzerIndex;

namespace RavenTests
{
    [TestFixture]
    public class RavenEnglishSnowballAnalyzerTests : RavenTestBase
    {
        [Test]
        public void InsertAndRetrieveDocumentsFromRaven()
        {
            using (var store = NewDocumentStore())
            {
                var comments = CreateTestComments();

                using (var session = store.OpenSession())
                {
                    foreach (var comment in comments)
                    {
                        session.Store(comment);
                    }
                    session.SaveChanges();
                
                    var results = session.Query<TestComment>().Customize(customization => customization.WaitForNonStaleResultsAsOfNow());

                    results.Count().Should().Be(7);
                }
            }
        }

        [Test]
        public void ShouldReturnCorrectStemmingResultsForPass()
        {
            using (var store = NewDocumentStore())
            {
                new TestCommentsEnglishSnowballAnalyzerIndex().Execute(store);

                var comments = CreateTestComments();

                using (var session = store.OpenSession())
                {
                    foreach (var comment in comments)
                    {
                        session.Store(comment);
                    }
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var results = session.Query<TestComment, TestCommentsEnglishSnowballAnalyzerIndex>()
                        .Search(x => x.Comment, "pass")
                        .Customize(customization => customization.WaitForNonStaleResults(TimeSpan.FromSeconds(10)));

                    results.Count().Should().Be(6);
                    results.Any(x => x.Id == 1).Should().BeTrue();
                    results.Any(x => x.Id == 2).Should().BeTrue();
                    results.Any(x => x.Id == 3).Should().BeTrue();
                    results.Any(x => x.Id == 4).Should().BeTrue();
                    results.Any(x => x.Id == 5).Should().BeTrue();
                    results.Any(x => x.Id == 6).Should().BeTrue();
                }
            }
        }

        [Test]
        public void ShouldReturnCorrectStemmingResultsForWord()
        {
            using (var store = NewDocumentStore())
            {
                new TestCommentsEnglishSnowballAnalyzerIndex().Execute(store);

                var comments = CreateTestComments();

                using (var session = store.OpenSession())
                {
                    foreach (var comment in comments)
                    {
                        session.Store(comment);
                    }
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var results = session.Query<TestComment, TestCommentsEnglishSnowballAnalyzerIndex>()
                        .Search(x => x.Comment, "word")
                        .Customize(customization => customization.WaitForNonStaleResultsAsOfNow()).ToList();

                    results.Count().Should().Be(6);
                    results.Any(x => x.Id == 1).Should().BeTrue();
                    results.Any(x => x.Id == 2).Should().BeTrue();
                    results.Any(x => x.Id == 3).Should().BeTrue();
                    results.Any(x => x.Id == 4).Should().BeTrue();
                    results.Any(x => x.Id == 5).Should().BeTrue();
                    results.Any(x => x.Id == 7).Should().BeTrue();
                }
            }
        }

        [Test]
        public void ShouldReturnCorrectStemmingResultsForPassSpaceWord()
        {
            using (var store = NewDocumentStore())
            {
                new TestCommentsEnglishSnowballAnalyzerIndex().Execute(store);

                var comments = CreateTestComments();

                using (var session = store.OpenSession())
                {
                    foreach (var comment in comments)
                    {
                        session.Store(comment);
                    }
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var results = session.Query<TestComment, TestCommentsEnglishSnowballAnalyzerIndex>()
                        .Search(x => x.Comment, "pass word")
                        .Customize(customization => customization.WaitForNonStaleResultsAsOfNow()).ToList();

                    results.Count().Should().Be(7);
                    results.Any(x => x.Id == 1).Should().BeTrue();
                    results.Any(x => x.Id == 2).Should().BeTrue();
                    results.Any(x => x.Id == 3).Should().BeTrue();
                    results.Any(x => x.Id == 4).Should().BeTrue();
                    results.Any(x => x.Id == 5).Should().BeTrue();
                    results.Any(x => x.Id == 6).Should().BeTrue();
                    results.Any(x => x.Id == 7).Should().BeTrue();
                }
            }
        }

        [Test]
        public void ShouldReturnCorrectResultsForWordUsingIQuerableWithAppendingCriteria()
        {
            using (var store = NewDocumentStore())
            {
                new TestCommentsEnglishSnowballAnalyzerIndex().Execute(store);

                var comments = CreateTestComments();

                using (var session = store.OpenSession())
                {
                    foreach (var comment in comments)
                    {
                        session.Store(comment);
                    }
                    session.SaveChanges();
                }

                using (var session = store.OpenSession())
                {
                    var query = session.Query<TestComment, TestCommentsEnglishSnowballAnalyzerIndex>().Search(x => x.Comment, "word")
                        .Customize(customization => customization.WaitForNonStaleResultsAsOfNow());

                    var results = query.Where(x => x.Id == 1).ToList();

                    results.Count().Should().Be(1);
                    results.Any(x => x.Id == 1).Should().BeTrue();
                }
            }
        }

        private static IEnumerable<TestComment> CreateTestComments()
        {
            var comments = new Collection<TestComment>
            {
                new TestComment(1, "pass the word"),
                new TestComment(2, "passing the word"),
                new TestComment(3, "passed the word"),
                new TestComment(4, "pass the words"),
                new TestComment(5, "pass the wording"),
                new TestComment(6, "pass wtf"),
                new TestComment(7, "wtf word")
            };
            return comments;
        }
    }
}