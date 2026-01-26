using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System.Net.Http.Json;

namespace BibliotekaSzkolnaAI.Client.Adaptors
{
    public class CopyApiAdaptor : DataAdaptor
    {
        [Inject]
        public HttpClient Http { get; set; }

        public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            if (Http == null) return new DataResult() { Result = new List<CopyGetDetailedDto>(), Count = 0 };

            int pageNum = (dm.Skip != 0 && dm.Take != 0) ? (dm.Skip / dm.Take) + 1 : 1;
            int pageSize = dm.Take != 0 ? dm.Take : 10;

            var queryParams = new Dictionary<string, string?>
            {
                ["pageNumber"] = pageNum.ToString(),
                ["pageSize"] = pageSize.ToString()
            };

            if (dm.Sorted != null && dm.Sorted.Count > 0)
            {
                var sortInfo = dm.Sorted[0];

                string sortField = sortInfo.Name.ToLower() switch
                {
                    "signature" => "signature",
                    "inventorynum" => "inventoryNum",
                    "booktitle" => "bookTitle",
                    "authorname" => "authorName",
                    _ => sortInfo.Name.ToLower()
                };

                queryParams["sortBy"] = sortField;
                queryParams["sortOrder"] = sortInfo.Direction.ToString() == "Ascending" ? "asc" : "desc";
            }

            if (dm.Where != null && dm.Where.Count > 0)
            {
                foreach (var filter in dm.Where)
                {
                    ProcessFilter(filter, queryParams);
                }
            }

            var url = QueryHelpers.AddQueryString("api/management/copies/list", queryParams);
            var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            try
            {
                var result = await Http.GetFromJsonAsync<PagedResult<CopyGetDetailedDto>>(url, options);
                if (result != null)
                {
                    return new DataResult() { Result = result.Items, Count = result.TotalCount };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CopyApiAdaptor] Error: {ex.Message}");
            }

            return new DataResult() { Result = new List<CopyGetDetailedDto>(), Count = 0 };
        }

        private void ProcessFilter(WhereFilter filter, Dictionary<string, string?> queryParams)
        {
            if (filter.IsComplex || (filter.predicates != null && filter.predicates.Any()))
            {
                Console.WriteLine("[Adaptor] Odpakowuję zagnieżdżony filtr...");
                foreach (var subFilter in filter.predicates)
                {
                    ProcessFilter(subFilter, queryParams);
                }
                return;
            }

            if (!string.IsNullOrEmpty(filter.Field))
            {
                var keyParam = MapFieldToApi(filter.Field);
                var val = filter.value?.ToString();

                Console.WriteLine($"[Adaptor] Znaleziono wartość: Field={filter.Field} (API: {keyParam}), Value={val}");

                if (!string.IsNullOrWhiteSpace(val))
                {
                    queryParams[keyParam] = val;
                }
            }
        }
        private string MapFieldToApi(string fieldName)
        {
            return fieldName.ToLower() switch
            {
                "signature" => "signature",
                "inventorynum" => "inventoryNum",
                "booktitle" => "bookTitle",
                "authorname" => "authorName",
                _ => fieldName.ToLower()
            };
        }
    }
}
