using BibliotekaSzkolnaAI.API.Models.Relations;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;
using BibliotekaSzkolnaAI.Shared.Models.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaSzkolnaAI.API.Controllers.Management.Catalog
{
    [Route("api/management/books")]
    [ApiController]
    [Authorize(Roles = "Admin,Librarian")]
    public class BookManagementController(IBookManagementService managementService) : ControllerBase
    {
        // GET: api/management/books/list
        [HttpGet("list")]
        public async Task<ActionResult<PagedResult<BookGetForListDetailedDto>>> GetDetailedBooks([FromQuery] BookQueryParams query)
        {
            var result = await managementService.GetDetailedBooksAsync(query);
            return Ok(result);
        }

        // GET: api/management/books/details/{id}
        [HttpGet("details/{id}")]
        public async Task<ActionResult<BookGetDetailedDto>> GetBookByIdDetailed(int id)
        {
            var result = await managementService.GetDetailedBookByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // GET: api/management/books/lookup/{isbn}
        [HttpGet("lookup/{isbn}")]
        public async Task<ActionResult<BookLookupDto>> LookupBook(string isbn)
        {
            var result = await managementService.LookupBookByIsbnAsync(isbn);
            if (result == null) return NotFound("Nie znaleziono książki.");
            return Ok(result);
        }

        // POST: api/management/books/create
        [HttpPost("create")]
        public async Task<ActionResult> CreateBook([FromBody] BookCreateDto dto)
        {
            var newId = await managementService.CreateBookAsync(dto);

            return CreatedAtAction(nameof(GetBookByIdDetailed), new { id = newId }, new { id = newId });
        }

        // PUT: api/management/books/edit/{id}
        [HttpPut("edit/{id}")]
        public async Task<ActionResult> EditBook(int id, [FromBody] BookEditDto dto)
        {
            try
            {
                await managementService.UpdateBookAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // PUT: api/management/books/bin/{id}
        [HttpPut("bin/{id}")]
        public async Task<ActionResult> BinBook(int id)
        {
            try
            {
                await managementService.SoftDeleteBookAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        // DELETE: api/management/books/delete/{id}
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                await managementService.HardDeleteBookAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
        }

        // GET: api/management/books/lookups
        [HttpGet("lookups")]
        public async Task<ActionResult<BookLookupsDto>> GetLookups()
        {
            var result = await managementService.GetLookupsAsync();
            return Ok(result);
        }
    }
}