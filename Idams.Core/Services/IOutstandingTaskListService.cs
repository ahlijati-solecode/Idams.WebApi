using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;

namespace Idams.Core.Services
{
    public interface IOutstandingTaskListService
    {
        Task<Paged<OutstandingTaskListPaged>> GetPaged(PagedDto pagedDto, OutstandingTaskListFilter filter, UserDto user);
    }
}

