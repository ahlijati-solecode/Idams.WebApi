using Idams.Core.Http;
using Idams.Core.Model.Requests;
using Idams.Core.Repositories;
using Idams.Core.Services;
using Idams.Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Idams.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IParameterListRepository _parameterListRepository;
        private readonly IMappingObject _mapper;
        private readonly IUserService _userService;
        private readonly ILogger _logger;
        public string MadamToken { get; private set; }
        public string MadamUrl { get; private set; }

        public EmailService(IUserRepository userRepository,
            IParameterListRepository parameterListRepository,
            IHierLvlVwRepository hierLvlVwRepository,
            IMappingObject mapper,
            IUserService userService,
            ILogger<IEmailService> logger)
        {
            _parameterListRepository = parameterListRepository;
            _mapper = mapper;
            _userService = userService;
            this.MadamToken = "";
            this.MadamUrl = "";
            _logger = logger;
        }

        public async Task SendMailAsync(List<SendMailRequest> requests)
        {
            HttpClient httpClient = new HttpClient();

            var appfkReq = await _parameterListRepository.GetParams("idams", "AppFK");
            var urlReq = await _parameterListRepository.GetParams("idams", "Url");

            this.MadamUrl = urlReq.Where(n => n.ParamListId == "BaseUrl").First().ParamValue1Text!;
            var propSendMail = appfkReq.Where(n => n.ParamListId == "PropSendMail").First().ParamValue1Text!;
            var emailSender = "phe.apps@pertamina.com";
            var appFk = appfkReq.Where(n => n.ParamListId == "AppFK").First().ParamValue1Text!;
            var token = appfkReq.Where(n => n.ParamListId == "token").First().ParamListDesc!;
            this.MadamToken = token ?? "";

            var bcc = _parameterListRepository.GetParam("idams", "Bcc", "BccEmail").Result!.ParamValue1Text!;
            List<HttpRequestMessage> messages = new List<HttpRequestMessage>();

            foreach(var request in requests)
            {
                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("X-User-Auth", this.MadamToken);
                message.Headers.Add("X-User-Prop", propSendMail);
                message.Method = HttpMethod.Post;

                UriBuilder builder = new UriBuilder(this.MadamUrl);
                builder.Path = _parameterListRepository.GetParam("idams", "url", "SendAppMail").Result!.ParamValue1Text!;
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["appfk"] = appFk;
                query["emailcode"] = request.EmailCode;
                query["transno"] = "transno";
                query["actionby"] = request.ActionBy;
                query["emailsender"] = emailSender;
                query["to"] = request.To;
                if (request.Cc != null) query["cc"] = request.Cc;
                query["bcc"] = bcc;
                var body = new StringContent(JsonConvert.SerializeObject(request.ListJson), Encoding.UTF8, "application/json");
                builder.Query = query.ToString() ?? "";

                message.RequestUri = builder.Uri;
                message.Content = body;
                messages.Add(message);
            }
            _ =  Task.Run(() => SendMail(messages));
        }

        public async Task SendMail(List<HttpRequestMessage> messages)
        {
            var httpClient = new HttpClient();
            try
            {
                foreach(var message in messages)
                {
                    await httpClient.SendAsync(message);
                }
            }catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to Send Email");
            }
        }
    }
}
