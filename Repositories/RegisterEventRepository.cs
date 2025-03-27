using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestProjectAnnur.Data;
using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Repositories
{
    public class RegisterEventRepository : IRegisterEventRepository
    {
        private readonly ApplicationDbContext _context;

        public RegisterEventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterEvent> CreateRegisterEventAsync(RegisterEvent registerEvent)
        {
            registerEvent.CreatedAt = DateTime.UtcNow;
            await _context.RegisterEvents.AddAsync(registerEvent);
            await _context.SaveChangesAsync();
            
            return registerEvent;
        }

        public async Task<bool> DeleteRegisterEventAsync(int id)
        {
            var existingRegisterEvent = await _context.RegisterEvents.FirstOrDefaultAsync(u => u.Id == id);
            
            if (existingRegisterEvent == null)
            {
                return false;
            }

            _context.RegisterEvents.Remove(existingRegisterEvent);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RegisterEvent>> GetAllRegisterEventsAsync()
        {
            return await _context.RegisterEvents.ToListAsync();
        }

        public async Task<RegisterEvent> GetRegisterEventAsync(int userId, int eventId)
        {
            return await _context.RegisterEvents.FirstOrDefaultAsync(u => u.EventId == eventId && u.UserId == userId);
        }

        public async Task<IEnumerable<RegisterEvent>> GetRegisterEventByEventIdAsync(int eventId)
        {
            return await _context.RegisterEvents.Where(u => u.EventId == eventId).ToListAsync();
        }

        public async Task<RegisterEvent> GetRegisterEventByIdAsync(int id)
        {
            var response = await _context.RegisterEvents.FirstOrDefaultAsync(u => u.Id == id);
            return response;
        }

        public async Task<IEnumerable<RegisterEvent>> GetRegisterEventsByUserIdAsync(int userId)
        {
            return await _context.RegisterEvents.Where(u => u.UserId == userId).ToListAsync();
        }

        public async Task<RegisterEvent> UpdateRegisterEventAsync(RegisterEvent registerEvent)
        {
            var existingRe = await _context.RegisterEvents.FirstOrDefaultAsync(u => u.Id == registerEvent.Id);

            if(existingRe == null)
            {
                throw new Exception($"Register Event dengan ID: {registerEvent.Id} tidak ditemukan");
            }

            existingRe.UpdatedAt = DateTime.UtcNow;

            _context.RegisterEvents.Update(existingRe);
            await _context.SaveChangesAsync();
            return existingRe;

        }
    }
}
