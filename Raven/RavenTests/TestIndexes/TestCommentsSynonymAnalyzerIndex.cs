using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.SynonymEngine;
using Raven.Client.Indexes;

namespace RavenTests.TestIndexes
{
    public class TestCommentsSynonymAnalyzerIndex : AbstractIndexCreationTask<TestComment, TestCommentsSynonymAnalyzerIndex.ReduceResult>
    {
        public class ReduceResult
        {
            public int Id { get; set; }
            public string Comment { get; set; }
        }

        public TestCommentsSynonymAnalyzerIndex()
        {
            Map = comments => from comment in comments select new {comment.Id, comment.Comment};

            Analyzers.Add(x => x.Id, typeof(SimpleAnalyzer).AssemblyQualifiedName);
            Analyzers.Add(x => x.Comment, typeof(SynonymAnalyzer).AssemblyQualifiedName);
        }
    }
}