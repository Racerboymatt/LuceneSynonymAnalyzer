using System;
using System.Windows.Forms;
//using Lucene.Net.SynonymEngine;

namespace AnalyzerViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            //var xmlEngine = new XmlSynonymEngine("syn.xml");

            //var list = xmlEngine.GetSynonyms("fast");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
