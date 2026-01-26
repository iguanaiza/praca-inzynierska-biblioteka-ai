using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;

namespace BibliotekaSzkolnaAI.API.Services.Management
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;

        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var vm = await _repository.GetDashboardDataAsync();

            var culture = new System.Globalization.CultureInfo("pl-PL");

            vm.LoansPerMonth = vm.LoansPerMonth
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .Select(x => {
                    x.MonthName = culture.DateTimeFormat.GetMonthName(x.Month);
                    x.MonthName = char.ToUpper(x.MonthName[0]) + x.MonthName.Substring(1);
                    return x;
                })
                .ToList();

            return vm;
        }
    }
}
