using BibliotekaSzkolnaAI.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Loans
{
    public class LoanGetDto
    {
        public int Id { get; set; }
        public int BookCopyId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string? BookPublisher { get; set; }
        public int? BookPublicationYear { get; set; }
        public LoanStatus Status { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal PenaltyAmount { get; set; }
        public bool IsReturned { get; set; }
        public bool WasProlonged { get; set; }
    }
}
