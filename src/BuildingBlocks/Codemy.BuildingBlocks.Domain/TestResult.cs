namespace Codemy.BuildingBlocks.Domain
{
    public class TestResult
    {
        public string TestName { get; set; }
        public string TestCategory { get; set; }
        public string Status { get; set; } // Passed, Failed, Skipped
        public DateTime ExecutionTime { get; set; }
        public double DurationMs { get; set; }
        public string ErrorMessage { get; set; }
        public string ApiEndpoint { get; set; }
        public int StatusCode { get; set; }
        public double ResponseTimeMs { get; set; }
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
    }
}
