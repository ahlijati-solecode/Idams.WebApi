
namespace Idams.Core.Model.Requests
{
    public class GetAvailableHierRequest
    {
        public List<string> HierLvl2s { get; set; } = new List<string>();
        public List<string> HierLvl3s { get; set; } = new List<string>();
    }
}
