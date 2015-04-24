﻿using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;

namespace Lucene.Net.SynonymEngine
{
    public class SynonymAnalyzer : Analyzer
    {
        public ISynonymEngine SynonymEngine { get; private set; }

        public SynonymAnalyzer()
        {
            SynonymEngine = new XmlSynonymEngine("syn.xml");
        }

        public override TokenStream TokenStream(string fieldName, System.IO.TextReader reader)
        {
            //create the tokenizer
            TokenStream result = new StandardTokenizer(Version.LUCENE_30, reader);

            //add in filters
            result = new StandardFilter(result); // first normalize the StandardTokenizer
            result = new LowerCaseFilter(result);// makes sure everything is lower case
            result = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(Version.LUCENE_30), result, StopAnalyzer.ENGLISH_STOP_WORDS_SET); // use the default list of Stop Words, provided by the StopAnalyzer class.
            result = new SynonymFilter(result, SynonymEngine); // injects the synonyms. 

            //return the built token stream.
            return result;
        }
    }
}
