using Idams.Core.Model.Entities;

namespace Idams.Core.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
    }
}
