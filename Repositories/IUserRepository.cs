using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(User userEntity, int role);
        Task<User> UpdateUserAsync(User userEntity);
        Task<bool> DeleteUserAsync(int id);
    }
}
