using System.ComponentModel.DataAnnotations;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog
{
    public class BookGetDto : BookGetForCatalogDto
    {
        public string Isbn { get; set; } = null!;
        public int PageCount { get; set; }
        public List<CopyGetDetailedDto>? BookCopies { get; set; }
        public int CopyCount { get; set; }
    }
}