using Microsoft.EntityFrameworkCore;
using TestProjectAnnur.Data;
using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Event> CreateEventAsync(Event eventEntity)
        {
            eventEntity.CreatedAt = DateTime.UtcNow;
            await _context.Events.AddAsync(eventEntity);
            await _context.SaveChangesAsync();
            return eventEntity;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var existingEvent = await _context.Events.FirstOrDefaultAsync(u => u.Id == id);

            if (existingEvent == null)
                return false;

            _context.Events.Remove(existingEvent);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _context.Events.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Event> UpdateEventAsync(Event eventEntity)
        {
            var existingEvent = await _context.Events.FirstOrDefaultAsync(u => u.Id == eventEntity.Id);

            if (existingEvent == null)
                return null;

            existingEvent.Title = eventEntity.Title;
            existingEvent.Description = eventEntity.Description;
            existingEvent.StartTime = eventEntity.StartTime;
            existingEvent.EndTime = eventEntity.EndTime;
            existingEvent.Location = eventEntity.Location;
            existingEvent.CategoryId = eventEntity.CategoryId;
            existingEvent.MaxAttendees = eventEntity.MaxAttendees;
            existingEvent.Status = eventEntity.Status;
            existingEvent.UpdatedAt = DateTime.UtcNow;

            _context.Events.Update(existingEvent);
            await _context.SaveChangesAsync();
            return existingEvent;
        }
    }
}
