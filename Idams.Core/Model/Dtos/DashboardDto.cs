namespace Idams.Core.Model.Dtos
{
    public class DashboardDto
    {
        public Dictionary<String, Dictionary<String, Dictionary<String, Decimal>>>? Stage { get; set; }
        public Dictionary<String, decimal>? Total { get; set; }
        public List<String>? RkapDropdown { get; set; }
    }
}

