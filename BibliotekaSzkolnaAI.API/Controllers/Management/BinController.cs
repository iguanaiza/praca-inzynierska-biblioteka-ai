using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Bin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.API.Controllers.Management
{
    [Route("api/management/bin")]
    [ApiController]
    [Authorize(Roles = "Admin,Librarian")]
    public class BinController(IBinService binService) : ControllerBase
    {
        // GET /api/management/bin
        [HttpGet]
        public async Task<ActionResult<List<DeletedItemDto>>> GetBin()
        {
            var result = await binService.GetAllDeletedItemsAsync();
            return Ok(result);
        }

        // POST /api/management/bin/restore
        [HttpPost("restore")]
        public async Task<IActionResult> Restore([FromBody] RestoreItemDto dto)
        {
            try
            {
                await binService.RestoreItemAsync(dto.Id, dto.Type);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST /api/management/bin/hard-delete
        [HttpPost("hard-delete")]
        public async Task<IActionResult> HardDelete([FromBody] RestoreItemDto dto)
        {
            try
            {
                await binService.HardDeleteItemAsync(dto.Id, dto.Type);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Nie można usunąć elementu (prawdopodobnie posiada powiązane dane historyczne)." });
            }
        }
    }
}
