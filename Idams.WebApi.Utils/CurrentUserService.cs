using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;
using Idams.Core.Services;
using Idams.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Http;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Idams.WebApi.Utils
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const string EMAIL = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public User CurrentUser
        {
            get
            {
                var user = _httpContextAccessor.HttpContext.User;
                return new User()
                {
                    Email = user.Claims.FirstOrDefault(n => n.Type == EMAIL)?.Value,
                    FullName = user.Claims.FirstOrDefault(n => n.Type == Claims.Name)?.Value,
                    Id = user.Claims.FirstOrDefault(n => n.Type == Claims.PreferredUsername)?.Value,
                    UserName = user.Claims.FirstOrDefault(n => n.Type == Claims.PreferredUsername)?.Value,
                };
            }
        }

        public UserDto CurrentUserInfo => _httpContextAccessor.HttpContext.GetUserInfo();
    }
}