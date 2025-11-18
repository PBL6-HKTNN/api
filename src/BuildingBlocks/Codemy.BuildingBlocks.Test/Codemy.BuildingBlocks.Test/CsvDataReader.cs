using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace Codemy.BuildingBlocks.Test
{
    public class CsvDataReader
    {
        private readonly string _testDataFolder;

        public CsvDataReader(string testDataFolder = "TestData")
        {
            _testDataFolder = testDataFolder;

            // Tạo folder nếu chưa có
            if (!Directory.Exists(_testDataFolder))
            {
                Directory.CreateDirectory(_testDataFolder);
            }
        }

        public List<T> ReadCsv<T>(string fileName)
        {
            var filePath = Path.Combine(_testDataFolder, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CSV file not found: {filePath}");
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using (var reader = new StreamReader(filePath, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                return csv.GetRecords<T>().ToList();
            }
        }

        public IEnumerable<T> ReadCsvLazy<T>(string fileName)
        {
            var filePath = Path.Combine(_testDataFolder, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"CSV file not found: {filePath}");
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null
            };

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                foreach (var record in csv.GetRecords<T>())
                {
                    yield return record;
                }
            }
        }

        public Dictionary<string, string> ReadCsvAsDictionary(string fileName, string keyColumn)
        {
            var filePath = Path.Combine(_testDataFolder, fileName);
            var result = new Dictionary<string, string>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Read();
                csv.ReadHeader();

                while (csv.Read())
                {
                    var key = csv.GetField(keyColumn);
                    var row = string.Join("|", csv.Parser.Record);
                    result[key] = row;
                }
            }

            return result;
        }
    }
}
