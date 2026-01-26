using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;

namespace BibliotekaSzkolnaAI.API.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
    }
}
