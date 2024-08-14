using Idams.Core.Enums;
using Idams.Core.Services;
using Idams.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Idams.WebApi.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _specifiedMenus;

        public ApiAuthorizeAttribute(params string[] menus)
        {
            _specifiedMenus = menus;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.Filters.OfType<AllowAnonymousAttribute>().Any())
                return;

            var guid = context.HttpContext.Request.Headers["x-api-key"].ToString();
            if (string.IsNullOrWhiteSpace(guid))
            {
                context.Result = new UnauthorizedObjectResult(new { code = 401, message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
            else
            {
                var distributedCache = context.HttpContext.RequestServices.GetService<IDistributedCache>();
                var accessToken = distributedCache.Get(guid).To<string>();
                bool userAlreadySet = false;
                if(accessToken == null)
                {
                    accessToken = context.HttpContext.GetTokenAsync("access_token").Result;
                    if(accessToken != null)
                    {
                        var userService = context.HttpContext.RequestServices.GetService<IUserService>();
                        var username = context.HttpContext.User.FindFirst("preferred_username")?.Value!;
                        var userInfo = userService!.GetUser(username).Result;
                        distributedCache.Set(guid, accessToken.ToBytes());
                        distributedCache.Set($"{guid}_claims", context.HttpContext.User.Claims.Select(n => new CustomClaim { Type = n.Type, Value = n.Value }).ToBytes());
                        distributedCache.Set($"{guid}_userInfo", userInfo.ToBytes());
                        userAlreadySet = true;
                    }
                }
                if (accessToken != null)
                {
                    if (!userAlreadySet)
                    {
                        var claims = distributedCache.Get($"{guid}_claims").To<IEnumerable<CustomClaim>>();
                        context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims.Select(n => new Claim(n.Type, n.Value)), "Basic"));
                    }                    
                    ValidateUserRoles(context);
                }
                else
                {
                    context.Result = new UnauthorizedObjectResult(new
                    {
                        errorCode = ErrorCode.TokenEmpty,
                        status = "Failed",
                        message = ResultCodeMessages.GetResultCode(ErrorCode.TokenEmpty),
                    })
                    { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
        }

        private void ValidateUserRoles(AuthorizationFilterContext context)
        {
            if (_specifiedMenus == null || _specifiedMenus.Length < 1)
                return;

            var userInfo = context.HttpContext.GetUserInfo();
            if (!userInfo.Menu.Any(m => _specifiedMenus.Contains(m.Name)))
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    errorCode = ErrorCode.Forbidden,
                    status = "Failed",
                    message = ResultCodeMessages.GetResultCode(ErrorCode.Forbidden),
                })
                { StatusCode = StatusCodes.Status403Forbidden };
            }
        }
    }

    public enum AuthType
    {
        WhiteList,
        BlackList
    }
}