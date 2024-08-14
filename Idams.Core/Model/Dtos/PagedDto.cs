namespace Idams.Core.Model.Dtos
{
    public class PagedDto
    {
        public int Size { get; set; } = 10;
        public int Page { get; set; } = 1;
        public string Sort { get; set; } = "id asc";
    }
    public class AuditEntityDto
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
