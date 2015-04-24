using System;

namespace Raven
{
    public class Comment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int SdKey { get; set; }
        public int IbKey { get; set; }
        public string Survey { get; set; }
        public string SurveyItem { get; set; }
        public string SurveyComment { get; set; }
    }
}