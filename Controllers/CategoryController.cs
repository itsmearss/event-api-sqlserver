using Microsoft.AspNetCore.Mvc;
using TestProjectAnnur.Data.DTOs;
using TestProjectAnnur.Services;

namespace TestProjectAnnur.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase 
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoryAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
                return NotFound($"Category dengan ID {id} tidak ditemukan");

            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdCategory = await _categoryService.CreateCategoryAsync(categoryDTO);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryDTO);

            if (updatedCategory == null)
                return NotFound($"Category dengan ID {id} tidak ditemukan");

            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleteCategory = await _categoryService.DeleteCategoryAsync(id);

            if (!deleteCategory)
                return NotFound($"Category dengan ID {id} tidak ditemukan");

            return Ok(deleteCategory);
        }
    }
}
