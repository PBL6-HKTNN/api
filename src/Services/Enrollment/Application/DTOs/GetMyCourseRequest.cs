using Codemy.Enrollment.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Codemy.Enrollment.Application.DTOs
{

    [JsonConverter(typeof(JsonStringEnumConverter))] // giúp Swagger hiển thị enum dạng string
    public enum SortByOption
    {
        Date
    }
    
    public class GetMyCourseRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1.")]
        public int Page { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;

        [RegularExpression("^(NotStarted|InProgress|Completed)$", ErrorMessage = "ProgressStatus must be NotStarted, InProgress, or Completed.")]
        public ProgressStatus? ProgressStatus { get; set; }

        public EnrollmentStatus? EnrollmentStatus { get; set; }
        public SortByOption? SortBy { get; set; } = SortByOption.Date;
        public bool SortDescending { get; set; } = true;
    }
}
