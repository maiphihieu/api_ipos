using iIpos_core.Dto.Store;
using iIpos_core.Service.StoreService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace iIpos_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var stores = await _storeService.GetAllAsync(pageNumber, pageSize);
            return Ok(stores);
        }
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var store = await _storeService.GetByIdAsync(id);
            if (store == null)
            {
                return NotFound();
            }
            return Ok(store);
        }
        [AllowAnonymous]
        [HttpGet("{storeId}/menu")]
        public async Task<IActionResult> GetFullMenu(int storeId)
        {
            var menu = await _storeService.GetFullMenuAsync(storeId);
            return Ok(menu);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUpdateStoreDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var newStore = await _storeService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = newStore.Id }, newStore);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateStoreDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedStore = await _storeService.UpdateAsync(id, updateDto);
            if (updatedStore == null)
            {
                return NotFound();
            }
            return Ok(updatedStore);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _storeService.DeleteAsync(id);
            if (!success)
            {
                return BadRequest("Cannot delete store. It may have related branches or products, or does not exist.");
            }
            return NoContent();
        }
    }
}
