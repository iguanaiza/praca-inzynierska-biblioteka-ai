using AutoMapper;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Favorites;
using BibliotekaSzkolnaAI.Shared.Models.Params;

namespace BibliotekaSzkolnaAI.API.Services.Catalog
{
    public class FavoritesService(
        IFavoritesRepository favoritesRepo,
        IBookRepository bookRepo,
        IMapper mapper) : IFavoritesService
    {
        public async Task<PagedResult<FavoriteBookGetDto>> GetMyFavoritesAsync(string userId, FavoriteQueryParams query)
        {
            var (entities, totalCount) = await favoritesRepo.GetFavoritesAsync(userId, query);

            var dtos = mapper.Map<List<FavoriteBookGetDto>>(entities);

            return new PagedResult<FavoriteBookGetDto>(dtos, totalCount, query.PageNumber, query.PageSize);
        }

        public async Task<bool> AddToFavoritesAsync(string userId, int bookId)
        {
            if (await favoritesRepo.IsFavoriteAsync(userId, bookId))
            {
                return true;
            }

            var book = await bookRepo.GetByIdAsync(bookId);
            if (book == null)
            {
                return false;
            }

            var favorite = new FavoriteBook
            {
                UserId = userId,
                BookId = bookId,
                AddedAt = DateTime.UtcNow
            };

            await favoritesRepo.AddAsync(favorite);
            return await favoritesRepo.SaveChangesAsync();
        }

        public async Task RemoveFromFavoritesAsync(string userId, int bookId)
        {
            var favorite = await favoritesRepo.GetByBookIdAsync(userId, bookId);
            if (favorite != null)
            {
                await favoritesRepo.RemoveAsync(favorite);
                await favoritesRepo.SaveChangesAsync();
            }
        }

        public async Task<List<int>> GetMyFavoriteIdsAsync(string userId)
        {
            return await favoritesRepo.GetFavoriteBookIdsAsync(userId);
        }
    }
}
