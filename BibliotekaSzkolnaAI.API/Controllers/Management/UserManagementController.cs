using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.API.Controllers.Management
{
    [Route("api/management/users")]
    [ApiController]
    [Authorize(Roles = "Admin,Librarian")]
    public class UserManagementController(IUserManagementService userService) : ControllerBase
    {
        // GET /api/management/users/list
        [HttpGet("list")]
        public async Task<ActionResult<PagedResult<UserForListDto>>> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await userService.GetUsersAsync(page, pageSize, search);
            return Ok(result);
        }

        // GET /api/management/users/details/{id}
        [HttpGet("details/{id}")]
        public async Task<ActionResult<UserDetailedDto>> GetUser(string id)
        {
            var result = await userService.GetUserDetailsAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST /api/management/users/create
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newUserId = await userService.CreateUserAsync(dto);

                return CreatedAtAction(nameof(GetUser), new { id = newUserId }, new { id = newUserId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Wystąpił błąd serwera podczas tworzenia użytkownika.");
            }
        }

        // PUT /api/management/users/edit/{id}
        [HttpPut("edit/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserEditDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await userService.UpdateUserAsync(id, dto);
                return NoContent(); 
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Nie znaleziono użytkownika.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Wystąpił błąd serwera podczas edycji.");
            }
        }

        // PUT /api/management/users/bin/{id}
        [HttpPut("bin/{id}")]
        public async Task<IActionResult> SoftDeleteUser(string id)
        {
            try
            {
                await userService.SoftDeleteUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Nie znaleziono użytkownika.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Błąd serwera przy usuwaniu.");
            }
        }

        // DELETE /api/management/users/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> HardDeleteUser(string id)
        {
            try
            {
                await userService.HardDeleteUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Nie znaleziono użytkownika.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Błąd serwera przy usuwaniu.");
            }
        }
    }
}
