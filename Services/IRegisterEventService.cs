using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Services
{
    public interface IRegisterEventService
    {
        Task<IEnumerable<RegisterEventResponseDTO>> GetAllRegisterEventsAsync();
        Task<IEnumerable<RegisterEventResponseDTO>> GetRegisterEventsByUserIdAsync(int id);
        Task<IEnumerable<RegisterEventResponseDTO>> GetRegisterEventsByEventIdAsync(int id);
        Task<RegisterEventResponseDTO> GetRegisterEventAsync(int userId, int eventId);
        Task<RegisterEventResponseDTO> GetRegisterEventById(int id);
        Task<RegisterEventResponseDTO> CreateRegisterEventAsync(RegisterEventDTO registerEventDTO);
        Task<RegisterEventResponseDTO> UpdateRegisterEventAsync(int id, RegisterEventDTO registerEventDTO);
        Task<bool> DeleteRegisterEventAsync(int id);
    }
}
