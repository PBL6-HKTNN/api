using System.ComponentModel.DataAnnotations;

namespace Codemy.Notification.Application.DTOs
{
    public class InformHideCourseRequest
    {
        [EmailAddress]
        public required string From { get; set; }
        [EmailAddress]
        public required string To { get; set; }
        public required Guid CourseId { get; set; }
        public required string Description { get; set; }
        public required string DateTime { get; set; }
        public required string CourseTitle { get; set; }
    }
}
