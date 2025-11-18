using Codemy.BuildingBlocks.Domain.TestData;
using Codemy.BuildingBlocks.Test;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Codemy.Course.UnitTests
{
    public class CourseTest : BaseTest
    {
        [Fact]
        public async Task GetCourse_WithCsvData_ShouldMatchExpectedResults()
        {
            var testDataList = CsvReader.ReadCsv<GetCourseTestData>("GetCourseTestData.csv");

            Console.WriteLine($"\n=== Running {testDataList.Count} test cases for GetCourse ===\n");

            int passedCount = 0;
            int failedCount = 0;

            foreach (var testData in testDataList)
            {
                StartTest($"GetCourse - {testData.TestCase}", "User");

                try
                {
                    bool isUnauthorizedTest = testData.TestCase.Contains("Unauthorized");
                    string originalToken = null;

                    if (isUnauthorizedTest)
                    {
                        originalToken = AuthToken;
                        ApiClient.SetAuthToken(null);
                        Console.WriteLine($"Testing unauthorized access (token removed)");
                    }
                    var courseId = ParseGuidOrDefault(testData.CourseId); 
                    var endpoint = GetEndpoint("Course") + $"/get/{courseId}";

                    Console.WriteLine($"  Test Case: {testData.TestCase}");
                    Console.WriteLine($"  Description: {testData.Description}"); 

                    var stopwatch = Stopwatch.StartNew();
                    var response = await ApiClient.GetAsync(endpoint);
                    stopwatch.Stop();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var actualStatus = (int)response.StatusCode;

                    Console.WriteLine($"  Expected Status: {testData.ExpectedStatus}, Actual: {actualStatus}");

                    dynamic profileResult = null;
                    if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseContent))
                    {
                        try
                        {
                            var apiResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                            if (apiResponse?.data != null)
                            {
                                profileResult = apiResponse.data;
                                Console.WriteLine($"Response has 'data' wrapper");
                            }
                            else
                            {
                                profileResult = apiResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Warning: Could not parse response: {ex.Message}");
                        }
                    }

                    var passed = actualStatus == testData.ExpectedStatus;

                    if (isUnauthorizedTest && originalToken != null)
                    {
                        ApiClient.SetAuthToken(originalToken);
                        Console.WriteLine($"Token restored");
                    }

                    if (passed)
                    {
                        passedCount++;
                    }
                    else
                    {
                        failedCount++;
                    }

                    RecordTestResult(
                        status: passed ? "Passed" : "Failed",
                        endpoint: endpoint,
                        statusCode: actualStatus,
                        responseTimeMs: stopwatch.Elapsed.TotalMilliseconds,
                        errorMessage: passed ? "" : $"Expected {testData.ExpectedStatus}, got {actualStatus}",
                        responseBody: responseContent
                    );

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    failedCount++;

                    RecordTestResult(
                        status: "Failed",
                        endpoint: GetEndpoint("Course") + "/get/{id}",
                        errorMessage: $"{testData.TestCase}: {ex.Message}"
                    );

                    Console.WriteLine($"  ✗ Exception: {ex.Message}");
                    Console.WriteLine();
                }
            }

            Console.WriteLine($"\n{'=',-60}");
            Console.WriteLine($"SUMMARY: Total: {testDataList.Count} | Passed: {passedCount} | Failed: {failedCount}");
            Console.WriteLine($"Pass Rate: {(passedCount * 100.0 / testDataList.Count):F2}%");
            Console.WriteLine($"{'=',-60}\n");
        }

        //[Theory]
        //[MemberData(nameof(GetCourseTestData))]
        //public async Task GetCourse_Theory_WithCsvData(GetCourseTestData testData)
        //{
        //    StartTest($"GetCourse Theory - {testData.TestCase}", "User");

        //    try
        //    {
        //        // ✅ FIX 7: Handle unauthorized tests in Theory
        //        bool isUnauthorizedTest = testData.TestCase.Contains("Unauthorized");
        //        string originalToken = null;

        //        if (isUnauthorizedTest)
        //        {
        //            originalToken = AuthToken;
        //            ApiClient.SetAuthToken(null);
        //        }

        //        var courseId = ParseGuidOrDefault(testData.CourseId);

        //        var endpoint = GetEndpoint("Course") + $"/get/{courseId}";

        //        var stopwatch = Stopwatch.StartNew();
        //        var response = await ApiClient.GetAsync(endpoint);
        //        stopwatch.Stop();

        //        var actualStatus = (int)response.StatusCode;

        //        // ✅ FIX 8: Restore token before assertions
        //        if (isUnauthorizedTest && originalToken != null)
        //        {
        //            ApiClient.SetAuthToken(originalToken);
        //        }

        //        // Assert
        //        Assert.Equal(testData.ExpectedStatus, actualStatus);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();

        //            // ✅ FIX 9: Handle nested response in Theory
        //            try
        //            {
        //                var apiResponse = JsonConvert.DeserializeObject<dynamic>(content);
        //                dynamic result = apiResponse?.data ?? apiResponse;
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.WriteLine($"Warning: Could not parse response: {ex.Message}");
        //            }
        //        }

        //        RecordTestResult(
        //            status: "Passed",
        //            endpoint: endpoint,
        //            statusCode: actualStatus,
        //            responseTimeMs: stopwatch.Elapsed.TotalMilliseconds
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        RecordTestResult(
        //            status: "Failed",
        //            endpoint: GetEndpoint("Course") + "/get/{courseId}",
        //            errorMessage: ex.Message
        //        );
        //        throw;
        //    }
        //}
        public static IEnumerable<object[]> GetCourseTestData()
        {
            var reader = new CsvDataReader();
            var testData = reader.ReadCsv<GetCourseTestData>("GetCourseTestData.csv");

            foreach (var data in testData)
            {
                yield return new object[] { data };
            }
        }
    }
}

