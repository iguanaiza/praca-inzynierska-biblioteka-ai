using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog
{
    public class BookGetForCatalogDto : BookGetForListDto
    {
        public int Year { get; set; }
        public string Description { get; set; } = null!;
        public string? ImageUrl { get; set; }

        public string? BookSeries { get; set; }

        public string BookCategory { get; set; } = null!;

        public string BookType { get; set; } = null!;

        public List<string> BookGenres { get; set; } = new();

        public List<string>? BookSpecialTags { get; set; }

        public bool IsFavorite { get; set; }
        public int FavoriteCount { get; set; }
    }
}
