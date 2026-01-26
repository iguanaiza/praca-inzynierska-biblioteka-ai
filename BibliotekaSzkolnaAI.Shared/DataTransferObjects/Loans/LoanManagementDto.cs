using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Loans
{
    public class LoanManagementDto : LoanGetDto
    {
        public string UserFullName { get; set; } = string.Empty;
        public int UserLibraryId { get; set; }
        public string BookSignature { get; set; } = string.Empty;
    }
}
