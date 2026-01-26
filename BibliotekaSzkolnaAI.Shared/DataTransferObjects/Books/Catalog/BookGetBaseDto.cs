using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog
{
    public class BookGetBaseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string BookAuthor { get; set; } = null!;
        public string BookPublisher { get; set; } = null!;
        public int AvailableCopyCount { get; set; }
    }
}