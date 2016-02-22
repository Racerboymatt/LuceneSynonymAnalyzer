using System.Linq;
using Lucene.Net.Analysis;
using Raven;
using Raven.Client.Indexes;
using RavenAnalyzer;

namespace RavenTests.TestIndexes
{
    public class TestCommentsEnglishSnowballAnalyzerIndex : AbstractIndexCreationTask<TestComment, TestCommentsEnglishSnowballAnalyzerIndex.ReduceResult>
    {
        public class ReduceResult
        {
            public int Id { get; set; }
            public string Comment { get; set; }
        }

        public TestCommentsEnglishSnowballAnalyzerIndex()
        {
            Map = comments => from comment in comments select new {comment.Id, comment.Comment};

            Analyzers.Add(x => x.Id, typeof(SimpleAnalyzer).AssemblyQualifiedName);
            Analyzers.Add(x => x.Comment, typeof(EnglishSnowballAnalyzer).AssemblyQualifiedName);
        }
    }
}