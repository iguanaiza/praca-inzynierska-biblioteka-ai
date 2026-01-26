namespace BibliotekaSzkolnaAI.Shared.Models.Params
{
    public class BookQueryParams : BaseQueryParams
    {
        public string? SearchTerm { get; set; }
        public string? Title { get; set; }
        public string? Isbn { get; set; }
        public string? BookAuthor { get; set; }

        public int? Year { get; set; }
        public bool? IsVisible { get; set; }

        public List<string> BookAuthors { get; set; } = new();  
        public List<string> BookPublishers { get; set; } = new();
        public List<string> BookSeries { get; set; } = new();
        public List<string> BookCategories { get; set; } = new(); 
        public List<string> BookTypes { get; set; } = new();  

        public List<string> BookGenres { get; set; } = new();
        public List<string> BookSpecialTags { get; set; } = new();
    }
}
