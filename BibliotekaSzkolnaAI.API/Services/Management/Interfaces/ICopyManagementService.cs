using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;
using BibliotekaSzkolnaAI.Shared.Models.Params;

namespace BibliotekaSzkolnaAI.API.Services.Management.Interfaces
{
    public interface ICopyManagementService
    {
        Task<PagedResult<CopyGetDetailedDto>> GetCopiesAsync(CopyQueryParams query);

        Task<List<CopyGetDetailedDto>> GetDeletedCopiesAsync();

        Task<CopyGetDetailedDto?> GetCopyByIdAsync(int id);

        Task<CopyGetDetailedDto> CreateCopyAsync(CopyCreateDto dto);
        Task UpdateCopyAsync(int id, CopyEditDto dto);
        Task SoftDeleteCopyAsync(int id);
        Task HardDeleteCopyAsync(int id);
        Task<int> GetNextInventoryNumberAsync();
    }
}
