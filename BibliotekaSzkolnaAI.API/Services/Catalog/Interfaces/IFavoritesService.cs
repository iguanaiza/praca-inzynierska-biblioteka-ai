using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Favorites;
using BibliotekaSzkolnaAI.Shared.Models.Params;

namespace BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces
{
    public interface IFavoritesService
    {
        Task<PagedResult<FavoriteBookGetDto>> GetMyFavoritesAsync(string userId, FavoriteQueryParams query);
        Task<bool> AddToFavoritesAsync(string userId, int bookId);
        Task RemoveFromFavoritesAsync(string userId, int bookId);
        Task<List<int>> GetMyFavoriteIdsAsync(string userId);
    }
}
