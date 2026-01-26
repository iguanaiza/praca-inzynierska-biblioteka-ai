
using BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BibliotekaSzkolnaAI.API.Controllers.Public
{
    [Route("api/bookreservation")]
    [ApiController]
    [Authorize]
    public class ReservationPublicController(IBookReservationService reservationService) : ControllerBase
    {
        private string GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

        // GET: api/bookreservation/cart
        [HttpGet("cart")]
        public async Task<ActionResult<List<ReservationGetDto>>> GetMyCart()
        {
            var userId = GetUserId();
            var result = await reservationService.GetMyCartAsync(userId);
            return Ok(result);
        }

        // GET: api/bookreservation/count
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetCartCount()
        {
            var userId = GetUserId();
            var count = await reservationService.GetCartCountAsync(userId);
            return Ok(count);
        }

        // POST: api/bookreservation/add/{bookCopyId}
        [HttpPost("add/{bookCopyId}")]
        public async Task<IActionResult> AddToCart(int bookCopyId)
        {
            var userId = GetUserId();
            var success = await reservationService.AddToCartAsync(userId, bookCopyId);

            if (!success)
                return BadRequest("Nie można dodać książki (niedostępna lub już w koszyku).");

            return Ok(new { message = "Dodano do koszyka" });
        }

        // POST: api/bookreservation/finalize
        [HttpPost("finalize")]
        public async Task<IActionResult> FinalizeCart()
        {
            var userId = GetUserId();
            var success = await reservationService.FinalizeCartAsync(userId);

            if (!success)
                return BadRequest("Nie udało się sfinalizować rezerwacji (koszyk pusty lub książki niedostępne).");

            return Ok(new { message = "Rezerwacja zakończona sukcesem" });
        }

        // DELETE: api/bookreservation/remove/{cartItemId}
        [HttpDelete("remove/{cartItemId}")]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = GetUserId();
            var success = await reservationService.RemoveFromCartAsync(userId, cartItemId);

            if (!success) return NotFound();

            return NoContent();
        }

        // DELETE: api/bookreservation/clear
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserId();
            await reservationService.ClearCartAsync(userId);
            return NoContent();
        }
    }
}
