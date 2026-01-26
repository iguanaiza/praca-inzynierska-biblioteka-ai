using System.ComponentModel.DataAnnotations;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management
{
    public class BookGetDetailedDto : BookGetDto
    {
        public bool IsDeleted { get; set; }
        public bool IsVisible { get; set; }
        public int BookAuthorId { get; set; }
        public int BookPublisherId { get; set; }
        public int BookSeriesId { get; set; }
        public int BookTypeId { get; set; }
        public int BookCategoryId { get; set; }

        public List<int> BookGenreIds { get; set; } = new();
        public List<int> BookSpecialTagIds { get; set; } = new();
    }

    public class BookLookupDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string PublisherName { get; set; }
        public int Year { get; set; }
    }
}
