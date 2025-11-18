namespace Codemy.BuildingBlocks.Domain.TestData
{
    public class GetQuizAttemptsTestData
    {
        public string TestCase { get; set; }
        public string QuizId { get; set; }
        public int ExpectedStatus { get; set; }
        public bool ExpectedSuccess { get; set; }
        public string ExpectedMessage { get; set; }
        public string Description { get; set; }
    }
}
