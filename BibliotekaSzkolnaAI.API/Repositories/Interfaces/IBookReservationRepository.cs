using BibliotekaSzkolnaAI.API.Models.Singles;

namespace BibliotekaSzkolnaAI.API.Repositories.Interfaces
{
    public interface IBookReservationRepository
    {
        Task<List<BookReservationCart>> GetCartItemsEntitiesAsync(string userId);
        Task<bool> IsCopyInCartAsync(string userId, int bookCopyId);

        Task<bool> IsAnyCopyOfBookInCartAsync(string userId, int bookId);

        Task<BookCopy?> GetBookCopyAsync(int bookCopyId);

        Task<BookReservationCart?> GetCartItemAsync(string userId, int cartItemId);
        Task AddToCartAsync(BookReservationCart cartItem);
        Task RemoveFromCartAsync(BookReservationCart cartItem);
        Task ClearCartAsync(string userId);

        Task<bool> SaveChangesAsync();
        Task<bool> ProcessCheckoutAsync(List<BookReservationCart> itemsToRemove, List<BookLoan> loansToAdd);
    }
}
