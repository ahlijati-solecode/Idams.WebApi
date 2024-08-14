using Idams.Core.Model.Dtos;

namespace Idams.Core.Repositories
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetData(string? regionalFilter, string? rkapFilter, string? thresholdFilter);
    }
}
