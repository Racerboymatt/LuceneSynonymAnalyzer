using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace AnalyzerViewer
{
    public abstract class AnalyzerView
    {
        public abstract string Name { get; }

        public virtual string GetView(TokenStream tokenStream, out int numberOfTokens)
        {
            var sb = new StringBuilder();

            numberOfTokens = 0;

            while (tokenStream.IncrementToken())
            {
                numberOfTokens++;
                sb.Append(GetTokenView(tokenStream));
            }

            return sb.ToString();
        }

        protected abstract string GetTokenView(TokenStream tokenStream);
    }

    public class TermAnalyzerView : AnalyzerView
    {
        public override string Name
        {
            get { return "Terms"; }
        }

        protected override string GetTokenView(TokenStream tokenStream)
        {
            var attTerm = tokenStream.GetAttribute<ITermAttribute>();
            return "[" + attTerm.Term + "]   ";
        }
    }

    public class TermWithOffsetsView : AnalyzerView
    {
        public override string Name
        {
            get { return "Terms With Offsets"; }
        }

        protected override string GetTokenView(TokenStream tokenStream)
        {
            var attTerm = tokenStream.GetAttribute<ITermAttribute>();
            var attOffSet = tokenStream.GetAttribute<IOffsetAttribute>();
            return attTerm.Term + "   Start: " + attOffSet.StartOffset.ToString(CultureInfo.InvariantCulture).PadLeft(5) + "  End: " + attOffSet.EndOffset.ToString(CultureInfo.InvariantCulture).PadLeft(5) + "\r\n";
        }
    }

    public class TermFrequencies : AnalyzerView
    {
        public override string Name
        {
            get { return "Term Frequencies"; }
        }

        private readonly Dictionary<string, int> _termDictionary = new Dictionary<string, int>();

        public override string GetView(TokenStream tokenStream, out int numberOfTokens)
        {
            var sb = new StringBuilder();

            numberOfTokens = 0;

            while (tokenStream.IncrementToken())
            {
                numberOfTokens++;
                var attTerm = tokenStream.GetAttribute<ITermAttribute>();


                if (_termDictionary.Keys.Contains(attTerm.Term))
                    _termDictionary[attTerm.Term] = _termDictionary[attTerm.Term] + 1;
                else
                    _termDictionary.Add(attTerm.Term, 1);
            }

            foreach (var item in _termDictionary.OrderBy(x => x.Key))
            {
                sb.Append(item.Key + " [" + item.Value + "]   ");
            }

            _termDictionary.Clear();

            return sb.ToString();
        }

        protected override string GetTokenView(TokenStream tokenStream)
        {
            throw new System.NotImplementedException();
        }
    }
}
