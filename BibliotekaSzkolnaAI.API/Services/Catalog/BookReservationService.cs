using AutoMapper;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;

using BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Reservation;

namespace BibliotekaSzkolnaAI.API.Services.Catalog
{
    public class BookReservationService(
        IBookReservationRepository reservationRepo,
        IMapper mapper) : IBookReservationService
    {
        public async Task<List<ReservationGetDto>> GetMyCartAsync(string userId)
        {
            var entities = await reservationRepo.GetCartItemsEntitiesAsync(userId);
            return mapper.Map<List<ReservationGetDto>>(entities);
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            var entities = await reservationRepo.GetCartItemsEntitiesAsync(userId);
            return entities.Count;
        }

        public async Task<bool> AddToCartAsync(string userId, int bookCopyId)
        {
            if (await reservationRepo.IsCopyInCartAsync(userId, bookCopyId))
            {
                return false;
            }

            var copy = await reservationRepo.GetBookCopyAsync(bookCopyId);
            if (copy == null || !copy.Available)
            {
                return false;
            }

            var cartItem = new BookReservationCart
            {
                UserId = userId,
                BookCopyId = bookCopyId,
                IsFinalized = false,
                AddedAt = DateTime.UtcNow
            };

            await reservationRepo.AddToCartAsync(cartItem);
            return await reservationRepo.SaveChangesAsync();
        }

        public async Task<bool> RemoveFromCartAsync(string userId, int cartItemId)
        {
            var item = await reservationRepo.GetCartItemAsync(userId, cartItemId);
            if (item == null) return false;

            await reservationRepo.RemoveFromCartAsync(item);
            return await reservationRepo.SaveChangesAsync();
        }

        public async Task<bool> ClearCartAsync(string userId)
        {
            await reservationRepo.ClearCartAsync(userId);
            return true;
        }

        public async Task<bool> FinalizeCartAsync(string userId)
        {
            var cartItems = await reservationRepo.GetCartItemsEntitiesAsync(userId);
            if (!cartItems.Any()) return false;

            var newLoans = new List<BookLoan>();
            const int defaultLoanDurationDays = 30; 

            foreach (var item in cartItems)
            {
                if (!item.BookCopy.Available)
                {
                    return false;
                }

                var loan = new BookLoan
                {
                    UserId = userId,
                    BookCopyId = item.BookCopyId,
                    BorrowDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(defaultLoanDurationDays),
                    Status = LoanStatus.PendingApproval
                };
                newLoans.Add(loan);
            }
 
            return await reservationRepo.ProcessCheckoutAsync(cartItems, newLoans);
        }
    }
}
