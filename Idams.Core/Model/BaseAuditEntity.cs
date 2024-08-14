namespace Idams.Core.Model
{
    public class BaseAuditEntity
    {
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}