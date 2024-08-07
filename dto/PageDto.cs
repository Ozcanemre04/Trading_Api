

namespace trading_app.dto
{
    public class PageDto<T>
    {
       public int PageNumber { get; init; }
       public int PageSize { get; init; }
       public int TotalPages { get; init; }
       public int TotalRecords { get; init; }
       public IEnumerable<T> Data { get; init; }
    }
}