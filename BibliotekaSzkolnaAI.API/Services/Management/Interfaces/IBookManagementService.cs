using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;
using BibliotekaSzkolnaAI.Shared.Models.Params;

namespace BibliotekaSzkolnaAI.API.Services.Management.Interfaces
{
    public interface IBookManagementService
    {
        Task<PagedResult<BookGetForListDetailedDto>> GetDetailedBooksAsync(BookQueryParams query);
        Task<BookGetDetailedDto?> GetDetailedBookByIdAsync(int id);

        Task<List<BookGetForListDetailedDto>> GetDeletedBooksAsync();

        Task<int> CreateBookAsync(BookUpsertDto dto);
        Task UpdateBookAsync(int id, BookUpsertDto dto);

        Task SoftDeleteBookAsync(int id);

        Task HardDeleteBookAsync(int id);

        Task<BookLookupsDto> GetLookupsAsync();

        Task<BookLookupDto?> LookupBookByIsbnAsync(string isbn);
    }
}
