using System.ComponentModel.DataAnnotations;

namespace Codemy.Notification.Application.DTOs
{
    public class EmailInformRequestContent
    {
        [EmailAddress]
        public required string From { get; set; }
        [EmailAddress]
        public required string To { get; set; }
        public required Guid RequestId { get; set; }
        public required string RequestType { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
        public string? Response { get; set; }
        public Guid? CourseId { get; set; }
    }
}
