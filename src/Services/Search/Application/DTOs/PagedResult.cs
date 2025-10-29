namespace Codemy.Search.Application.DTOs
{
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public long Total { get; set; }
        public List<T> Items { get; set; } = new();
    }
}
