using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Repositories
{
    public interface IFollowUpRepository
    {
        Task<FollowUpDropDownDto> GetDropdownList();
        Task<List<FollowUpDetailResponse>> GetFollowUpList(string projectActionId);
        Task<List<TxFollowUp>> AddFollowUpList(List<FollowUpRequest> followUpList);
        Task<TxFollowUp> UpdateFollowUp(FollowUpRequest followUp);
        Task<bool> DeleteFollowUp(int followUpId, string projectActionId);
    }
}
