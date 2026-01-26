using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.Reservation
{
    public class ReservationGetDto
    {
        public int Id { get; set; }
        public int BookCopyId { get; set; }
        public int BookId { get; set; }

        public string Title { get; set; }
        public string Author { get; set; }
        public string? CoverImageUrl { get; set; }

        public DateTime AddedAt { get; set; }
    }
}
