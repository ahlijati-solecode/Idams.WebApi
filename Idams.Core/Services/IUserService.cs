using Idams.Core.Model.Dtos;

namespace Idams.Core.Services
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);
        Task<UserDto> GetUser(string username);
        Task<List<MenuDto>> GetUserMenu(string username);
        Task<EmployeeDto?> GetEmployee(string username, string MasterEmployeeUrl);
        Task<List<EmployeeDto>> GetUserSuggestion(string email);
        Task GetBaseUrlListAsync();
    }
}
