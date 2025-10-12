namespace Codemy.BuildingBlocks.Core.Models
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public T? Data { get; set; }
        public ErrorResponse? Error { get; set; }
        public bool IsSuccess => Error == null;

        // Static factory methods for creating responses
        public static ApiResponse<T> Success(T data, int status = 200)
        {
            return new ApiResponse<T>
            {
                Status = status,
                Data = data,
                Error = null
            };
        }

        public static ApiResponse<T> Failure(string message, int status = 400, string? details = null)
        {
            return new ApiResponse<T>
            {
                Status = status,
                Data = default,
                Error = new ErrorResponse
                {
                    Message = message,
                    Details = details
                }
            };
        }
    }

    public class ErrorResponse
    {
        public required string Message { get; set; }
        public string? Details { get; set; }
        public Dictionary<string, string[]>? ValidationErrors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
