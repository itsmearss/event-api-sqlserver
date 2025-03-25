using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);
        Task<Event> CreateEventAsync(Event eventEntity);
        Task<Event> UpdateEventAsync(Event eventEntity);
        Task<bool> DeleteEventAsync(int id);
    }
}
