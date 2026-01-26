using System.ComponentModel.DataAnnotations;

namespace BibliotekaSzkolnaAI.API.Models.Singles
{
    // encja serii książek (np. "Harry Potter", "Opowieści z Narnii")
    public class BookSeries
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; }
        public string Title { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
