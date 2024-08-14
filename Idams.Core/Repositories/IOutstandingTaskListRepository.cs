using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;

namespace Idams.Core.Repositories
{
    public interface IOutstandingTaskListRepository
    {
        Task<Paged<OutstandingTaskListPaged>> GetPaged(OutstandingTaskListFilter filter, int page, int size, string sort);
    }
}

