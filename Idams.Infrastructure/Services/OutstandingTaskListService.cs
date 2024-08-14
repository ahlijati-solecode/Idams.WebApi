using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Entities.Custom;
using Idams.Core.Model.Filters;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.Utils;

namespace Idams.Infrastructure.Services
{
    public class OutstandingTaskListService : IOutstandingTaskListService
    {
        private readonly IOutstandingTaskListRepository _outstandingTaskListRepository;

        public OutstandingTaskListService(IOutstandingTaskListRepository outstandingTaskListRepository)
        {
            _outstandingTaskListRepository = outstandingTaskListRepository;
        }

        public async Task<Paged<OutstandingTaskListPaged>> GetPaged(PagedDto pagedDto, OutstandingTaskListFilter filter, UserDto user)
        {
            UserUtil.DetermineUserPrivelege(user, out string? hierlvl2, out string? hierlvl3);
            filter.HierLvl2 = hierlvl2;
            filter.HierLvl3 = hierlvl3;

            return await _outstandingTaskListRepository.GetPaged(filter, pagedDto.Page, pagedDto.Size, pagedDto.Sort);
        }
    }
}

