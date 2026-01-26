using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Users
{
    public class UserCreateDto : UserEditDto
    {
        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rola jest wymagana.")]
        public string Role { get; set; } = "User";

        [Required(ErrorMessage = "Typ użytkownika jest wymagany.")]
        public string UserType { get; set; } = "Uczeń";
    }
}

//role: Admin, Librarian, User
//userType: Uczeń, Pracownik
