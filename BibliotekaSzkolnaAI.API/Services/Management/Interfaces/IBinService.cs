using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Bin;

namespace BibliotekaSzkolnaAI.API.Services.Management.Interfaces
{
    public interface IBinService
    {
        Task<List<DeletedItemDto>> GetAllDeletedItemsAsync();
        Task RestoreItemAsync(string id, string type);
        Task HardDeleteItemAsync(string id, string type);
    }
}
