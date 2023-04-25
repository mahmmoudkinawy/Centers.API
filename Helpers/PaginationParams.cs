namespace Centers.API.Helpers;
public class PaginationParams
{
    private const int _maxPageSize = 60;
    private int _pageSize = 40;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > _maxPageSize ? _maxPageSize : value;
    }

    private int _pageNumber = 1;
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value <= _pageNumber ? _pageNumber : value;
    }
}
