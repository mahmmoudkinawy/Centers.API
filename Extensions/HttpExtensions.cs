namespace Centers.API.Extensions;
public static class HttpExtensions
{
    public static void AddPaginationHeader(
        this HttpResponse response,
        int currentPage,
        int itemsPerPage,
        int totalPages,
        int totalCount)
    {
        var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalPages, totalCount);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationHeader, options));
        response.Headers.Add("Access-Control-Expose-Headers", "X-Pagination");
    }
}
