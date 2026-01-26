using BibliotekaSzkolnaAI.API.Data;

namespace BibliotekaSzkolnaAI.API.Models.Singles
{
    // encja ulubionych książek użytkownika
    public class FavoriteBook
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
