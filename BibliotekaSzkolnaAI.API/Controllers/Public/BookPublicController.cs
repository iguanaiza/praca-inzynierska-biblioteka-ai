using BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;
using BibliotekaSzkolnaAI.Shared.Models.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BibliotekaSzkolnaAI.API.Controllers.Public
{
    [Route("api/catalog")]
    [ApiController]
    [AllowAnonymous]

    public class BookPublicController(IBookCatalogService catalogService) : ControllerBase
    {
        #region BOOKS

        // GET: api/catalog/books
        [HttpGet("books")]
        public async Task<ActionResult<PagedResult<BookGetForCatalogDto>>> GetBooks([FromQuery] BookQueryParams query)
        {
            var result = await catalogService.GetCatalogBooksAsync(query);
            return Ok(result);
        }

        // GET: api/catalog/homepage-books
        [HttpGet("homepage-books")]
        public async Task<ActionResult<Dictionary<string, List<BookGetForHomepageDto>>>> GetHomepageBooks()
        {
            var result = await catalogService.GetHomepageBooksAsync();
            return Ok(result);
        }

        // GET: api/catalog/setbooks
        [HttpGet("setbooks")]
        public async Task<ActionResult<List<BookGetForListDto>>> GetSetBooksAsync()
        {
            var result = await catalogService.GetSetBooksAsync();
            return Ok(result);
        }

        // GET: api/catalog/textbooks
        [HttpGet("textbooks")]
        public async Task<ActionResult<List<BookGetForListDto>>> GetTextBooksAsync()
        {
            var result = await catalogService.GetTextBooksAsync();
            return Ok(result);
        }

        // GET: api/catalog/details/{id}
        [HttpGet("details/{id}")]
        public async Task<ActionResult<BookGetDto>> GetBookByIDAsync(int id)
        {
            var result = await catalogService.GetBookDetailsAsync(id);

            if (result == null)
            {
                return NotFound("Książka nie została znaleziona lub jest niedostępna.");
            }

            return Ok(result);
        }

        // GET: api/catalog/filter-options
        [HttpGet("filter-options")]
        public async Task<ActionResult<FilterOptionsDto>> GetFilterOptions()
        {
            var result = await catalogService.GetFilterOptionsAsync();
            return Ok(result);
        }

        #endregion
    }
}
