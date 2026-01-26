using System.ComponentModel.DataAnnotations;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog
{
    public class BookGetForHomepageDto : BookGetBaseDto
    {
        public string? ImageUrl { get; set; }
    }
}
