using Idams.Core.Model.Entities;
using Idams.Core.Repositories;
using Idams.Core.Services;

namespace Idams.Infrastructure.Services
{
    public class HomeService : IHomeService
    {
        IParameterListRepository _parameterListRepository;

        public HomeService(IParameterListRepository parameterListRepository)
        {
            _parameterListRepository = parameterListRepository;
        }

        public async Task<Dictionary<string, string>> GetAboutIdamsUrls()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            List<MdParamaterList> list = await _parameterListRepository.GetParams("idams", "HelpDoc");

            foreach (MdParamaterList e in list)
            {
                result.Add(e.ParamListId, string.IsNullOrEmpty(e.ParamValue1Text) ? "" : e.ParamValue1Text);
            }

            return result;
        }
    }
}

