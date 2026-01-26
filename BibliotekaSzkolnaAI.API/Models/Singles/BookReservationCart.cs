using BibliotekaSzkolnaAI.API.Data;

namespace BibliotekaSzkolnaAI.API.Models.Singles
{
    // encja koszyka rezerwacji
    public class BookReservationCart
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int BookCopyId { get; set; }
        public BookCopy BookCopy { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public bool IsFinalized { get; set; } = false;
    }
}
