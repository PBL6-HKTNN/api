namespace Codemy.BuildingBlocks.Test
{
    internal class DataDrivenTestBase : BaseTest
    {
        protected CsvDataReader CsvReader;

        public DataDrivenTestBase()
        {
            CsvReader = new CsvDataReader();
        }

        // Helper để thay thế placeholders trong test data
        protected string ReplacePlaceholders(string value, Dictionary<string, string> replacements)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            foreach (var kvp in replacements)
            {
                value = value.Replace(kvp.Key, kvp.Value);
            }

            return value;
        }

        // Helper để parse Guid từ CSV
        protected Guid ParseGuidOrDefault(string value, Guid defaultValue = default)
        {
            if (Guid.TryParse(value, out Guid result))
            {
                return result;
            }
            return defaultValue;
        }
    }
}
