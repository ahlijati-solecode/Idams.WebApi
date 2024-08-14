using AutoMapper;
using Idams.Core.Model.Requests;
using Idams.Core.Model.Responses;
using Idams.Core.Services;
using Idams.WebApi.Constants;
using Idams.WebApi.Utils;
using Idams.WebApi.Utils.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Idams.WebApi.Controllers
{
    [Route("calendar")]
    [ApiController]
    public class CalendarController : ApiController
    {
        private readonly IProjectService _projectService;
        private readonly IMapper _mapper;

        const string Success = "Success";
        public CalendarController(IProjectService projectService, IMapper mapper)
        {
            _projectService = projectService;
            _mapper = mapper;
        }

        [HttpGet("dropdown")]
        [ApiAuthorize()]
        public async Task<IActionResult> GetDropdownCalendarEvent()
        {
            var res = await _projectService.GetCalendarEventDropdown();
            Message = Success;
            return Ok(_mapper.Map<CalendarEventDropdownResponse>(res));
        }

        [HttpGet("meeting")]
        [ApiAuthorize()]
        public async Task<IActionResult> GetCalendar([FromQuery] CalendarEventRequest request)
        {
            var res = await _projectService.GetCalendarEvent(request.StartDate, request.EndDate, request.ProjectId, request.Threshold);
            Message = Success;
            return Ok(_mapper.Map<List<MeetingWithProjectAndStageResponse>>(res));
        }
    }
}
