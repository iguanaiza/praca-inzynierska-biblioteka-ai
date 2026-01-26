using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog
{
    public class BookGetForListDto : BookGetBaseDto
    {
        public string? Subject { get; set; }
        public string? Class { get; set; }
    }
}
