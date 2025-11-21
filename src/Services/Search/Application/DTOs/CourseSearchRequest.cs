namespace Codemy.Search.Application.DTOs
{
    public class CourseSearchRequest
    {
        public string? Q { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Dictionary<string, object>? Filters { get; set; }
        public string? SortBy { get; set; }
        public bool SortDesc { get; set; } = true;
    }
}
