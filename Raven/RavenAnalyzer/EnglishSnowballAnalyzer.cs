using Lucene.Net.Analysis.Snowball;
using Lucene.Net.Util;

namespace RavenAnalyzer
{
    public class EnglishSnowballAnalyzer : SnowballAnalyzer
    {
        public EnglishSnowballAnalyzer() : base(Version.LUCENE_30, "English")
        {
        }
    }
}
