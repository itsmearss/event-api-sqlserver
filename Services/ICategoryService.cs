using TestProjectAnnur.Data.DTOs;

namespace TestProjectAnnur.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDTO>> GetAllCategoryAsync();
        Task<CategoryResponseDTO> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDTO> CreateCategoryAsync(CategoryDTO categoryDto);
        Task<CategoryResponseDTO> UpdateCategoryAsync(int id, CategoryDTO categoryDTO);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
