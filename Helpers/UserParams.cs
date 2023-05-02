namespace Centers.API.Helpers;
public sealed class UserParams : PaginationParams
{
    public string? Keyword { get; set; }
    public string? Role { get; set; }
}
