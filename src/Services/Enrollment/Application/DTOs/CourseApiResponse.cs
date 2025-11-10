using Codemy.Enrollment.Application.DTOs;

public class CourseApiResponse
{
    public int Status { get; set; }
    public CourseDto Data { get; set; } = null!;
    public string? Error { get; set; }
    public bool IsSuccess { get; set; }
}
