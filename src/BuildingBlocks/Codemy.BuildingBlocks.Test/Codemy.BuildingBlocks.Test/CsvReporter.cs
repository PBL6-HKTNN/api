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

        public void GenerateDetailedReport()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss"); 
            ExportToCSV($"DetailedResults_{timestamp}.csv");
        }

        public void Clear()
        {
            _results.Clear();
        }
    }
}
