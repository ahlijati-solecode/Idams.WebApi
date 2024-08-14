using Idams.Core.Model.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Idams.WebApi.Utils.Extensions
{
    public static class HttpContextExtension
    {
        public static UserDto GetUserInfo(this HttpContext httpContext)
        {
            var distributedCache = httpContext.RequestServices.GetService<IDistributedCache>();
            var guid = httpContext.Request.Headers["x-api-key"].ToString();
            return distributedCache!.Get($"{guid}_userInfo").To<UserDto>();
        }
    }
}
