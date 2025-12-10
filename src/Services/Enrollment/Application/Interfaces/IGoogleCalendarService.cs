

namespace Codemy.Enrollment.Application.Interfaces
{
    public interface IGoogleCalendarService
    {
        Task<CalendarResponse> AddCourseToCalendarAsync(Guid courseId);
    }

    public class CalendarResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string>? CalendarLink { get; set; }
    }
}
