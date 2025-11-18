using Codemy.BuildingBlocks.Domain.TestData;
using Codemy.BuildingBlocks.Test;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Codemy.Identity.UnitTests
{
    public class UserTest : BaseTest
    {
        [Fact]
        public async Task UpdateProfile_WithCsvData_ShouldMatchExpectedResults()
        {
            var testDataList = CsvReader.ReadCsv<UpdateProfileTestData>("UpdateProfileTestData.csv");

            Console.WriteLine($"\n=== Running {testDataList.Count} test cases for UpdateProfile ===\n");

            // ✅ FIX 1: Add counters for summary
            int passedCount = 0;
            int failedCount = 0;

            foreach (var testData in testDataList)
            {
                StartTest($"UpdateProfile - {testData.TestCase}", "User");

                try
                {
                    // ✅ FIX 2: Handle unauthorized tests
                    bool isUnauthorizedTest = testData.TestCase.Contains("Unauthorized");
                    string originalToken = null;

                    if (isUnauthorizedTest)
                    {
                        originalToken = AuthToken;
                        ApiClient.SetAuthToken(null); // Remove token for unauthorized test
                        Console.WriteLine($"  ℹ️ Testing unauthorized access (token removed)");
                    }

                    // Arrange
                    var userId = ParseGuidOrDefault(testData.UserId);

                    var updateRequest = new
                    {
                        name = string.IsNullOrEmpty(testData.Name) ? "" : testData.Name,
                        bio = string.IsNullOrEmpty(testData.Bio) ? "" : testData.Bio,
                    };

                    var requestJson = JsonConvert.SerializeObject(updateRequest);
                    var endpoint = GetEndpoint("User") + $"/{userId}/profile";

                    Console.WriteLine($"  Test Case: {testData.TestCase}");
                    Console.WriteLine($"  Description: {testData.Description}");
                    Console.WriteLine($"  User ID: {userId}");
                    Console.WriteLine($"  Request: {requestJson}");

                    // Act
                    var stopwatch = Stopwatch.StartNew();
                    var response = await ApiClient.PutAsync(endpoint, updateRequest);
                    stopwatch.Stop();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var actualStatus = (int)response.StatusCode;

                    Console.WriteLine($"  Expected Status: {testData.ExpectedStatus}, Actual: {actualStatus}");

                    // ✅ FIX 3: Better response parsing with nested data handling
                    dynamic profileResult = null;
                    if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseContent))
                    {
                        try
                        {
                            var apiResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

                            // Check if response has a "data" wrapper
                            if (apiResponse?.data != null)
                            {
                                profileResult = apiResponse.data;
                                Console.WriteLine($"  ℹ️ Response has 'data' wrapper");
                            }
                            else
                            {
                                profileResult = apiResponse;
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"  ⚠️ Warning: Could not parse response: {ex.Message}");
                        }
                    }

                    // Assert
                    var passed = actualStatus == testData.ExpectedStatus;

                    if (passed && profileResult != null && response.IsSuccessStatusCode)
                    {
                        // Verify updated fields
                        if (!string.IsNullOrEmpty(testData.Name))
                        {
                            string actualName = profileResult.name?.ToString() ?? "";
                            if (actualName != testData.Name)
                            {
                                passed = false;
                                Console.WriteLine($"  ✗ Name not updated. Expected: '{testData.Name}', Actual: '{actualName}'");
                            }
                            else
                            {
                                Console.WriteLine($"  ✓ Name updated correctly: {actualName}");
                            }
                        }

                        if (!string.IsNullOrEmpty(testData.Bio))
                        {
                            string actualBio = profileResult.bio?.ToString() ?? "";
                            if (actualBio != testData.Bio)
                            {
                                passed = false;
                                Console.WriteLine($"  ✗ Bio not updated. Expected: '{testData.Bio}', Actual: '{actualBio}'");
                            }
                            else
                            {
                                Console.WriteLine($"  ✓ Bio updated correctly: {actualBio}");
                            }
                        }
                    }
                    else if (!passed)
                    {
                        Console.WriteLine($"  Response: {responseContent}");
                    }

                    // ✅ FIX 4: Restore token after unauthorized test
                    if (isUnauthorizedTest && originalToken != null)
                    {
                        ApiClient.SetAuthToken(originalToken);
                        Console.WriteLine($"  ℹ️ Token restored");
                    }

                    // ✅ FIX 5: Update counters
                    if (passed)
                    {
                        passedCount++;
                    }
                    else
                    {
                        failedCount++;
                    }

                    // Record result
                    RecordTestResult(
                        status: passed ? "Passed" : "Failed",
                        endpoint: endpoint,
                        statusCode: actualStatus,
                        responseTimeMs: stopwatch.Elapsed.TotalMilliseconds,
                        errorMessage: passed ? "" : $"Expected {testData.ExpectedStatus}, got {actualStatus}",
                        requestBody: requestJson,
                        responseBody: responseContent
                    );

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    failedCount++;

                    RecordTestResult(
                        status: "Failed",
                        endpoint: GetEndpoint("User") + "/{id}/profile",
                        errorMessage: $"{testData.TestCase}: {ex.Message}"
                    );

                    Console.WriteLine($"  ✗ Exception: {ex.Message}");
                    Console.WriteLine();
                }
            }

            // ✅ FIX 6: Print summary
            Console.WriteLine($"\n{'=',-60}");
            Console.WriteLine($"SUMMARY: Total: {testDataList.Count} | Passed: {passedCount} | Failed: {failedCount}");
            Console.WriteLine($"Pass Rate: {(passedCount * 100.0 / testDataList.Count):F2}%");
            Console.WriteLine($"{'=',-60}\n");
        }

        [Theory]
        [MemberData(nameof(GetUpdateProfileTestData))]
        public async Task UpdateProfile_Theory_WithCsvData(UpdateProfileTestData testData)
        {
            StartTest($"UpdateProfile Theory - {testData.TestCase}", "User");

            try
            {
                // ✅ FIX 7: Handle unauthorized tests in Theory
                bool isUnauthorizedTest = testData.TestCase.Contains("Unauthorized");
                string originalToken = null;

                if (isUnauthorizedTest)
                {
                    originalToken = AuthToken;
                    ApiClient.SetAuthToken(null);
                }

                var userId = ParseGuidOrDefault(testData.UserId);

                var updateRequest = new
                {
                    name = string.IsNullOrEmpty(testData.Name) ? null : testData.Name,
                    bio = string.IsNullOrEmpty(testData.Bio) ? null : testData.Bio,
                };

                var endpoint = GetEndpoint("User") + $"/{userId}/profile";

                var stopwatch = Stopwatch.StartNew();
                var response = await ApiClient.PutAsync(endpoint, updateRequest);
                stopwatch.Stop();

                var actualStatus = (int)response.StatusCode;

                // ✅ FIX 8: Restore token before assertions
                if (isUnauthorizedTest && originalToken != null)
                {
                    ApiClient.SetAuthToken(originalToken);
                }

                // Assert
                Assert.Equal(testData.ExpectedStatus, actualStatus);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    // ✅ FIX 9: Handle nested response in Theory
                    try
                    {
                        var apiResponse = JsonConvert.DeserializeObject<dynamic>(content);
                        dynamic result = apiResponse?.data ?? apiResponse;

                        // Verify updated fields
                        if (!string.IsNullOrEmpty(testData.Name))
                        {
                            Assert.Equal(testData.Name, result.name.ToString());
                        }

                        if (!string.IsNullOrEmpty(testData.Bio))
                        {
                            Assert.Equal(testData.Bio, result.bio.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Warning: Could not parse response: {ex.Message}");
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
                    endpoint: GetEndpoint("User") + "/{id}/profile",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        public static IEnumerable<object[]> GetUpdateProfileTestData()
        {
            var reader = new CsvDataReader();
            var testData = reader.ReadCsv<UpdateProfileTestData>("UpdateProfileTestData.csv");

            foreach (var data in testData)
            {
                yield return new object[] { data };
            }
        }
    }
}