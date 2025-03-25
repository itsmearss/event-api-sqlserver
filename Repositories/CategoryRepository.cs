using Microsoft.EntityFrameworkCore;
using TestProjectAnnur.Data;
using TestProjectAnnur.Data.Models;

namespace TestProjectAnnur.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            category.CreatedAt = DateTime.UtcNow;
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(u => u.Id == id);

            if (existingCategory == null)
                return false;

            _context.Categories.Remove(existingCategory);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Category>> GetAllCategoryAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category> GetCategoryById(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(u => u.Id == category.Id);

            if (existingCategory == null)
                return null;

            existingCategory.Name = category.Name;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            _context.Categories.Update(existingCategory);
            await _context.SaveChangesAsync();

            return existingCategory;
        }
    }
}
