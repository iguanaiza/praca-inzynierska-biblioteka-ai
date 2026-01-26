using BibliotekaSzkolnaAI.API.Models.Singles;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace BibliotekaSzkolnaAI.API.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int LibraryId { get; set; } //karta biblioteczna, 5 cyfr np 12345

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public string UserType { get; set; } = null!; // Uczeń, Pracownik

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "PESEL musi składać się z 11 cyfr.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "PESEL musi zawierać same cyfry.")]
        public string Pesel { get; set; } = null!;

        public string? Class { get; set; }

        public string? ProfilePictureUrl { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal FineAmount { get; set; } = 0;

        public DateTime DateAdded { get; set; } = DateTime.UtcNow; 

        public DateTime? DateModified { get; set; } 

        public ICollection<BookLoan>? BookLoans { get; set; }

        public ICollection<FavoriteBook>? FavoriteBooks { get; set; }

        public ICollection<BookReservationCart>? ReservationCart { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
