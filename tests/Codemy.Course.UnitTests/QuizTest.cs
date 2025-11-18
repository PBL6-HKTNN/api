using Codemy.BuildingBlocks.Domain.TestData;
using Codemy.BuildingBlocks.Test;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Codemy.Course.UnitTests
{
    public class QuizTest : BaseTest
    {
        [Fact]
        public async Task GetQuizResults_WithCsvData_ShouldMatchExpectedResults()
        {
            var testDataList = CsvReader.ReadCsv<GetQuizResultsTestData>("GetQuizResultsTestData.csv");

            Console.WriteLine($"\n=== Running {testDataList.Count} test cases for GetQuizResults ===\n");

            foreach (var testData in testDataList)
            {
                StartTest($"GetQuizResults - {testData.TestCase}", "Quiz");

                try
                {
                    // Arrange
                    var lessonId = ParseGuidOrDefault(testData.LessonId);
                    var endpoint = GetEndpoint("Quiz") + $"/results/{lessonId}";

                    Console.WriteLine($"  Test Case: {testData.TestCase}");
                    Console.WriteLine($"  Description: {testData.Description}");
                    Console.WriteLine($"  Lesson ID: {lessonId}");

                    // Act
                    var stopwatch = Stopwatch.StartNew();
                    var response = await ApiClient.GetAsync(endpoint);
                    stopwatch.Stop();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var actualStatus = (int)response.StatusCode;

                    Console.WriteLine($"  Expected Status: {testData.ExpectedStatus}, Actual: {actualStatus}");

                    // Parse response
                    dynamic quizResult = null;
                    if (response.IsSuccessStatusCode)
                    {
                        quizResult = JsonConvert.DeserializeObject(responseContent);
                    }

                    // Assert
                    var passed = actualStatus == testData.ExpectedStatus;

                    if (passed && quizResult != null)
                    {
                        bool actualSuccess = quizResult.success ?? false;
                        if (actualSuccess != testData.ExpectedSuccess)
                        {
                            passed = false;
                            Console.WriteLine($"  ✗ Success flag mismatch. Expected: {testData.ExpectedSuccess}, Actual: {actualSuccess}");
                        }
                    }
                    if (!string.IsNullOrEmpty(testData.ExpectedMessage))
                    {
                        string actualMessage = quizResult.message?.ToString() ?? "";
                        if (!actualMessage.Contains(testData.ExpectedMessage))
                        {
                            passed = false;
                            Console.WriteLine($"  ✗ Message mismatch. Expected to contain: '{testData.ExpectedMessage}'");
                            Console.WriteLine($"  Actual message: '{actualMessage}'");
                        }
                    }


                    // Record result
                    RecordTestResult(
                        status: passed ? "Passed" : "Failed",
                        endpoint: endpoint,
                        statusCode: actualStatus,
                        responseTimeMs: stopwatch.Elapsed.TotalMilliseconds,
                        errorMessage: passed ? "" : $"Expected {testData.ExpectedStatus}, got {actualStatus}",
                        requestBody: $"LessonId: {lessonId}",
                        responseBody: responseContent
                    );

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    RecordTestResult(
                        status: "Failed",
                        endpoint: GetEndpoint("Quiz") + "/results/{lessonId}",
                        errorMessage: $"{testData.TestCase}: {ex.Message}"
                    );

                    Console.WriteLine($"  ✗ Exception: {ex.Message}");
                    Console.WriteLine();
                }
            }
        }

        [Fact]
        public async Task GetQuizAttempts_WithCsvData_ShouldMatchExpectedResults()
        {
            var testDataList = CsvReader.ReadCsv<GetQuizAttemptsTestData>("GetQuizAttemptsTestData.csv");

            Console.WriteLine($"\n=== Running {testDataList.Count} test cases for GetQuizAttempts ===\n");

            foreach (var testData in testDataList)
            {
                StartTest($"GetQuizAttempts - {testData.TestCase}", "Quiz");

                try
                {
                    // Arrange
                    var quizId = ParseGuidOrDefault(testData.QuizId);
                    var endpoint = GetEndpoint("Quiz") + $"/Attempts/{quizId}";

                    Console.WriteLine($"  Test Case: {testData.TestCase}");
                    Console.WriteLine($"  Description: {testData.Description}");
                    Console.WriteLine($"  Quiz ID: {quizId}");

                    // Act
                    var stopwatch = Stopwatch.StartNew();
                    var response = await ApiClient.GetAsync(endpoint);
                    stopwatch.Stop();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var actualStatus = (int)response.StatusCode;

                    Console.WriteLine($"  Expected Status: {testData.ExpectedStatus}, Actual: {actualStatus}");

                    // Parse response
                    dynamic attemptsResult = null;
                    try
                    {
                        attemptsResult = JsonConvert.DeserializeObject(responseContent);
                    }
                    catch
                    {
                        // Response might be plain text or different format
                    }

                    // Assert
                    var passed = actualStatus == testData.ExpectedStatus;

                    if (passed && attemptsResult != null && actualStatus == 200)
                    {
                        // Verify it's an array/list
                        if (attemptsResult is Newtonsoft.Json.Linq.JArray)
                        {
                            Console.WriteLine($"  ✓ Returned {attemptsResult.Count} attempts");
                        }
                    }

                    if (!passed)
                    {
                        Console.WriteLine($"  ✗ Status code mismatch");
                    }

                    // Record result
                    RecordTestResult(
                        status: passed ? "Passed" : "Failed",
                        endpoint: endpoint,
                        statusCode: actualStatus,
                        responseTimeMs: stopwatch.Elapsed.TotalMilliseconds,
                        errorMessage: passed ? "" : $"Expected {testData.ExpectedStatus}, got {actualStatus}",
                        requestBody: $"QuizId: {quizId}",
                        responseBody: responseContent
                    );

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    RecordTestResult(
                        status: "Failed",
                        endpoint: GetEndpoint("Quiz") + "/Attempts/{quizId}",
                        errorMessage: $"{testData.TestCase}: {ex.Message}"
                    );

                    Console.WriteLine($"  ✗ Exception: {ex.Message}");
                    Console.WriteLine();
                }
            }
        }

        [Theory]
        [MemberData(nameof(GetQuizResultsTestData))]
        public async Task GetQuizResults_Theory_WithCsvData(GetQuizResultsTestData testData)
        {
            StartTest($"GetQuizResults Theory - {testData.TestCase}", "Quiz");

            try
            {
                var lessonId = ParseGuidOrDefault(testData.LessonId);
                var endpoint = GetEndpoint("Quiz") + $"/results/{lessonId}";

                var stopwatch = Stopwatch.StartNew();
                var response = await ApiClient.GetAsync(endpoint);
                stopwatch.Stop();

                var actualStatus = (int)response.StatusCode;

                // Assert status code
                Assert.Equal(testData.ExpectedStatus, actualStatus);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(content);

                    bool actualSuccess = result.success ?? false;
                    Assert.Equal(testData.ExpectedSuccess, actualSuccess);

                    if (!string.IsNullOrEmpty(testData.ExpectedMessage))
                    {
                        string actualMessage = result.message?.ToString() ?? "";
                        Assert.Contains(testData.ExpectedMessage, actualMessage);
                    }
                }

                RecordTestResult(
                    status: "Passed",
                    endpoint: endpoint,
                    statusCode: actualStatus,
                    responseTimeMs: stopwatch.Elapsed.TotalMilliseconds
                );
            }
            catch (Exception ex)
            {
                RecordTestResult(
                    status: "Failed",
                    endpoint: GetEndpoint("Quiz") + "/results/{lessonId}",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        public static IEnumerable<object[]> GetQuizResultsTestData()
        {
            var reader = new CsvDataReader();
            var testData = reader.ReadCsv<GetQuizResultsTestData>("GetQuizResultsTestData.csv");

            foreach (var data in testData)
            {
                yield return new object[] { data };
            }
        }
    }
}

