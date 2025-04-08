using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Data.Models;

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
        Task<SaveResponse> ProcessUserImport(List<UserDTO> excelData);
        string HashPassword(string password);
    }
}
