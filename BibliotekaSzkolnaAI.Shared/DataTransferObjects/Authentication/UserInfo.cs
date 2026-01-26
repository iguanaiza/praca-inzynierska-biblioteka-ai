using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Authentication
{
    public class UserInfo
    {
        public required string UserId { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
