using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries
{
    public class DashboardViewModel
    {
        public int ActiveLoansCount { get; set; }
        public int OverdueLoansCount { get; set; }
        public int PendingLoanApprovalsCount { get; set; }
        public int PendingReturnApprovalsCount { get; set; }

        public List<MonthlyLoanStatDto> LoansPerMonth { get; set; } = new();
        public List<BookGetBaseDto> RecommendedBooks { get; set; } = new();
    }

    public class MonthlyLoanStatDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
