using Idams.Core.Repositories;
using Idams.WebApi.Utils;
using Microsoft.AspNetCore.Mvc;
namespace Idams.WebApi.Controllers
{
    [Route("dashboard")]
    [ApiController]
    public class DashboardController : ApiController
    {
        private readonly IDashboardService _dashboardService;
        public const string Success = "Success";

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("data")]
        public async Task<object> GetData(string? regionalFilter, string? rkapFilter, string? thresholdFilter)
        {
            var res = await _dashboardService.GetData(regionalFilter, rkapFilter, thresholdFilter);
            Message = Success;

            return Ok(res);
        }
    }
}

