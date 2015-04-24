using Lucene.Net.Analysis;

namespace AnalyzerViewer
{
    /// <summary>
    /// Provides the analyzer and some information about a lucene analyzer.
    /// </summary>
    public class AnalyzerInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzerInfo"/> class.
        /// </summary>
        /// <param name="name">the name of the analyzer.</param>
        /// <param name="description"></param>
        /// <param name="analyzer">The Lucene.Net Analyzer to use.</param>
        public AnalyzerInfo(string name, string description, Analyzer analyzer)
        {
            Name = name;
            Description = description;
            LuceneAnalyzer = analyzer;
        }

        /// <summary>
        /// Gets or sets the name of the analyzer.
        /// </summary>
        public string Name { get; protected set; }

        public string Description { get; protected set; }
        /// <summary>
        /// Gets or sets the Lucene.Net Analyzer object.
        /// </summary>
        public Analyzer LuceneAnalyzer { get; protected set; }
    }
}
