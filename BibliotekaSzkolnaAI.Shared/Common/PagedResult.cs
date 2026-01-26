using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BibliotekaSzkolnaAI.Shared.Common
{
    public class PagedResult<T>
    {
        // Lista elementów na bieżącej stronie (np. lista książek)
        [JsonPropertyName("items")]
        public List<T> Items { get; set; }

        // Całkowita liczba stron
        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        // Numer bieżącej strony
        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; }

        // Całkowita liczba wszystkich elementów w bazie danych
        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }

        // Informacja, czy istnieje poprzednia strona
        public bool HasPreviousPage => PageNumber > 1;

        // Informacja, czy istnieje następna strona
        public bool HasNextPage => PageNumber < TotalPages;

        public PagedResult()
        {
            Items = new List<T>();
        }

        public PagedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}
