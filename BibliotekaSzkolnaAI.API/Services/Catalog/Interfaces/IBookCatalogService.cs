using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;
using BibliotekaSzkolnaAI.Shared.Models.Params;

namespace BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces
{
    public interface IBookCatalogService
    {
        Task<PagedResult<BookGetForCatalogDto>> GetCatalogBooksAsync(BookQueryParams query);

        Task<Dictionary<string, List<BookGetForHomepageDto>>> GetHomepageBooksAsync();

        Task<List<BookGetForListDto>> GetSetBooksAsync();

        Task<List<BookGetForListDto>> GetTextBooksAsync();

        Task<BookGetDto?> GetBookDetailsAsync(int id);

        Task<FilterOptionsDto> GetFilterOptionsAsync();
    }
}
