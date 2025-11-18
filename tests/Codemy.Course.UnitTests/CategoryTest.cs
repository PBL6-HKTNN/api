using Codemy.BuildingBlocks.Domain.TestData;
using Codemy.BuildingBlocks.Test; 
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;

namespace Codemy.Course.UnitTests
{
    public class CategoryTest : BaseTest
    {
        [Fact]
        public async Task CreateCategory_WithCsvData_ShouldMatchExpectedResults()
        {
            var testDataList = CsvReader.ReadCsv<CreateCategoryTestData>("CreateCategoryTestData.csv");

            int passedCount = 0;
            int failedCount = 0;

            foreach (var testData in testDataList)
            {
                StartTest($"CreateCategory - {testData.TestCase}", "Category");

                try
                {
                    var categoryName = testData.Name;

                    if (!string.IsNullOrEmpty(categoryName))
                    {
                        categoryName = ReplacePlaceholders(categoryName);
                    }

                    var request = new
                    {
                        name = categoryName,
                        description = testData.Description
                    };

                    var requestJson = JsonConvert.SerializeObject(request);
                    var endpoint = GetEndpoint("Category") + "/create";

                    Console.WriteLine($"  Test Case: {testData.TestCase}");
                    Console.WriteLine($"  Description: {testData.Description_TestCase}");
                    Console.WriteLine($"  Category Name: {categoryName ?? "(empty)"}");

                    var stopwatch = Stopwatch.StartNew();
                    var response = await ApiClient.PostAsync(endpoint, request);
                    stopwatch.Stop();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var actualStatus = (int)response.StatusCode;

                    Console.WriteLine($"  Expected Status: {testData.ExpectedStatus}, Actual: {actualStatus}");

                    dynamic categoryResult = null;
                    try
                    {
                        categoryResult = JsonConvert.DeserializeObject(responseContent);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to parse response: {ex.Message}");
                    }

                    var passed = actualStatus == testData.ExpectedStatus;

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
                        endpoint: GetEndpoint("Category") + "/create",
                        errorMessage: $"{testData.TestCase}: {ex.Message}"
                    );

                    Console.WriteLine($"Exception: {ex.Message}");
                    Console.WriteLine();
                }
            }

            Console.WriteLine($"\n{'=',-60}");
            Console.WriteLine($"SUMMARY: Total: {testDataList.Count} | Passed: {passedCount} | Failed: {failedCount}");
            Console.WriteLine($"Pass Rate: {(testDataList.Count > 0 ? passedCount * 100.0 / testDataList.Count : 0):F2}%");
            Console.WriteLine($"{'=',-60}\n");
        }

