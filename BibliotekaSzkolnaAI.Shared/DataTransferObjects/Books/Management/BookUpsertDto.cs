using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management
{
    public abstract class BookUpsertDto
    {
        [Required(ErrorMessage = "Wprowadź tytuł książki (maksymalnie 100 znaków).")]
        [StringLength(100, ErrorMessage = "Niepoprawny tytuł: wpisz maksymalnie 100 znaków")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wprowadź rok wydania książki (zakres 1900-2200.")]
        [Range(1900, 2200, ErrorMessage = "Niepoprawny rok: pisz cyfry z zakresu 1900-2200.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Wprowadź krótki opis książki (maksymalnie 500 znaków).")]
        [StringLength(500, ErrorMessage = "Niepoprawny opis: wpisz maksymalnie 500 znaków")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wprowadź numer ISBN książki (13 cyfr w formacie 978-83-XXXX-XXX-X).")]
        [StringLength(17, ErrorMessage = "Niepoprawny ISBN: wprowadź 13 cyfr w formacie 978-83-XXXX-XXX-X.")]
        public string Isbn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Wprowadź ilość stron w książce (zakres 1-1500).")]
        [Range(1, 1500, ErrorMessage = "Niepoprawna ilość stron: wpisz cyfry z zakresu 1-1500.")]
        public int PageCount { get; set; }

        public string? ImageUrl { get; set; }

        public string? Subject { get; set; }

        public string? Class { get; set; }
        public bool IsVisible { get; set; } = true;

        [Required(ErrorMessage = "Wprowadź ID autora książki.")]
        public int BookAuthorId { get; set; }

        [Required(ErrorMessage = "Wprowadź ID wydawcy książki.")]
        public int BookPublisherId { get; set; }

        [Required(ErrorMessage = "Wprowadź ID serii, do której należy książka.")]
        public int BookSeriesId { get; set; }

        [Required(ErrorMessage = "Wprowadź ID rodzajów książki.")]
        public int BookTypeId { get; set; }

        [Required(ErrorMessage = "Wprowadź ID kategorii książki.")]
        public int BookCategoryId { get; set; }

        [Required(ErrorMessage = "Wprowadź ID gatunków książki.")]
        public List<int> BookGenreIds { get; set; } = new List<int>();

        public List<int>? BookSpecialTagIds { get; set; }
    }
}
