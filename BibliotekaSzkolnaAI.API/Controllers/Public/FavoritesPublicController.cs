
using BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Favorites;
using BibliotekaSzkolnaAI.Shared.Models.Params;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BibliotekaSzkolnaAI.API.Controllers.Public
{
    [Route("api/favorites")]
    [ApiController]
    [Authorize]

    public class FavoritesPublicController(IFavoritesService favoritesService) : ControllerBase
    {
        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        // GET: api/favorites
        [HttpGet]
        public async Task<ActionResult<PagedResult<FavoriteBookGetDto>>> GetMyFavorites([FromQuery] FavoriteQueryParams queryParams)
        {
            var userId = GetUserId();
            var result = await favoritesService.GetMyFavoritesAsync(userId, queryParams);
            return Ok(result);
        }

        // POST: api/favorites/{bookId}
        [HttpPost("{bookId:int}")]
        public async Task<IActionResult> AddFavorite(int bookId)
        {
            var userId = GetUserId();
            var success = await favoritesService.AddToFavoritesAsync(userId, bookId);

            if (!success) return BadRequest("Książka nie istnieje.");

            return Ok();
        }

        // DELETE: api/favorites/{bookId}
        [HttpDelete("{bookId:int}")]
        public async Task<IActionResult> RemoveFavorite(int bookId)
        {
            var userId = GetUserId();
            await favoritesService.RemoveFromFavoritesAsync(userId, bookId);
            return NoContent();
        }

        // GET: api/favorites/ids
        [HttpGet("ids")]
        public async Task<ActionResult<List<int>>> GetMyFavoriteIds()
        {
            var userId = GetUserId();
            var ids = await favoritesService.GetMyFavoriteIdsAsync(userId);
            return Ok(ids);
        }
    }
}
