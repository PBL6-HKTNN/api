using Codemy.BuildingBlocks.Domain;
namespace Codemy.BuildingBlocks.Test
{
    public class TestRunner : BaseTest
    {
        [Fact]
        public void RunAllTestsAndGenerateReport()
        {
            Console.WriteLine("\n╔══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║      AUTOMATION TEST EXECUTION COMPLETED                 ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════╝\n");

            // Generate all reports
            Reporter.GenerateDetailedReport();

            Console.WriteLine("\n═══════════════════════════════════════════════════════════");
            Console.WriteLine("                    TEST SUMMARY");
            Console.WriteLine("═══════════════════════════════════════════════════════════\n");

            // Print summary to console
            var summary = GetTestSummary();
            Console.WriteLine($"Total Tests:     {summary.Total}");
            Console.WriteLine($"Passed:          {summary.Passed} ({summary.PassRate:F2}%)");
            Console.WriteLine($"Failed:          {summary.Failed}");
            Console.WriteLine($"Total Duration:  {summary.TotalDuration:F2}ms");
            Console.WriteLine($"Avg Response:    {summary.AvgResponse:F2}ms");
            Console.WriteLine();
            Console.WriteLine("Reports generated in TestResults/ folder:");
            Console.WriteLine($"  - DetailedResults_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            Console.WriteLine($"  - Summary_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            Console.WriteLine($"  - ResultsByCategory_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
            Console.WriteLine("\n═══════════════════════════════════════════════════════════\n");
        }

        private (int Total, int Passed, int Failed, double PassRate, double TotalDuration, double AvgResponse) GetTestSummary()
        {
            // Access the static Reporter from BaseTest
            var results = typeof(CsvReporter)
                .GetField("_results", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(Reporter) as List<Codemy.BuildingBlocks.Domain.TestResult>;

            if (results == null || results.Count == 0)
            {
                return (0, 0, 0, 0, 0, 0);
            }

            var total = results.Count;
            var passed = results.Count(r => r.Status == "Passed");
            var failed = results.Count(r => r.Status == "Failed");
            var passRate = total > 0 ? (passed * 100.0 / total) : 0;
            var totalDuration = results.Sum(r => r.DurationMs);
            var avgResponse = results.Count > 0 ? results.Average(r => r.ResponseTimeMs) : 0;

            return (total, passed, failed, passRate, totalDuration, avgResponse);
        }
    }
}
