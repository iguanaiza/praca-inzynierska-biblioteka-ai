using System.ComponentModel.DataAnnotations;

namespace BibliotekaSzkolnaAI.API.Models.Singles
{
    //Encja kategorii książki (lektura, podręcznik, pozostałe)
    public class BookCategory
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; }
        public string Name { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
