namespace Idams.Core.Model.Entities
{
    public partial class MdParamHeader: AuditEntity
    {
        public string ParamId { get; set; } = null!;
        public string? ParamDesc { get; set; }
        
    }
}
