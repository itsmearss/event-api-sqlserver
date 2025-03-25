using TestProjectAnnur.Data.DTOs;

namespace TestProjectAnnur.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO registerDTO);
        Task<AuthResponseDTO> LoginAsync(LoginDTO loginDTO);
    }
}
