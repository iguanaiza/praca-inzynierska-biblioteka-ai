using System.ComponentModel.DataAnnotations;

namespace BibliotekaSzkolnaAI.API.Models.Singles
{
    // encja typu książki (nowela, powieść, komiks itd.)
    public class BookType
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; }
        public string Title { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
