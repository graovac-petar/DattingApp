using AppDating.API.Helpers;
using Newtonsoft.Json;

namespace AppDating.API.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
        {
            var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);

            // Serialize the object using Newtonsoft.Json
            var serializedPaginationHeader = JsonConvert.SerializeObject(paginationHeader, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });

            // Add the serialized pagination header to the response headers
            response.Headers.Append("Pagination", serializedPaginationHeader);
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
