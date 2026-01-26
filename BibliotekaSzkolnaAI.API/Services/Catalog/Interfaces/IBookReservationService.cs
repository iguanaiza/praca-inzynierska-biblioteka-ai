using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Reservation;

namespace BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces
{
    public interface IBookReservationService
    {
        Task<List<ReservationGetDto>> GetMyCartAsync(string userId);
        Task<int> GetCartCountAsync(string userId);
        Task<bool> AddToCartAsync(string userId, int bookCopyId);
        Task<bool> RemoveFromCartAsync(string userId, int cartItemId);
        Task<bool> ClearCartAsync(string userId);
        Task<bool> FinalizeCartAsync(string userId);
    }
}
