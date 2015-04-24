using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace Lucene.Net.SynonymEngine
{
    public class SynonymFilter : TokenFilter
    {
        private readonly Queue<Token> _synonymTokenQueue = new Queue<Token>();
        private readonly ITermAttribute _attTerm;
        private readonly IOffsetAttribute _attOffSet;
        private State _savedState;

        public ISynonymEngine SynonymEngine { get; private set; }

        public SynonymFilter(TokenStream input, ISynonymEngine synonymEngine) : base(input)
        {
            if (synonymEngine == null)
                throw new ArgumentNullException("synonymEngine");

            SynonymEngine = synonymEngine;

            _attTerm = input.GetAttribute<ITermAttribute>();
            _attOffSet = input.GetAttribute<IOffsetAttribute>();
        }

        public override bool IncrementToken()
        {
            // if our synonymTokens queue contains any tokens, return the next one.
            if (_synonymTokenQueue.Count > 0)
            {
                input.RestoreState(_savedState);
                var token = _synonymTokenQueue.Dequeue();
                _attTerm.SetTermBuffer(token.TermBuffer(), 0, token.TermLength());
                return true;
            }

            if (input.IncrementToken())
            {
                //retrieve the synonyms
                IEnumerable<string> synonyms = SynonymEngine.GetSynonyms(_attTerm.Term);

                //if we don't have any synonyms just return true
                if (synonyms == null)
                {
                    return true;
                }

                _savedState = input.CaptureState();

                foreach (string syn in synonyms)
                {
                    //make sure we don't add the same word 
                    if (!_attTerm.Term.Equals(syn))
                    {
                        //create the synonymToken
                        var synToken = new Token(syn, _attOffSet.StartOffset, _attOffSet.EndOffset, "<SYNONYM>");

                        // set the position increment to zero
                        // this tells lucene the the synonym is 
                        // in the exact same location as the originating word
                        synToken.PositionIncrement = 0;

                        //add the synToken to the synonyms queue
                        _synonymTokenQueue.Enqueue(synToken);
                    }
                }
                return true;
            }

            return false;
        }
    }
}
