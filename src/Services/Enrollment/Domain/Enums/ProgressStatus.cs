using System.Text.Json.Serialization;

namespace Codemy.Enrollment.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProgressStatus
    {
        NotStarted,
        InProgress,
        Completed
    }
}
