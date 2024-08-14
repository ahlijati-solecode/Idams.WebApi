namespace Idams.Infrastructure.EntityFramework.Core
{
    public interface IsDeletedEntity
    {
        int? DeletedBy { get; set; }
        DateTime? DeletedDate { get; set; }
    }
}