using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Raven.Client;
using Raven.Tests.Helpers;
using TestCommentsSynonymAnalyzerIndex = RavenTests.TestIndexes.TestCommentsSynonymAnalyzerIndex;

namespace RavenTests
{
    [TestFixture]
    public class RavenSynonymAnalyzerTests : RavenTestBase
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
        public void ShouldReturnCorrectSynonymResultsForFast()
        {
            using (var store = NewDocumentStore())
            {
                new TestCommentsSynonymAnalyzerIndex().Execute(store);

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
                    var results = session.Query<TestComment, TestCommentsSynonymAnalyzerIndex>()
                        .Search(x => x.Comment, "fast")
                        .Customize(customization => customization.WaitForNonStaleResults());

                    results.Count().Should().Be(3);
                    results.Any(x => x.Id == 1).Should().BeTrue();
                    results.Any(x => x.Id == 2).Should().BeTrue();
                    results.Any(x => x.Id == 3).Should().BeTrue();
                }
            }
        }

        private static IEnumerable<TestComment> CreateTestComments()
        {
            var comments = new Collection<TestComment>
            {
                new TestComment(1, "pass the word quick"),
                new TestComment(2, "passing the word fast"),
                new TestComment(3, "passed the word rapid"),
                new TestComment(4, "pass the words"),
                new TestComment(5, "pass the wording"),
                new TestComment(6, "pass wtf"),
                new TestComment(7, "wtf word")
            };
            return comments;
        }
    }
}