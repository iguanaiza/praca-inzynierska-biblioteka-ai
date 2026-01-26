using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.Shared.Models.Params;

namespace BibliotekaSzkolnaAI.API.Repositories.Interfaces
{
    public interface IFavoritesRepository
    {
        Task<(List<FavoriteBook> Items, int TotalCount)> GetFavoritesAsync(string userId, FavoriteQueryParams query);

        Task<bool> IsFavoriteAsync(string userId, int bookId);
        Task<List<int>> GetFavoriteBookIdsAsync(string userId);

        Task AddAsync(FavoriteBook favorite);
        Task RemoveAsync(FavoriteBook favorite);
        Task<FavoriteBook?> GetByBookIdAsync(string userId, int bookId);
        Task<bool> SaveChangesAsync();
    }
}
