using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Repositories
{
    public interface IRegisterEventRepository
    {
        Task<IEnumerable<RegisterEvent>> GetAllRegisterEventsAsync();
        Task<IEnumerable<RegisterEvent>> GetRegisterEventByEventIdAsync(int eventId);
        Task<IEnumerable<RegisterEvent>> GetRegisterEventsByUserIdAsync(int userId);
        Task<RegisterEvent> GetRegisterEventAsync(int userId, int eventId);
        Task<RegisterEvent> GetRegisterEventByIdAsync(int id);
        Task<RegisterEvent> CreateRegisterEventAsync(RegisterEvent registerEvent);
        Task<RegisterEvent> UpdateRegisterEventAsync(RegisterEvent registerEvent);
        Task<bool> DeleteRegisterEventAsync(int id);
    }
}
