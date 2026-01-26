using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Loans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Users
{
    public class UserDetailedDto : UserForListDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Pesel { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public List<string> Roles { get; set; } = new();
        public List<LoanGetDto> Loans { get; set; } = new();
    }
}
