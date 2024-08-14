using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.Core.Repositories
{
    public interface IParameterListRepository
    {
        Task<MdParamaterListDto?> GetParam(string schema, string? ParamID = null, string? ParamListID = null);
        Task<List<MdParamaterList>> GetParams(string schema, string paramID);
        Task<List<MdParamaterList>> GetParams(List<ParamFilter> filter);
    }
}
