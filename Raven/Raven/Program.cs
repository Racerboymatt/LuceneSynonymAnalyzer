using System.Collections.Generic;
using System.Collections.ObjectModel;
using Raven.Client;
using Raven.Client.Document;
using System;
using System.Linq;
using Raven.Client.Indexes;

namespace Raven
{
    class Program
    {
        static void Main(string[] args)
        {
            IDocumentStore documentStore;
            using (documentStore = new DocumentStore()
            {
                Url = "http://localhost:8080",  
                DefaultDatabase = "TextSearch"
            }.Initialize())
            {
                Console.WriteLine("Raven Started");

                DeleteTestData(documentStore);
                CreateSampleData(documentStore);

                //CreateFullTextIndexTestComment(documentStore);
                CreateSynonmIndexTestCommentANicerWay(documentStore);

                //CreateFullTextIndex(documentStore);
                //CreateIndexANicerWay(documentStore);
                
                //ImportCommentsIntoRavenDb(documentStore);
                
                var allResults = documentStore.OpenSession().Query<TestComment>();

                Console.WriteLine("All Test Documents: " + allResults.Count());

                const string searchTerm = "fast";

                Console.WriteLine("Search on Term: " + searchTerm);

                var results =
                    documentStore.OpenSession()
                        .Query<TestComment, TestCommentsSynonymAnalyzerIndex>()
                        .Search(x => x.Comment, searchTerm)
                        .Customize(customization => customization.WaitForNonStaleResultsAsOfNow());

                Console.WriteLine(results.Count());
                foreach (var testComment in results)
                {
                    Console.WriteLine("Id: " + testComment.Id + " Comment: " + testComment.Comment);
                }
                Console.ReadLine();
            }
        }

        private static void DeleteTestData(IDocumentStore documentStore)
        {
            Console.WriteLine("Deleting Test Data");
            using (var session = documentStore.OpenSession())
            {
                var results = session.Query<TestComment>();
                Console.WriteLine("Test Data Count: " + results.Count());
                foreach (var testComment in results)
                {
                    session.Delete(testComment);
                }
                session.SaveChanges();
            }
            Console.WriteLine("Test Data Deleted");
        }

        static void CreateFullTextIndex(IDocumentStore documentStore)
        {
            Console.WriteLine("Create Raven Index CommentsFullTextIdx");
            documentStore.DatabaseCommands.PutIndex("CommentsFullTextIdx",
            new IndexDefinitionBuilder<Comment,Comment>
            {
                Map =
                    comments =>
                    from comment in comments  select new { comment.SdKey, comment.Survey, comment.SurveyComment},
                Analyzers =
                    {
                        {x => x.SdKey, "SimpleAnalyzer"},
                        {x => x.Survey, "SimpleAnalyzer"},
                        {x => x.SurveyComment, "StandardAnalyzer"}
                    },
            });
            Console.WriteLine("Raven Index CommentsFullTextIdx Created");
        }

        static void CreateIndexANicerWay(IDocumentStore documentStore)
        {
            Console.WriteLine("Create Raven Index of type: CommentsFullTextIndex");
            IndexCreation.CreateIndexes(typeof(CommentsFullTextIndex).Assembly, documentStore);
            Console.WriteLine("Raven Index of type: CommentsFullTextIndex Created");
        }

        static void CreateSampleData(IDocumentStore documentStore)
        {
            var comments = CreateTestComments();
            using (var session = documentStore.OpenSession())
            {
                foreach (var comment in comments)
                {
                    session.Store(comment);
                }
                session.SaveChanges();
            }
        }

        private static IEnumerable<TestComment> CreateTestComments()
        {
            var comments = new Collection<TestComment>
            {
                new TestComment(1, "fast pass the word"),
                new TestComment(2, "quick passing the word"),
                new TestComment(3, "rapid passed the word"),
                new TestComment(4, "pass the words"),
                new TestComment(5, "pass the wording"),
                new TestComment(6, "pass wtf"),
                new TestComment(7, "wtf word")
            };
            return comments;
        }

        static void CreateFullTextIndexTestComment(IDocumentStore documentStore)
        {
            Console.WriteLine("Create Raven Index CreateFullTextIndexTestComment");
            documentStore.DatabaseCommands.PutIndex("CreateFullTextIndexTestComment",
            new IndexDefinitionBuilder<TestComment, TestComment>
            {
                Map =
                    comments =>
                    from comment in comments select new { comment.Id, comment.Comment },
                Analyzers =
                    {
                        {x => x.Id, "SimpleAnalyzer"},
                        {x => x.Comment, "StandardAnalyzer"}
                    },
            });
            Console.WriteLine("Raven Index CreateFullTextIndexTestComment Created");
        }

        static void CreateIndexTestCommentANicerWay(IDocumentStore documentStore)
        {
            Console.WriteLine("Create Raven Index of type: TestCommentsEnglishSnowballAnalyzerIndex");
            IndexCreation.CreateIndexes(typeof(TestCommentsEnglishSnowballAnalyzerIndex).Assembly, documentStore);
            Console.WriteLine("Raven Index of type: TestCommentsEnglishSnowballAnalyzerIndex Created");
        }

        static void CreateSynonmIndexTestCommentANicerWay(IDocumentStore documentStore)
        {
            Console.WriteLine("Create Raven Index of type: TestCommentsSynonymAnalyzerIndex");
            IndexCreation.CreateIndexes(typeof(TestCommentsSynonymAnalyzerIndex).Assembly, documentStore);
            Console.WriteLine("Raven Index of type: TestCommentsSynonymAnalyzerIndex Created");
        }

        static void ImportCommentsIntoRavenDb(IDocumentStore documentStore) 
        {
            using (var session = documentStore.OpenSession())
            {
                int minKey = session.Query<Comment>()
                             .OrderBy(x => x.SdKey)
                             .Take(1)
                             .Select(x => x.SdKey)
                             .FirstOrDefault();
                if (minKey == 0) minKey = int.MaxValue;

                using (var context = new SID_DBEntities())
                {
                    var comments = (from cmts in context.sidCommentsViews
                                    join sd in context.sidSurveydatas on cmts.sdKey equals sd.sdKey
                                    join cv in context.sidCasesViews on sd.csKey equals cv.csKey
                                      where cmts.sdKey<minKey
                                      orderby sd.sdKey descending
                                      select new Comment
                                      {
                                          Id = cmts.frKey,
                                          SurveyItem = cmts.surveyItem,
                                          SurveyComment = cmts.comment,
                                          Survey = cv.suName,
                                          SdKey = cmts.sdKey,
                                          IbKey = cmts.ibkey,
                                          Date = cmts.sdDate
                                      }).Take(10000);

                    foreach (Comment cmt in comments)
                    {
                        cmt.SurveyComment = System.Web.HttpUtility.HtmlDecode(cmt.SurveyComment);
                        session.Store(cmt);
                    }
                    session.SaveChanges();
                    Console.WriteLine("Comments Saved");
                }
            }
        }
    }
}
