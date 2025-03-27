using TestProjectAnnur.Data.Models;
using Microsoft.EntityFrameworkCore;
using TestProjectAnnur.Data;

namespace TestProjectAnnur.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> CreateUserAsync(User userEntity, int role)
        {
            userEntity.CreatedAt = DateTime.UtcNow;
            await _context.Users.AddAsync(userEntity); 
            await _context.SaveChangesAsync();
            var userRole = new UserRole
            {
                UserId = userEntity.Id,
                RoleId = role
            };
            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
            return userEntity;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (existingUser == null)
                return false;

            _context.Users.Remove(existingUser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> UpdateUserAsync(User userEntity)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userEntity.Id);

            if (existingUser == null)
                return null;

            existingUser.Username = userEntity.Username;
            existingUser.Fullname = userEntity.Fullname;
            existingUser.Password = userEntity.Password;
            existingUser.UpdatedAt = DateTime.UtcNow;

            _context.Users.Update(existingUser);
            await _context.SaveChangesAsync();

            return existingUser;
        }
    }
}
