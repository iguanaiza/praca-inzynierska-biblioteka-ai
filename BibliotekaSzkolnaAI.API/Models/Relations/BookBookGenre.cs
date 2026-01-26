using BibliotekaSzkolnaAI.API.Models.Singles;

namespace BibliotekaSzkolnaAI.API.Models.Relations
{
    //Encja łącząca książki z gatunkami (wiele do wielu)
    public class BookBookGenre
    {
        public int BookId { get; set; }
        public Book Book { get; set; }

        public int BookGenreId { get; set; }
        public BookGenre BookGenre { get; set; }
    }
}
