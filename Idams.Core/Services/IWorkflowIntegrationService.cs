using Idams.Core.Model.Dtos;

namespace Idams.Core.Services
{
    public interface IWorkflowIntegrationService
    {
        Task<PHEMadamAPIDto?> DoTransaction(string action , string transno, string startWf, string actionFor, string actionBy);
    }
}
