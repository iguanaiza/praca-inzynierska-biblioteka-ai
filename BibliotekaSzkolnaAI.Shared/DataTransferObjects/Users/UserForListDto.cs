using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Users
{
    public class UserForListDto
    {
        public string Id { get; set; } = string.Empty;
        public int LibraryId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public decimal FineAmount { get; set; }
        public string? Class { get; set; }
        public int ActiveLoanCount { get; set; }
    }
}
