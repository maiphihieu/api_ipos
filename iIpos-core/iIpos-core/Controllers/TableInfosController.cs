using iIpos_core.Dto.TableInfo;
using iIpos_core.Service.TableInfo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ilpos_core.Controllers
{
    [Route("api/tables")]
    [ApiController]
    [Authorize]
    public class TableInfosController : ControllerBase
    {
        private readonly ITableInfoService _tableInfoService;

        public TableInfosController(ITableInfoService tableInfoService)
        {
            _tableInfoService = tableInfoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? storeId,
            [FromQuery] int? branchId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _tableInfoService.GetAllAsync(storeId, branchId, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var table = await _tableInfoService.GetByIdAsync(id);
            if (table == null) return NotFound();
            return Ok(table);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUpdateTableInfoDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var newTable = await _tableInfoService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = newTable.Id }, newTable);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUpdateTableInfoDto updateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var updatedTable = await _tableInfoService.UpdateAsync(id, updateDto);
            if (updatedTable == null) return NotFound();
            return Ok(updatedTable);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _tableInfoService.DeleteAsync(id);
            if (!success)
            {
                return BadRequest("Cannot delete table. It may have existing orders.");
            }
            return NoContent();
        }
        [AllowAnonymous]
        [HttpGet("by-token/{token}")]
        public async Task<IActionResult> GetByToken(string token)
        {
            var table = await _tableInfoService.GetByTokenAsync(token);
            if (table == null) return NotFound();
            return Ok(table);
        }
    }
}