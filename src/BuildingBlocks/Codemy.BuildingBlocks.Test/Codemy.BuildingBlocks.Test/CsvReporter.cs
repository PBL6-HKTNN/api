using Codemy.BuildingBlocks.Domain;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace Codemy.BuildingBlocks.Test
{
    public class CsvReporter
    {
        private readonly string _outputPath;
        private readonly List<TestResult> _results;

        public CsvReporter(string outputPath = "TestResults")
        {
            _outputPath = outputPath;
            _results = new List<TestResult>();

            // Tạo thư mục nếu chưa có
            if (!Directory.Exists(_outputPath))
            {
                Directory.CreateDirectory(_outputPath);
            }
        }

        public void AddResult(TestResult result)
        {
            _results.Add(result);
        }

        public void ExportToCSV(string fileName = null)
        {
            if (fileName == null)
            {
                fileName = $"TestResults_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            }

            var filePath = Path.Combine(_outputPath, fileName);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                Encoding = Encoding.UTF8
            };

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                csv.WriteRecords(_results);
            }

            Console.WriteLine($"Test results exported to: {filePath}");
        }

        public void ExportSummaryToCSV(string fileName = null)
        {
            if (fileName == null)
            {
                fileName = $"TestSummary_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            }

            var filePath = Path.Combine(_outputPath, fileName);

            var summary = new
            {
                TotalTests = _results.Count,
                Passed = _results.Count(r => r.Status == "Passed"),
                Failed = _results.Count(r => r.Status == "Failed"),
                Skipped = _results.Count(r => r.Status == "Skipped"),
                PassRate = _results.Count > 0
                    ? Math.Round((_results.Count(r => r.Status == "Passed") * 100.0 / _results.Count), 2)
                    : 0,
                TotalDurationMs = _results.Sum(r => r.DurationMs),
                AverageResponseTimeMs = _results.Count > 0
                    ? Math.Round(_results.Average(r => r.ResponseTimeMs), 2)
                    : 0,
                ExecutionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(new[] { summary });
            }

            Console.WriteLine($"Test summary exported to: {filePath}");
        }

        public void GenerateDetailedReport()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            // Export chi tiết tất cả tests
            ExportToCSV($"DetailedResults_{timestamp}.csv");

            // Export summary
            //ExportSummaryToCSV($"Summary_{timestamp}.csv");

            // Export theo category
            //ExportByCategoryToCSV($"ResultsByCategory_{timestamp}.csv");
        }

        private void ExportByCategoryToCSV(string fileName)
        {
            var filePath = Path.Combine(_outputPath, fileName);

            var categoryResults = _results
                .GroupBy(r => r.TestCategory)
                .Select(g => new
                {
                    Category = g.Key,
                    TotalTests = g.Count(),
                    Passed = g.Count(r => r.Status == "Passed"),
                    Failed = g.Count(r => r.Status == "Failed"),
                    PassRate = Math.Round((g.Count(r => r.Status == "Passed") * 100.0 / g.Count()), 2),
                    AvgResponseTime = Math.Round(g.Average(r => r.ResponseTimeMs), 2)
                });

            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(categoryResults);
            }

            Console.WriteLine($"Results by category exported to: {filePath}");
        }

        public void Clear()
        {
            _results.Clear();
        }
    }
}
