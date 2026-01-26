using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies
{
    public abstract class CopyUpsertDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Wprowadź sygnaturę egzemplarza (18 znaków).")]
        [StringLength(18, MinimumLength = 18, ErrorMessage = "Niepoprawna sygnatura: wpisz 18 znaków")]
        public string Signature { get; set; } = null!;

        [Required(ErrorMessage = "Wprowadź numer inwentarzowy (5 cyfr)")]
        [Range(10000, 99999, ErrorMessage = "Niepoprawny numer inwentarzowy: wpisz 5 cyfr")]
        public int InventoryNum { get; set; }

        [Required(ErrorMessage = "Wprowadź ID książki.")]
        public int BookId { get; set; }
        public string BookIsbn { get; set; } = null!;
        public string BookTitle { get; set; } = null!;
        public int YearOfPublication { get; set; }
        public string PublisherName { get; set; } = null!;
        public string AuthorName { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
    }
}
