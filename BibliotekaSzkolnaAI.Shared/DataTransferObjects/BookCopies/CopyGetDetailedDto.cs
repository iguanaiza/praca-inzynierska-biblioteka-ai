namespace BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies
{
    public class CopyGetDetailedDto
    {
        public int Id { get; set; }
        public string Signature { get; set; } = null!;
        public int InventoryNum { get; set; }
        public bool Available { get; set; }
        public int BookId { get; set; }
        public string BookIsbn { get; set; } = null!;
        public string BookTitle { get; set; } = null!;
        public int YearOfPublication { get; set; }
        public string PublisherName { get; set; } = null!;
        public string AuthorName { get; set; } = null!;
        public bool IsDeleted { get; set; }
    }
}
