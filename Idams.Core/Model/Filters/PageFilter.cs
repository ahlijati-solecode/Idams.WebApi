namespace Idams.Core.Model.Filters
{
    public class PageFilter
    {
        public int Size { get; set; }
        public int Page { get; set; }
        public string Sort { get; set; } = "id asc";
    }
}