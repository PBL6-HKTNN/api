using System.Text.Json.Serialization;

namespace Codemy.Courses.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnrollmentStatus
    {
        Active,
        Completed,
        Cancelled
    }
}
