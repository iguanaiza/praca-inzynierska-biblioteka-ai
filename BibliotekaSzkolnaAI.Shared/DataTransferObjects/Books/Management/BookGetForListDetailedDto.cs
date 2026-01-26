using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management
{
    public class BookGetForListDetailedDto : BookGetBaseDto
    {
        public string? BookSeries { get; set; }
        public int Year { get; set; }
        public string Isbn { get; set; } = null!;
        public int CopyCount { get; set; }
        public bool IsVisible { get; set; }
    }
}
