using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;

namespace BibliotekaSzkolnaAI.API.Services.Management.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync();
    }
}
