using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Data.Models;
using TestProjectAnnur.Repositories;

namespace TestProjectAnnur.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryResponseDTO> CreateCategoryAsync(CategoryDTO categoryDto)
        {
            var categoryEntity = new Category
            {
                Name = categoryDto.Name
            };

            var createdCategory = await _categoryRepository.CreateCategoryAsync(categoryEntity);
            return MapToResponseDTO(createdCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await _categoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<IEnumerable<CategoryResponseDTO>> GetAllCategoryAsync()
        {
            var categories = await _categoryRepository.GetAllCategoryAsync();
            return categories.Select(MapToResponseDTO);
        }

        public async Task<CategoryResponseDTO> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryById(id);

            if (category == null)
                return null;

            return MapToResponseDTO(category);
        }

        public async Task<CategoryResponseDTO> UpdateCategoryAsync(int id, CategoryDTO categoryDTO)
        {
            var existingCategory = await _categoryRepository.GetCategoryById(id);

            if (existingCategory == null)
                return null;

            existingCategory.Name = categoryDTO.Name;
            existingCategory.UpdatedAt = DateTime.UtcNow;

            var updateCategory = await _categoryRepository.UpdateCategoryAsync(existingCategory);
            return MapToResponseDTO(updateCategory);
        }

        private CategoryResponseDTO MapToResponseDTO(Category category)
        {
            return new CategoryResponseDTO
            {
                Id = category.Id,
                Name = category.Name,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt
            };
        }
    }
}
