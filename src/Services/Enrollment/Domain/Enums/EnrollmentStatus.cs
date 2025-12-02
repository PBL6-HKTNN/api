using System.Text.Json.Serialization;

namespace Codemy.Enrollment.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnrollmentStatus
    {
        Active,
        Completed
    }
}
