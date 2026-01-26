namespace BibliotekaSzkolnaAI.Shared.Models.Params
{
    public class CopyQueryParams : BaseQueryParams
    {
        public string? Signature { get; set; }
        public int? InventoryNum { get; set; }
        public bool? Available { get; set; }

        public string? BookTitle { get; set; }
        public string? AuthorName { get; set; }
    }
}
