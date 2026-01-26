using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;
using BibliotekaSzkolnaAI.Shared.Models.Params;


namespace BibliotekaSzkolnaAI.API.Repositories.Interfaces
{
    public interface IBookRepository
    {
        Task<(List<Book> Items, int TotalCount)> GetBooksAsync(BookQueryParams query, bool includeHidden = false);

        Task<List<Book>> GetBooksByTagsAsync(IEnumerable<string> tags);

        Task<List<Book>> GetBooksByCategoryIdAsync(int categoryId);

        Task<Book?> GetByIdAsync(int id);

        Task<FilterOptionsDto> GetFilterOptionsAsync();

        Task AddAsync(Book book);
        void Delete(Book book);
        Task SaveChangesAsync();
        Task<Book?> GetByIdForEditAsync(int id);
        Task<BookLookupsDto> GetBookLookupsAsync();
        Task<Book?> GetBookByIsbnAsync(string isbn);
    }
}
