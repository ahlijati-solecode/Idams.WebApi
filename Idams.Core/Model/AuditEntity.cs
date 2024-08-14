namespace Idams.Core.Model
{
    public class AuditEntity : BaseAuditEntity
    {
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }

    }
}