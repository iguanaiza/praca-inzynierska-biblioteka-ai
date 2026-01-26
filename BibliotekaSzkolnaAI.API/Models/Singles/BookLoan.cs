using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaSzkolnaAI.API.Models.Singles
{
    // encja wypożyczenia książki
    public class BookLoan
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int BookCopyId { get; set; }
        public BookCopy BookCopy { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        [Precision(4, 2)]
        public decimal PenaltyAmount { get; set; }

        public bool WasProlonged { get; set; } = false;
        public LoanStatus Status { get; set; } = LoanStatus.PendingApproval;

        public bool IsReturned => ReturnDate != null;
    }
}
