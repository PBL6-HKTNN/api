namespace Codemy.BuildingBlocks.Domain.TestData
{
    public class GetQuizResultsTestData
    {
        public string TestCase { get; set; }
        public string LessonId { get; set; }
        public int ExpectedStatus { get; set; }
        public bool ExpectedSuccess { get; set; }
        public string ExpectedMessage { get; set; }
        public string Description { get; set; }
    }
}
