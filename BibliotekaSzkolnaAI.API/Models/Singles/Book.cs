using BibliotekaSzkolnaAI.API.Models.Relations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotekaSzkolnaAI.API.Models.Singles
{
    public class Book
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; }
        public string Title { get; set; } = null!;
        public int Year { get; set; }
        public string Description { get; set; } = null!; // krótki opis ksiazki do 500 znaków
        public string Isbn { get; set; } = null!; // numer ISBN
        public int PageCount { get; set; } // ilość stron
        public bool IsDeleted { get; set; } = false; // do soft delete
        public bool IsVisible { get; set; } = true; // do widoku w katalogu
        public string? ImageUrl { get; set; } // okładka książki - URL do folderu
        public string? Subject { get; set; } // przedmiot (dla podrecznikow)
        public string? Class { get; set; } // klasa (dla lektur)

        #region Odwołania do innych
        public int BookAuthorId { get; set; }
        [ForeignKey(nameof(BookAuthorId))]
        public BookAuthor BookAuthor { get; set; } = null!; // autor

        public int BookPublisherId { get; set; }
        [ForeignKey(nameof(BookPublisherId))]
        public BookPublisher BookPublisher { get; set; } = null!; // wydawca

        public int BookSeriesId { get; set; }
        [ForeignKey(nameof(BookSeriesId))]
        public BookSeries BookSeries { get; set; } = null!; // seria np. Harry Potter

        public int BookTypeId { get; set; }
        [ForeignKey(nameof(BookTypeId))]
        public BookType BookType { get; set; } = null!; // typ ksiazki, rodzaj np. powieść

        public int BookCategoryId { get; set; }
        [ForeignKey(nameof(BookCategoryId))]
        public BookCategory BookCategory { get; set; } = null!; // jedno z trzech: lektura, podręcznik, pozostałe

        public ICollection<BookBookGenre> BookBookGenres { get; set; } = new List<BookBookGenre>(); // gatunek ksiazki np. fantasy - moze byc wiele

        public ICollection<BookBookSpecialTag> BookBookSpecialTags { get; set; } = new List<BookBookSpecialTag>(); // special tag

        public ICollection<BookCopy> BookCopies { get; set; } = new List<BookCopy>(); // lista kopii
        public int CopyCount => BookCopies?.Count ?? 0; // liczba kopii
        public int AvailableCopyCount => BookCopies?.Count(c => c.Available) ?? 0; // liczba dostępnych
        public ICollection<FavoriteBook> FavoriteByUsers { get; set; } // ulubione przez
        public int FavoriteCount => FavoriteByUsers?.Count ?? 0;
        #endregion
    }
}