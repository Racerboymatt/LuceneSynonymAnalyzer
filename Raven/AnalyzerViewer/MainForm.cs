using System;
using System.ComponentModel;
using System.Windows.Forms;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using System.IO;
using Lucene.Net.Analysis.Snowball;
using Version = Lucene.Net.Util.Version;

namespace AnalyzerViewer
{
    public partial class MainForm : Form
    {
        public BindingList<AnalyzerInfo> AnalyzerList = new BindingList<AnalyzerInfo>();
        public BindingList<AnalyzerView> AnalyzerViews = new BindingList<AnalyzerView>();

        public MainForm()
        {
            InitializeComponent();

            AnalyzerList.Add(new AnalyzerInfo("Keyword Analyzer", "\"Tokenizes\" the entire stream as a single token.",  new KeywordAnalyzer()));
            AnalyzerList.Add(new AnalyzerInfo("Whitespace Analyzer", "An Analyzer that uses WhitespaceTokenizer.",  new WhitespaceAnalyzer()));
            AnalyzerList.Add(new AnalyzerInfo("Stop Analyzer", "Filters LetterTokenizer with LowerCaseFilter and StopFilter.",  new StopAnalyzer(Version.LUCENE_30)));
            AnalyzerList.Add(new AnalyzerInfo("Simple Analyzer", "An Analyzer that filters LetterTokenizer with LowerCaseFilter.",  new SimpleAnalyzer()));
            AnalyzerList.Add(new AnalyzerInfo("Standard Analyzer", "Filters StandardTokenizer with StandardFilter, LowerCaseFilter and StopFilter, using a list of English stop words.",  new StandardAnalyzer(Version.LUCENE_30)));
            AnalyzerList.Add(new AnalyzerInfo("Synonym Analyzer", "A Custom Analyzer That Injects Synonyms into the analysis.", new Lucene.Net.SynonymEngine.SynonymAnalyzer()));
            AnalyzerList.Add(new AnalyzerInfo("SnowBallAnalyzer", "Snow Ball", new SnowballAnalyzer(Version.LUCENE_30, "English")));

            AnalyzerViews.Add(new TermAnalyzerView());
            AnalyzerViews.Add(new TermWithOffsetsView());
            AnalyzerViews.Add(new TermFrequencies());
            
            tbDescription.DataBindings.Add(new Binding("Text", AnalyzerList, "Description"));

            cbAnalysers.DisplayMember = "Name";
            cbAnalysers.ValueMember = "LuceneAnalyzer";
            cbAnalysers.DataSource = AnalyzerList;

            cbViews.DisplayMember = "Name";
            cbViews.DataSource = AnalyzerViews;

            cbAnalysers.SelectedIndex = 0;
            cbViews.SelectedIndex = 0;

            cbAnalysers.SelectedValueChanged += cbAnalysers_SelectedValueChanged;
            cbViews.SelectedValueChanged += cbViews_SelectedValueChanged;
            tbSourceText.TextChanged += tbSourceText_TextChanged;

            tbSourceText.Text = @"The quick brown fox jumped over the lazy dog.";
            //tbSourceText.Text = "fast pass the word";
            AnalyzeText();
        }

        void cbViews_SelectedValueChanged(object sender, EventArgs e)
        {
            AnalyzeText();            
        }

        void tbSourceText_TextChanged(object sender, EventArgs e)
        {
            AnalyzeText();
        }

        void cbAnalysers_SelectedValueChanged(object sender, EventArgs e)
        {
            AnalyzeText();
        }

        public void AnalyzeText()
        {
            var analyzer = cbAnalysers.SelectedValue as Analyzer;

            int termCounter = 0;

            if (analyzer != null)
            {
                //var sb = new StringBuilder();

                var view = cbViews.SelectedValue as AnalyzerView;

                var stringReader = new StringReader(tbSourceText.Text);

                TokenStream tokenStream = analyzer.TokenStream("defaultFieldName", stringReader);

                if (view != null)
                {
                    tbOutputText.Text = view.GetView(tokenStream, out termCounter).Trim();
                }
            }

            lblStats.Text = string.Format("Total of {0} Term(s) Found.", termCounter);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
