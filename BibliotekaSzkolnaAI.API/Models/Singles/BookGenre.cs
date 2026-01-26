using BibliotekaSzkolnaAI.API.Models.Relations;
using System.ComponentModel.DataAnnotations;

namespace BibliotekaSzkolnaAI.API.Models.Singles
{
    // encja gatunku książki (np. fantasy, kryminał, romans itp.)
    public class BookGenre
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; }
        public string Title { get; set; }
        public ICollection<BookBookGenre> BookBookGenres { get; set; }
    }
}