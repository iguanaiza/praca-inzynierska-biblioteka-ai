using BibliotekaSzkolnaAI.API.Models.Singles;

namespace BibliotekaSzkolnaAI.API.Models.Relations
{
    //Encja łącząca książki z tagami specjalnymi (wiele do wielu)
    public class BookBookSpecialTag
    {
        public int BookId { get; set; }
        public Book? Book { get; set; }

        public int BookSpecialTagId { get; set; }
        public BookSpecialTag? BookSpecialTag { get; set; }
    }
}
