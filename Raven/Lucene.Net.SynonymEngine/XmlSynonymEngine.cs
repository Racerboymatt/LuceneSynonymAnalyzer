using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.ObjectModel;

namespace Lucene.Net.SynonymEngine
{
    public class XmlSynonymEngine : ISynonymEngine
    {
        //this will contains a list, of lists of words that go together
        private List<ReadOnlyCollection<string>> SynonymGroups =
            new List<ReadOnlyCollection<string>>();

        public XmlSynonymEngine(string xmlSynonymFilePath)
        {
            // create an xml document object, and load it from the specified file.
            XmlDocument Doc = new XmlDocument();
            Doc.Load(xmlSynonymFilePath);

            // get all the <group> nodes
            var groupNodes = Doc.SelectNodes("/synonyms/group");

            //enumerate groups
            foreach (XmlNode g in groupNodes)
            {
                //get all the <syn> elements from the group nodes.
                XmlNodeList synNodes = g.SelectNodes("child::syn");

                //create a list that will hold the items for this group
                List<string> synonymGroupList = new List<string>();

                //enumerate then and add them to the list,
                //and add each synonym group to the list
                foreach (XmlNode synNode in g)
                {
                    synonymGroupList.Add(synNode.InnerText.Trim());
                }

                //add single synonm group to the list of synonm groups.
                SynonymGroups.Add(new ReadOnlyCollection<string>(synonymGroupList));
            }

            // clear the xml document
            Doc = null;
        }

        #region ISynonymEngine Members

        public IEnumerable<string> GetSynonyms(string word)
        {
            //enumerate all the synonym groups
            foreach (var synonymGroup in SynonymGroups)
            {
                //if the word is a part of the group return 
                //the group as the results.
                if (synonymGroup.Contains(word))
                {
                    //gonna use a read only collection for security purposes
                    return synonymGroup;
                }
            }

            return null;
        }

        #endregion
    }
}
