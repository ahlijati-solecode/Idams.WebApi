using Idams.Core.Model.Dtos;
using Idams.Core.Model.Entities;

namespace Idams.Core.Services
{
    public interface ICurrentUserService
    {
        User CurrentUser { get; }
        UserDto CurrentUserInfo { get; }
    }
}