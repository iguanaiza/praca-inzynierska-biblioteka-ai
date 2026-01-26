using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;
using BibliotekaSzkolnaAI.Shared.Models.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.API.Controllers.Management
{
    [Route("api/management/copies")]
    [ApiController]
    [Authorize(Roles = "Admin,Librarian")]
    public class CopyManagementController(ICopyManagementService copyService) : ControllerBase
    {
        // GET: api/management/copies/list
        [HttpGet("list")]
        public async Task<ActionResult<PagedResult<CopyGetDetailedDto>>> GetCopies([FromQuery] CopyQueryParams query)
        {
            var result = await copyService.GetCopiesAsync(query);
            return Ok(result);
        }

        // GET: api/management/copies/details/{id}
        [HttpGet("details/{id}")]
        public async Task<ActionResult<CopyGetDetailedDto>> GetCopyById(int id)
        {
            var result = await copyService.GetCopyByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET: api/management/copies/next-inventory-number
        [HttpGet("next-inventory-number")]
        public async Task<ActionResult<int>> GetNextInventoryNum()
        {
            var number = await copyService.GetNextInventoryNumberAsync();
            return Ok(number);
        }

        // POST: api/management/copies
        [HttpPost("create")]
        public async Task<ActionResult<CopyGetDetailedDto>> CreateCopy([FromBody] CopyCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var createdCopy = await copyService.CreateCopyAsync(dto);
                return CreatedAtAction(nameof(GetCopyById), new { id = createdCopy.Id }, createdCopy);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/management/copies/edit/{id}
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> UpdateCopy(int id, [FromBody] CopyEditDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                await copyService.UpdateCopyAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // PUT: api/management/copies/bin/{id} (Soft Delete)
        [HttpPut("bin/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                await copyService.SoftDeleteCopyAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        // DELETE: api/management/copies/{id} (Hard Delete)
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> HardDelete(int id)
        {
            try
            {
                await copyService.HardDeleteCopyAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { return Conflict(new { message = ex.Message }); }
        }
    }
}
