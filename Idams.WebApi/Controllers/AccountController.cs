using Idams.Core.Http;
using Idams.Core.Model.Dtos;
using Idams.Core.Services;
using Idams.WebApi.Constants;
using Idams.WebApi.Utils;
using Idams.WebApi.Utils.Attributes;
using Idams.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;

namespace Idams.WebApi.Controllers
{
    public class AccountController : ApiController
    {
        private readonly IDistributedCache _distributedCache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _iUserService;
        private readonly IConfiguration _configuration;

        public AccountController(IDistributedCache distributedCache,
            IHttpContextAccessor httpContextAccessor,
            IUserService iUserService,
            IConfiguration configuration)
        {
            _distributedCache = distributedCache;
            _httpContextAccessor = httpContextAccessor;
            _iUserService = iUserService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> Signout(string? returnUrl)
        {
            if (returnUrl == null)
                returnUrl = _configuration["Keycloak:Callback"];
            _httpContextAccessor.HttpContext?.Session.SetString("ReturnUrl", returnUrl);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            Response.Cookies.Delete(AuthConstants.HEADER_AUTH);
            return new SignOutResult(new[] { CookieAuthenticationDefaults.AuthenticationScheme });
        }

        [ApiAuthorize]
        [HttpGet()]
        public IActionResult CurrentUser()
        {
            return Ok(User.Claims.Select(n => new { n.Type, n.Value }));
        }

        [HttpGet()]
        public IActionResult Callback(string? returnUrl)
        {
            if (Request.Cookies[AuthConstants.HEADER_AUTH] != null)
            {
                ViewBag.Token = Request.Cookies[AuthConstants.HEADER_AUTH];
            }
            bool validUri = Uri.TryCreate(returnUrl, UriKind.Absolute, out Uri? uri);
            if (!validUri || uri == null)
                uri = GetUri();
            var path = uri.AbsolutePath;
            if (path == "/")
                path = "^";
            return View("Callback", uri.ToString().Replace(path, string.Empty));
        }

        private Uri GetUri()
        {
            var url = _distributedCache.Get($"{ViewBag.Token}_url").To<string>() ?? Request.Headers.Referer;
            if (url == null)
                url = _configuration["Keycloak:Callback"];
            return new Uri(url);
        }

        [HttpGet()]
        [ApiAuthorize]
        public async Task<IActionResult> UserDataAsync()
        {
            return Ok(await Task.FromResult(HttpContext.GetUserInfo()));
        }

        [HttpGet()]
        [ApiAuthorize]
        public async Task<IActionResult> UserMenuAsync()
        {
            var userInfo = HttpContext.GetUserInfo();
            return Ok(await Task.FromResult(userInfo.Menu));
        }

    }
}
