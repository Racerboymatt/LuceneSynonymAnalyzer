using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Raven.Client.Indexes;

namespace RavenTests.TestIndexes
{
    public class TestCommentsStandandAnalyzerIndex : AbstractIndexCreationTask<TestComment, TestCommentsStandandAnalyzerIndex.ReduceResult>
    {
        public class ReduceResult
        {
            public int Id { get; set; }
            public string Comment { get; set; }
        }

        public TestCommentsStandandAnalyzerIndex()
        {
            Map = comments => from comment in comments select new {comment.Id, comment.Comment};

            Analyzers.Add(x => x.Id, typeof(SimpleAnalyzer).AssemblyQualifiedName);
            Analyzers.Add(x => x.Comment, typeof(StandardAnalyzer).AssemblyQualifiedName);
        }
    }
}