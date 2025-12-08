using System;
using System.ComponentModel.DataAnnotations;

public class GetCoursesRequest
{
    public Guid? CategoryId { get; set; }
    public Guid? InstructorId { get; set; }
    [RegularExpression("^(en|vi|jp)$", ErrorMessage = "Language must be one of: en, vi, jp.")]
    public string? Language { get; set; }

    [RegularExpression("^(Beginner|Intermediate|Advanced)$", ErrorMessage = "Level must be one of: Beginner, Intermediate, Advanced.")]
    public string? Level { get; set; }

    [RegularExpression("^(price|rating)$", ErrorMessage = "SortBy must be one of: price, rating.")]
    public string? SortBy { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Page must be greater than 0.")]
    public int Page { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;
}
