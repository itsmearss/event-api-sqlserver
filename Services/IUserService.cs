using TestProjectAnnur.Data.DTOs;

namespace TestProjectAnnur.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<UserResponseDTO> GetUserByIdAsync(int id);
        Task<UserResponseDTO> GetUserByUsernameAsync(string username);
        Task<UserResponseDTO> CreateUserAsync(UserDTO userDto);
        Task<UserResponseDTO> UpdateUserAsync(int id, UserDTO userDto);
        Task<bool> DeleteUserAsync(int id);
        string HashPassword(string password);
    }
}
