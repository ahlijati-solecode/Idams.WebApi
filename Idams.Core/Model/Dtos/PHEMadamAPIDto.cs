namespace Idams.Core.Model.Dtos
{
    public class PHEMadamAPIDto
    {
        public string? Status { get; set; }
        public PHEMadamAPIDtoObject? Object { get; set; }
        public string? Message { get; set; }
    }


    public class PHEMadamAPIDtoObject
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
