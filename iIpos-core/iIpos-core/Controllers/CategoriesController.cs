using iIpos_core.Dto.Category;
using iIpos_core.Service.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ilpos_core.Controllers
{
    [Route("api/categories")]
    [ApiController]
    [Authorize]

    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? storeId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize =9)
        {
            var result = await _categoryService.GetAllAsync(storeId, pageNumber, pageSize);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUpdateCategoryDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newCategory = await _categoryService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = newCategory.Id }, newCategory);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateCategoryDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedCategory = await _categoryService.UpdateAsync(id, updateDto);
            if (updatedCategory == null) return NotFound();
            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _categoryService.DeleteAsync(id);
            if (!success) return BadRequest("Cannot delete category.");
            return NoContent();
        }
    }
}