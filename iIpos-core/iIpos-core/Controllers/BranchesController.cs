using iIpos_core.Dto.Branch;
using iIpos_core.Service.Branch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ilpos_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class BranchesController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchesController(IBranchService branchService)
        {
            _branchService = branchService;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? storeId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 9)
        {
            var branches = await _branchService.GetAllByStoreIdAsync(storeId, pageNumber, pageSize);
            return Ok(branches);
        }

        [AllowAnonymous]
        [HttpGet("{branchId}")]
        public async Task<IActionResult> GetById(int branchId)
        {
            var branch = await _branchService.GetByIdAsync(branchId);
            if (branch == null) return NotFound();
            return Ok(branch);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUpdateBranchDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newBranch = await _branchService.CreateAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { branchId = newBranch.Id }, newBranch);
        }

        [HttpPut("{branchId}")]
        public async Task<IActionResult> Update(int branchId, [FromBody] CreateUpdateBranchDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBranch = await _branchService.UpdateAsync(branchId, updateDto);
            if (updatedBranch == null) return NotFound();
            return Ok(updatedBranch);
        }

        [HttpDelete("{branchId}")]
        public async Task<IActionResult> Delete(int branchId)
        {
            var success = await _branchService.DeleteAsync(branchId);
            if (!success) return BadRequest("Cannot delete branch. It may have related tables or orders.");
            return NoContent();
        }
    }
}