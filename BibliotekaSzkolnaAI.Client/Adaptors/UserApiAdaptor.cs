using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System.Net.Http.Json;

namespace BibliotekaSzkolnaAI.Client.Adaptors
{
    public class UserApiAdaptor : DataAdaptor
    {
        [Inject]
        public HttpClient Http { get; set; }

        public override async Task<object> ReadAsync(DataManagerRequest dm, string key = null)
        {
            if (Http == null) return new DataResult() { Result = new List<UserForListDto>(), Count = 0 };

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

                queryParams["sortBy"] = MapFieldToApi(sortInfo.Name);

                var direction = sortInfo.Direction?.ToString() ?? "Ascending";
                bool isDescending = direction.StartsWith("Desc", StringComparison.OrdinalIgnoreCase);
                queryParams["sortOrder"] = isDescending ? "desc" : "asc";
            }

            if (dm.Where != null && dm.Where.Count > 0)
            {
                foreach (var filter in dm.Where)
                {
                    ProcessFilter(filter, queryParams);
                }
            }

            var url = QueryHelpers.AddQueryString("api/management/users/list", queryParams);

            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var result = await Http.GetFromJsonAsync<PagedResult<UserForListDto>>(url, options);

                if (result != null)
                {
                    return new DataResult()
                    {
                        Result = result.Items,
                        Count = result.TotalCount
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserAdaptor Error]: {ex.Message}");
            }

            return new DataResult() { Result = new List<UserForListDto>(), Count = 0 };
        }

        private void ProcessFilter(WhereFilter filter, Dictionary<string, string?> queryParams)
        {
            if (filter.IsComplex || (filter.predicates != null && filter.predicates.Any()))
            {
                foreach (var subFilter in filter.predicates)
                {
                    ProcessFilter(subFilter, queryParams);
                }
                return;
            }

            if (!string.IsNullOrEmpty(filter.Field))
            {
                queryParams["search"] = filter.value?.ToString();
            }
        }

        private string MapFieldToApi(string fieldName)
        {
            return fieldName.ToLower() switch
            {
                "fullname" => "lastname",
                "email" => "email",
                "libraryid" => "libraryid",
                "usertype" => "usertype",
                "class" => "class",
                _ => fieldName.ToLower()
            };
        }
    }
}
