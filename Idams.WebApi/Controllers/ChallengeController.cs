using Idams.Core.Services;
using Idams.WebApi.Constants;
using Idams.WebApi.Utils;
using Idams.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;

namespace Idams.WebApi.Controllers
{
    [Route("challenge")]
    [ApiController]
    public class ChallengeController : ApiController
    {
        private readonly IDistributedCache _distributeCache;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;

        public ChallengeController(IDistributedCache distributedCache,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService)
        {
            _distributeCache = distributedCache;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        [HttpGet("callback")]
        [Authorize]
        public async Task<IActionResult> Callback()
        {
            var returnUrl = _httpContextAccessor.HttpContext?.Session.GetString("ReturnUrl") ?? Request.Cookies[AuthConstants.HEADER_CALLBACK] ?? _configuration["Keycloak:Callback"];
            return await Index(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? returnUrl)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            returnUrl = GenerateReturnUrl(returnUrl);
            _httpContextAccessor.HttpContext?.Session.SetString("ReturnUrl", returnUrl);
            CookieOptions cookieOptions = GetCookiesOptions();

            Response.Cookies.Append(AuthConstants.HEADER_CALLBACK, returnUrl, cookieOptions);
            if (token == null)
            {
                return RedirectToAction("callback", "Challenge");
            }
            ViewBag.accessToken = token;
            ViewBag.idToken = await HttpContext.GetTokenAsync("id_token");
            string guid = Guid.NewGuid().ToString();
            if (Request.Cookies[AuthConstants.HEADER_AUTH] != null)
                guid = Request.Cookies[AuthConstants.HEADER_AUTH] ?? guid;
            
            ViewBag.Token = guid;
            await SetCache(returnUrl, token, cookieOptions, guid);
            return Redirect(returnUrl);
        }

        private async Task SetCache(string returnUrl, string token, CookieOptions cookieOptions, string guid)
        {
            if (_distributeCache.Get(guid) == null)
            {
                var username = User.FindFirst("preferred_username")?.Value!;
                var userInfo = await _userService.GetUser(username);
                _distributeCache.Set(guid, token.ToBytes());
                _distributeCache.Set($"{guid}_claims", User.Claims.Select(n => new CustomClaim { Type = n.Type, Value = n.Value }).ToBytes());
                _distributeCache.Set($"{guid}_url", returnUrl.ToBytes());
                _distributeCache.Set($"{guid}_userInfo", userInfo.ToBytes());
                Response.Cookies.Append(AuthConstants.HEADER_AUTH, guid, cookieOptions);
            }
        }

        private string GenerateReturnUrl(string? returnUrl)
        {
            if (returnUrl == null)
                returnUrl = _httpContextAccessor.HttpContext?.Session.GetString("ReturnUrl") ?? _configuration["Keycloak:Callback"];
            return returnUrl;
        }

        private static CookieOptions GetCookiesOptions()
        {
            return new CookieOptions()
            {
                IsEssential = true,
                Expires = DateTime.MaxValue,
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None
            };
        }
    }
}
