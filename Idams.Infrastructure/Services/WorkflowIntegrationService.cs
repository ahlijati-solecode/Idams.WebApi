using Idams.Core.Model.Dtos;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web;

namespace Idams.Infrastructure.Services
{
    public class WorkflowIntegrationService : IWorkflowIntegrationService
    {
        private readonly IParameterListRepository _parameterListRepository;
        private readonly IMappingObject _mapper;
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        public string MadamToken { get; private set; }
        public string MadamUrl { get; private set; }

        public WorkflowIntegrationService(IParameterListRepository parameterListRepository, IMappingObject mapper, IUserService userService, ILogger<IWorkflowIntegrationService> logger)
        {
            _parameterListRepository = parameterListRepository;
            _mapper = mapper;
            _userService = userService;
            _logger = logger;
            this.MadamToken = "";
            this.MadamUrl = "";
        }

        public async Task<PHEMadamAPIDto?> DoTransaction(string action, string transno, string startWf, string actionFor, string actionBy)
        {
            try
            {
                var jsonString = "";
                HttpClient httpClient = new HttpClient();

                var appfkReq = await _parameterListRepository.GetParams("idams", "AppFK");
                var urlReq = await _parameterListRepository.GetParams("idams", "Url");

                this.MadamUrl = urlReq.Where(n => n.ParamListId == "BaseUrl").First().ParamValue1Text!;
                var appFk = appfkReq.Where(n => n.ParamListId == "AppFK").First().ParamValue1Text!;
                var token = urlReq.Where(n => n.ParamListId == "token").First().ParamListDesc!;
                this.MadamToken = token ?? "";

                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("X-User-Auth", this.MadamToken);
                message.Method = HttpMethod.Post;

                UriBuilder builder = new UriBuilder(this.MadamUrl);
                builder.Path = _parameterListRepository.GetParam("idams", "url", "WorkflowTransaction").Result!.ParamValue1Text!;
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["appId"] = appFk;
                query["companyCode"] = "5000";
                query["transNo"] = transno;
                query["startWF"] = startWf;
                query["action"] = action;
                query["actionBy"] = actionBy;
                query["actionFor"] = actionFor;
                query["source"] = "IDAMS";
                query["notes"] = "-";
                builder.Query = query.ToString() ?? "";

                message.RequestUri = builder.Uri;
                var res = await httpClient.SendAsync(message);

                if (res.IsSuccessStatusCode)
                {
                    jsonString = await res.Content.ReadAsStringAsync();
                }
                return JsonConvert.DeserializeObject<PHEMadamAPIDto>(jsonString)!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Can't do Transaction");
            }
            return null;
        }
    }
}
