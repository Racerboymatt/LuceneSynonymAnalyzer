using System.Collections.Generic;

namespace Lucene.Net.SynonymEngine
{
    public interface ISynonymEngine
    {
        IEnumerable<string> GetSynonyms(string word);
    }
}
