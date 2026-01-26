using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BibliotekaSzkolnaAI.API.Models.Singles
{
    // encja egzemplarza książki
    public class BookCopy
    {
        public int Id { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime? Modified { get; set; }

        [Required]
        public string Signature { get; set; } = null!; 

        public int InventoryNum { get; set; } 
        public bool Available { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public int BookId { get; set; } 

        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }

        public ICollection<BookReservationCart> ReservationCart { get; set; }

        public ICollection<BookLoan> BookLoans { get; set; }
    }
}

/* Przyjęty schemat sygnatury:
 * Rodzaj (LP – literatura piękna, PR - podręczniki),
 * Kategoria główna (np. FAN fantastyka, GEO geografia)
 * Inicjały autora (nazwisko, imie),
 * Tytuł (pierwsze litery słów tytułowych lub pierwsze 2 litery tytułu)
 * Kolejność w serii lub numerze tomu (opcjonalnie), dla PR klasa
 * Numer kopii
 *
 * Przykład: LP.FAN.RJ.HP.1-001
 */
