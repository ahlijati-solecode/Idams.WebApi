using AutoMapper;
using Idams.Core.Model.Dtos;
using Idams.Core.Model.Filters;
using Idams.Core.Model.Requests;
using Idams.Core.Services;
using Idams.WebApi.Utils;
using Idams.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
namespace Idams.WebApi.Controllers
{
    [Route("outstandingTaskList")]
    [ApiController]
    public class OutstandingTaskListController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly IOutstandingTaskListService _outstandingTaskListService;
        public const string Success = "Success";

        public OutstandingTaskListController(IOutstandingTaskListService outstandingTaskListService, IMapper mapper)
        {
            _outstandingTaskListService = outstandingTaskListService;
            _mapper = mapper;
        }

        [HttpGet("list")]
        public async Task<object> GetPagedAsync([FromQuery] OutstandingTaskListPagedRequest request)
        {
            var userInfo = HttpContext.GetUserInfo();
            var res = await _outstandingTaskListService.GetPaged(_mapper.Map<PagedDto>(request), _mapper.Map<OutstandingTaskListFilter>(request), userInfo!);
            Message = Success;

            return Ok(res);
        }
    }
}

