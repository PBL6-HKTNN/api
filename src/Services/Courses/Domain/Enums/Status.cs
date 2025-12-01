using System.Text.Json.Serialization;

namespace Codemy.Courses.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Status
    {
        Draft,
        Published,
        Archived
    }

    public enum QuizAttemptStatus
    {
        InProgress,
        Completed,
        Failed
    }
}
