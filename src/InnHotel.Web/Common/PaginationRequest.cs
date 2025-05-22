namespace InnHotel.Web.Common;

public class PaginationRequest
{
    private const int MaxPageSize = 50;
    private const int DefaultPageSize = 10;
    
    private int _pageSize = DefaultPageSize;
    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? DefaultPageSize : value;
    }
}