namespace Raven
{
    public class TestComment
    {
        public TestComment(int id, string comment)
        {
            Id = id;
            Comment = comment;
        }

        public int Id { get; set; }
        public string Comment { get; set; }
    }
}