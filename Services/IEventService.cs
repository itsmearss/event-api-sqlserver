using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<EventResponseDTO> GetEventByIdAsync(int id);
        Task<EventResponseDTO> CreateEventAsync(EventDTO eventDTO);
        Task<EventResponseDTO> UpdateEventAsync(int id, EventDTO eventDTO);
        Task<bool> DeleteEventAsync(int id);
        Task<List<ExportEvent>> GetDataExportEvent();
    }
}
