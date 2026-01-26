using BibliotekaSzkolnaAI.API.Models.Relations;

namespace BibliotekaSzkolnaAI.API.Models.Singles
{
    // encja tagów książek (np. "Polecane", "Nowość" itd.)
    public class BookSpecialTag
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; }
        public string Title { get; set; }
        public ICollection<BookBookSpecialTag>? BookBookSpecialTags { get; set; }
    }
}
