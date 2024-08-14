using Idams.Core.Model;

namespace Idams.Infrastructure.EntityFramework.Core
{
    public class AuditEntityWithApprove : AuditEntity
    {
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}