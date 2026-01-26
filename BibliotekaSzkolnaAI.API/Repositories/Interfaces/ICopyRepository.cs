using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.Shared.Models.Params;

namespace BibliotekaSzkolnaAI.API.Repositories.Interfaces
{
    public interface ICopyRepository
    {
        Task<(List<BookCopy> Items, int TotalCount)> GetCopiesAsync(CopyQueryParams query, bool includeDeleted);

        Task<BookCopy?> GetByIdAsync(int id);

        Task<bool> ExistsAsync(string signature, int inventoryNum, int? excludeId = null);

        Task AddAsync(BookCopy copy);
        void Delete(BookCopy copy);
        Task<bool> SaveChangesAsync();
        Task<int> GetMaxInventoryNumberAsync();
    }
}