        [Theory]
        [MemberData(nameof(GetCreateCategoryTestData))]
        public async Task CreateCategory_Theory_WithCsvData(CreateCategoryTestData testData)
        {
            StartTest($"CreateCategory Theory - {testData.TestCase}", "Category");

            try
            {
                // Arrange
                var categoryName = testData.Name;
                if (!string.IsNullOrEmpty(categoryName))
                {
                    categoryName = ReplacePlaceholders(categoryName);
                }

                var request = new
                {
                    name = categoryName,
                    description = testData.Description
                };

                var endpoint = GetEndpoint("Category") + "/create";

                // Act
                var stopwatch = Stopwatch.StartNew();
                var response = await ApiClient.PostAsync(endpoint, request);
                stopwatch.Stop();

                var actualStatus = (int)response.StatusCode;
                var content = await response.Content.ReadAsStringAsync();

                // Assert status code
                Assert.Equal(testData.ExpectedStatus, actualStatus);

                dynamic result = JsonConvert.DeserializeObject(content);

                // Assert success flag
                bool actualSuccess = result.success ?? false;
                Assert.Equal(testData.ExpectedSuccess, actualSuccess);

                // Assert message if specified
                if (!string.IsNullOrEmpty(testData.ExpectedMessage))
                {
                    string actualMessage = result.message?.ToString() ?? "";
                    Assert.Contains(testData.ExpectedMessage, actualMessage, StringComparison.OrdinalIgnoreCase);
                }

                // If successful, verify category was created
                if (actualSuccess && result.data != null)
                {
                    Assert.NotNull(result.data.id);

                    if (!string.IsNullOrEmpty(categoryName))
                    {
                        Assert.Equal(categoryName, result.data.name.ToString());
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
                    endpoint: GetEndpoint("Category") + "/create",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        [Fact]
        public async Task CreateCategory_ValidData_ShouldReturnCreatedCategory()
        {
            StartTest("CreateCategory - Valid Data Single Test", "Category");

            try
            {
                var uniqueName = $"Test Category {DateTime.Now:yyyyMMddHHmmss}";
                var request = new
                {
                    name = uniqueName,
                    description = "This is a test category created by automation test"
                };

                var endpoint = GetEndpoint("Category") + "/create";

                Console.WriteLine($"  Creating category: {uniqueName}");

                var stopwatch = Stopwatch.StartNew();
                var response = await ApiClient.PostAsync(endpoint, request);
                stopwatch.Stop();

                var content = await response.Content.ReadAsStringAsync();
                var actualStatus = (int)response.StatusCode;

                Assert.Equal(201, actualStatus);

                dynamic result = JsonConvert.DeserializeObject(content);
                Assert.True((bool)(result.success ?? false));
                Assert.NotNull(result.data);
                Assert.Equal(uniqueName, result.data.name.ToString());

                Console.WriteLine($"Category created successfully with ID: {result.data.id}");

                RecordTestResult(
                    status: "Passed",
                    endpoint: endpoint,
                    statusCode: actualStatus,
                    responseTimeMs: stopwatch.Elapsed.TotalMilliseconds,
                    requestBody: JsonConvert.SerializeObject(request),
                    responseBody: content
                );
            }
            catch (Exception ex)
            {
                RecordTestResult(
                    status: "Failed",
                    endpoint: GetEndpoint("Category") + "/create",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        [Fact]
        public async Task CreateCategory_DuplicateName_ShouldReturnBadRequest()
        {
            StartTest("CreateCategory - Duplicate Name Test", "Category");

            try
            {
                var duplicateName = $"Duplicate Test {DateTime.Now:yyyyMMddHHmmss}";
                var request = new
                {
                    name = duplicateName,
                    description = "First category"
                };

                var endpoint = GetEndpoint("Category") + "/create";

                var firstResponse = await ApiClient.PostAsync(endpoint, request);
                Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

                Console.WriteLine($"  ✓ First category created: {duplicateName}");

                var stopwatch = Stopwatch.StartNew();
                var duplicateResponse = await ApiClient.PostAsync(endpoint, request);
                stopwatch.Stop();

                var content = await duplicateResponse.Content.ReadAsStringAsync();
                var actualStatus = (int)duplicateResponse.StatusCode;

                Console.WriteLine($"  Attempting to create duplicate...");
                Console.WriteLine($"  Response status: {actualStatus}");

                Assert.Equal(400, actualStatus);

                dynamic result = JsonConvert.DeserializeObject(content);
                Assert.False((bool)(result.success ?? true));
                Assert.Contains("already exists", result.message.ToString(), StringComparison.OrdinalIgnoreCase);

                Console.WriteLine($"  ✓ Duplicate correctly rejected");

                RecordTestResult(
                    status: "Passed",
                    endpoint: endpoint,
                    statusCode: actualStatus,
                    responseTimeMs: stopwatch.Elapsed.TotalMilliseconds,
                    requestBody: JsonConvert.SerializeObject(request),
                    responseBody: content
                );
            }
            catch (Exception ex)
            {
                RecordTestResult(
                    status: "Failed",
                    endpoint: GetEndpoint("Category") + "/create",
                    errorMessage: ex.Message
                );
                throw;
            }
        }

        public static IEnumerable<object[]> GetCreateCategoryTestData()
        {
            var reader = new CsvDataReader();
            var testData = reader.ReadCsv<CreateCategoryTestData>("CreateCategoryTestData.csv");

            foreach (var data in testData)
            {
                yield return new object[] { data };
            }
        }
    }
}
