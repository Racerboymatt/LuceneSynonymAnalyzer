using System.Linq;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Raven.Client.Indexes;

namespace Raven
{
    public class CommentsFullTextIndex : AbstractIndexCreationTask<Comment, CommentsFullTextIndex.ReduceResult>
    {
        public class ReduceResult
        {
            //public int Id { get; set; }
            //public DateTime Date { get; set; }
            public int SdKey { get; set; }
            //public int IbKey { get; set; }
            public string Survey { get; set; }
            //public string SurveyItem { get; set; }
            public string SurveyComment { get; set; }
        }

        public CommentsFullTextIndex()
        {
            Map = comments => from comment in comments select new {comment.SdKey, comment.Survey, comment.SurveyComment};

            Analyzers.Add(x => x.SdKey, typeof(SimpleAnalyzer).AssemblyQualifiedName);
            Analyzers.Add(x => x.Survey, typeof(SimpleAnalyzer).AssemblyQualifiedName);
            Analyzers.Add(x => x.SurveyComment, typeof(StandardAnalyzer).AssemblyQualifiedName);    
        }
    }
}
