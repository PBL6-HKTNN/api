using Codemy.BuildingBlocks.Domain;
using Codemy.Identity.API.DTOs;
using Codemy.Identity.Application.DTOs.Authentication;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Codemy.BuildingBlocks.Test
{
    public class BaseTest : IDisposable
    {
        protected ApiClient ApiClient;
        protected TestSettings Settings;
        protected static CsvReporter Reporter;
        protected CsvDataReader CsvReader;

        private Stopwatch _stopwatch;
        private string _currentTestName;
        protected string AuthToken;
        protected Guid CurrentUserId; // Lưu user ID của user đang login

        // Dictionary để lưu các IDs thật để replace placeholders
        protected Dictionary<string, string> TestDataReplacements;

        public BaseTest()
        {
            // Load config
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json")
                .Build();

            Settings = new TestSettings();
            config.Bind(Settings);

            // Initialize ApiClient với SSL handling
            ApiClient = new ApiClient(
                baseUrl: Settings.ApiGateway.BaseUrl,
                timeoutSeconds: Settings.ApiGateway.Timeout,
                ignoreSslErrors: Settings.ApiGateway.IgnoreSslErrors
            );

            // Initialize CSV Reader
            CsvReader = new CsvDataReader();

            // Initialize Reporter (static để share across tests)
            if (Reporter == null)
            {
                Reporter = new CsvReporter();
            }

            // Initialize replacements dictionary
            TestDataReplacements = new Dictionary<string, string>();

            _stopwatch = new Stopwatch();

            // Auto login
            LoginAsync().GetAwaiter().GetResult();

            // Setup test data (tạo dữ liệu thật để test)
            SetupTestDataAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Login và lấy auth token
        /// </summary>
        protected async Task LoginAsync()
        {
            try
            {
                Console.WriteLine("=== Attempting to login ===");

                var loginRequest = new LoginRequest
                {
                    email = Settings.TestUser.Email,
                    password = Settings.TestUser.Password
                };

                var endpoint = Settings.Endpoints["Auth"] + "/login";
                var response = await ApiClient.PostAsync(endpoint, loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    // ✅ FIX: Handle nested response with "data" wrapper
                    try
                    {
                        var apiResponse = JsonConvert.DeserializeObject<dynamic>(content);

                        // Check if response has "data" wrapper
                        dynamic loginData = apiResponse?.data ?? apiResponse;

                        // Try different property names for token
                        string token = null;
                        if (loginData?.token != null)
                            token = loginData.token.ToString();
                        else if (loginData?.accessToken != null)
                            token = loginData.accessToken.ToString();
                        else if (loginData?.Token != null)
                            token = loginData.Token.ToString();
                        else if (loginData?.AccessToken != null)
                            token = loginData.AccessToken.ToString();

                        if (!string.IsNullOrEmpty(token))
                        {
                            AuthToken = token;
                            ApiClient.SetAuthToken(AuthToken);

                            // Lấy user ID từ token
                            CurrentUserId = GetUserIdFromToken();

                            Console.WriteLine("✓ Login successful!");
                            Console.WriteLine($"User: {Settings.TestUser.Email}");
                            Console.WriteLine($"User ID: {CurrentUserId}");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("✗ Could not extract token from response");
                            Console.WriteLine($"Response: {content}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ Error parsing login response: {ex.Message}");
                        Console.WriteLine($"Response: {content}");
                    }
                }
                else
                {
                    Console.WriteLine($"✗ Login failed: {response.StatusCode}");
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Login error: {ex.Message}");
            }
        }

        /// <summary>
        /// Setup test data - Tạo dữ liệu thật để replace placeholders
        /// </summary>
        protected virtual async Task SetupTestDataAsync()
        {
            try
            {
                //Console.WriteLine("=== Setting up test data ===");

                //// 1. Tạo Course và Lesson để có Quiz
                //var courseAndLessonIds = await CreateTestCourseWithLesson();
                //var courseId = courseAndLessonIds.Item1;
                //var lessonId = courseAndLessonIds.Item2;

                //// 2. Tạo Quiz cho lesson vừa tạo
                //var quizId = await CreateTestQuizForLesson(lessonId);
                //TestDataReplacements["VALID_QUIZ_ID"] = quizId.ToString();

                //// 3. Submit quiz để tạo attempt (user đã complete)
                //await SubmitTestQuiz(quizId);
                //TestDataReplacements["VALID_LESSON_ID"] = lessonId.ToString();

                //// 4. Tạo lesson + quiz khác nhưng chưa submit (no attempt)
                //var anotherLesson = await CreateTestLesson(courseId);
                //var quizIdNoAttempt = await CreateTestQuizForLesson(anotherLesson);
                //TestDataReplacements["VALID_LESSON_ID_NO_ATTEMPT"] = anotherLesson.ToString();
                //TestDataReplacements["VALID_QUIZ_ID_NO_ATTEMPTS"] = quizIdNoAttempt.ToString();

                //// 5. Lưu user ID
                //TestDataReplacements["VALID_USER_ID"] = CurrentUserId.ToString();

                //Console.WriteLine("✓ Test data setup completed!");
                //Console.WriteLine($"Valid Lesson ID (with attempt): {lessonId}");
                //Console.WriteLine($"Valid Lesson ID (no attempt): {anotherLesson}");
                //Console.WriteLine($"Valid Quiz ID: {quizId}");
                //Console.WriteLine($"Valid User ID: {CurrentUserId}");
                //Console.WriteLine();
                Console.WriteLine("\n=== Setting up test data ===");

                // Setup cho Category tests
                await SetupCategoryTestData();

                // Setup cho Quiz tests (nếu cần)
                // await SetupQuizTestData();

                // Setup cho User tests
                TestDataReplacements["VALID_USER_ID"] = CurrentUserId.ToString();

                Console.WriteLine("✓ Test data setup completed!");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error setting up test data: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        #region Helper Methods để tạo test data

        /// <summary>
        /// Tạo Course và Lesson
        /// </summary>
        protected virtual async Task<(Guid, Guid)> CreateTestCourseWithLesson()
        {
            try
            {
                Console.WriteLine("  Creating course with lesson...");

                // TODO: Implement API call để tạo course
                // Ví dụ:
                var createCourseRequest = new
                {
                    title = $"Test Course {DateTime.Now:yyyyMMddHHmmss}",
                    description = "Course for automation testing",
                    price = 99.99m,
                    instructorId = CurrentUserId
                };

                var courseEndpoint = Settings.Endpoints.ContainsKey("Course")
                    ? Settings.Endpoints["Course"]
                    : "/gateway/api/courses";

                var courseResponse = await ApiClient.PostAsync(courseEndpoint, createCourseRequest);

                if (!courseResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to create course: {courseResponse.StatusCode}");
                    return (Guid.NewGuid(), Guid.NewGuid()); // Fallback
                }

                var courseContent = await courseResponse.Content.ReadAsStringAsync();
                dynamic courseData = JsonConvert.DeserializeObject(courseContent);
                Guid courseId = Guid.Parse(courseData.id.ToString());

                // Tạo Lesson cho course
                var createLessonRequest = new
                {
                    courseId = courseId,
                    title = $"Test Lesson {DateTime.Now:yyyyMMddHHmmss}",
                    content = "Lesson for testing quiz",
                    order = 1
                };

                var lessonEndpoint = Settings.Endpoints.ContainsKey("Lesson")
                    ? Settings.Endpoints["Lesson"]
                    : "/gateway/api/lessons";

                var lessonResponse = await ApiClient.PostAsync(lessonEndpoint, createLessonRequest);

                if (!lessonResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to create lesson: {lessonResponse.StatusCode}");
                    return (courseId, Guid.NewGuid()); // Fallback
                }

                var lessonContent = await lessonResponse.Content.ReadAsStringAsync();
                dynamic lessonData = JsonConvert.DeserializeObject(lessonContent);
                Guid lessonId = Guid.Parse(lessonData.id.ToString());

                Console.WriteLine($"  ✓ Created Course: {courseId}, Lesson: {lessonId}");
                return (courseId, lessonId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Error creating course/lesson: {ex.Message}");
                // Return mock IDs nếu không tạo được
                return (Guid.NewGuid(), Guid.NewGuid());
            }
        }

        /// <summary>
        /// Tạo Lesson mới cho một course đã có
        /// </summary>
        protected virtual async Task<Guid> CreateTestLesson(Guid courseId)
        {
            try
            {
                var createLessonRequest = new
                {
                    courseId = courseId,
                    title = $"Another Test Lesson {DateTime.Now:yyyyMMddHHmmss}",
                    content = "Another lesson for testing",
                    order = 2
                };

                var lessonEndpoint = Settings.Endpoints.ContainsKey("Lesson")
                    ? Settings.Endpoints["Lesson"]
                    : "/gateway/api/lessons";

                var response = await ApiClient.PostAsync(lessonEndpoint, createLessonRequest);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(content);
                    return Guid.Parse(data.id.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Error creating lesson: {ex.Message}");
            }

            return Guid.NewGuid();
        }

        /// <summary>
        /// Tạo Quiz cho một lesson
        /// </summary>
        protected virtual async Task<Guid> CreateTestQuizForLesson(Guid lessonId)
        {
            try
            {
                Console.WriteLine($"  Creating quiz for lesson {lessonId}...");

                var createQuizRequest = new
                {
                    lessonId = lessonId,
                    title = $"Test Quiz {DateTime.Now:yyyyMMddHHmmss}",
                    description = "Quiz for automation testing",
                    passingScore = 70,
                    timeLimit = 30,
                    questions = new[]
                    {
                        new
                        {
                            questionText = "What is 2 + 2?",
                            questionType = "MultipleChoice",
                            points = 10,
                            answers = new[]
                            {
                                new { answerText = "3", isCorrect = false },
                                new { answerText = "4", isCorrect = true },
                                new { answerText = "5", isCorrect = false }
                            }
                        }
                    }
                };

                var quizEndpoint = Settings.Endpoints.ContainsKey("Quiz")
                    ? Settings.Endpoints["Quiz"] + "/create"
                    : "/gateway/api/Quiz/create";

                var response = await ApiClient.PostAsync(quizEndpoint, createQuizRequest);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(content);
                    Guid quizId = Guid.Parse(data.id.ToString());
                    Console.WriteLine($"  ✓ Created Quiz: {quizId}");
                    return quizId;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"  ✗ Failed to create quiz: {response.StatusCode}");
                    Console.WriteLine($"  Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Error creating quiz: {ex.Message}");
            }

            return Guid.NewGuid();
        }

        /// <summary>
        /// Submit quiz để tạo attempt
        /// </summary>
        protected virtual async Task SubmitTestQuiz(Guid quizId)
        {
            try
            {
                Console.WriteLine($"  Submitting quiz {quizId}...");

                // Lấy quiz details để biết questions và answers
                var quizEndpoint = Settings.Endpoints.ContainsKey("Quiz")
                    ? Settings.Endpoints["Quiz"] + $"/{quizId}"
                    : $"/gateway/api/Quiz/{quizId}";

                var quizResponse = await ApiClient.GetAsync(quizEndpoint);

                if (!quizResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"  ✗ Failed to get quiz details");
                    return;
                }

                var quizContent = await quizResponse.Content.ReadAsStringAsync();
                dynamic quizData = JsonConvert.DeserializeObject(quizContent);

                // Submit quiz với correct answers
                var submitRequest = new
                {
                    quizId = quizId,
                    answers = new[]
                    {
                        new
                        {
                            questionId = Guid.Parse(quizData.questions[0].id.ToString()),
                            answerId = Guid.Parse(quizData.questions[0].answers[1].id.ToString()) // Correct answer
                        }
                    }
                };

                var submitEndpoint = Settings.Endpoints.ContainsKey("Quiz")
                    ? Settings.Endpoints["Quiz"] + "/submit"
                    : "/gateway/api/Quiz/submit";

                var submitResponse = await ApiClient.PostAsync(submitEndpoint, submitRequest);

                if (submitResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"  ✓ Quiz submitted successfully");
                }
                else
                {
                    Console.WriteLine($"  ✗ Failed to submit quiz: {submitResponse.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Error submitting quiz: {ex.Message}");
            }
        }

        #endregion

        private async Task SetupCategoryTestData()
        {
            try
            {
                // Tạo một category để test duplicate
                var duplicateCategoryName = await CreateTestCategory("Duplicate Test Category");
                TestDataReplacements["DUPLICATE_CATEGORY_NAME"] = duplicateCategoryName;
                Console.WriteLine($"  ✓ Created duplicate test category: {duplicateCategoryName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Error setting up category test data: {ex.Message}");
            }
        }

        /// <summary>
        /// Tạo một category và return tên của nó
        /// </summary>
        private async Task<string> CreateTestCategory(string name)
        {
            try
            {
                var endpoint = GetEndpoint("Category") + "/create";

                var categoryName = $"{name} {DateTime.Now:yyyyMMddHHmmss}";
                var request = new
                {
                    name = categoryName,
                    description = "Test category for automation testing"
                };

                var response = await ApiClient.PostAsync(endpoint, request);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"    ✓ Created test category: {categoryName}");
                    return categoryName;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"    ⚠ Failed to create category: {response.StatusCode}");
                    Console.WriteLine($"    Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Error creating test category: {ex.Message}");
            }

            // Return a unique name as fallback
            return $"{name} {DateTime.Now:yyyyMMddHHmmss}";
        }

        /// <summary>
        /// Bắt đầu một test case
        /// </summary>
        protected void StartTest(string testName, string category = "General")
        {
            _currentTestName = testName;
            _stopwatch.Restart();
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] ▶ Starting test: {testName}");
        }

        /// <summary>
        /// Ghi lại kết quả test vào CSV
        /// </summary>
        protected void RecordTestResult(
            string status,
            string endpoint = "",
            int statusCode = 0,
            double responseTimeMs = 0,
            string errorMessage = "",
            string requestBody = "",
            string responseBody = "")
        {
            _stopwatch.Stop();

            var result = new TestResult
            {
                TestName = _currentTestName,
                TestCategory = GetTestCategory(),
                Status = status,
                ExecutionTime = DateTime.Now,
                DurationMs = _stopwatch.Elapsed.TotalMilliseconds,
                ErrorMessage = errorMessage,
                ApiEndpoint = endpoint,
                StatusCode = statusCode,
                ResponseTimeMs = responseTimeMs,
                RequestBody = TruncateString(requestBody, 500),
                ResponseBody = TruncateString(responseBody, 500)
            };

            Reporter.AddResult(result);

            var statusIcon = status == "Passed" ? "✓" : status == "Failed" ? "✗" : "⊘";

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {statusIcon} Test {status}: {_currentTestName} ({result.DurationMs:F2}ms)");

            if (!string.IsNullOrEmpty(errorMessage))
            {
                Console.WriteLine($"   Error: {errorMessage}");
            }
        }

        /// <summary>
        /// Xác định category của test dựa trên tên class
        /// </summary>
        private string GetTestCategory()
        {
            var testClass = GetType().Name;

            if (testClass.Contains("Category")) return "Category";
            if (testClass.Contains("Quiz")) return "Quiz";
            if (testClass.Contains("User")) return "User";
            if (testClass.Contains("Profile")) return "Profile";
            if (testClass.Contains("Course")) return "Course";
            if (testClass.Contains("Enrollment")) return "Enrollment";
            if (testClass.Contains("Payment")) return "Payment";

            return "General";
        }

        /// <summary>
        /// Truncate string để tránh CSV quá lớn
        /// </summary>
        private string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }

        /// <summary>
        /// Replace placeholders trong test data với giá trị thật
        /// </summary>
        protected string ReplacePlaceholders(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            foreach (var kvp in TestDataReplacements)
            {
                value = value.Replace(kvp.Key, kvp.Value);
            }

            return value;
        }

        /// <summary>
        /// Parse Guid từ string, return default nếu invalid
        /// </summary>
        protected Guid ParseGuidOrDefault(string value, Guid defaultValue = default)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            // Replace placeholders trước
            value = ReplacePlaceholders(value);

            if (Guid.TryParse(value, out Guid result))
            {
                return result;
            }

            return defaultValue;
        }

        /// <summary>
        /// Lấy endpoint từ config
        /// </summary>
        protected string GetEndpoint(string key)
        {
            if (Settings.Endpoints.TryGetValue(key, out string endpoint))
            {
                return endpoint;
            }

            throw new KeyNotFoundException($"Endpoint '{key}' not found in configuration");
        }

        /// <summary>
        /// Lấy User ID từ JWT token
        /// </summary>
        protected Guid GetUserIdFromToken()
        {
            if (string.IsNullOrEmpty(AuthToken))
                return Guid.Empty;

            try
            {
                // Decode JWT token (simple parsing, không verify signature)
                var parts = AuthToken.Split('.');
                if (parts.Length != 3)
                    return Guid.Empty;

                var payload = parts[1];

                // Add padding if needed
                var mod = payload.Length % 4;
                if (mod != 0)
                {
                    payload += new string('=', 4 - mod);
                }

                var jsonBytes = Convert.FromBase64String(payload);
                var json = System.Text.Encoding.UTF8.GetString(jsonBytes);

                // Parse as JObject instead of dynamic
                var tokenData = Newtonsoft.Json.Linq.JObject.Parse(json);

                // Thử các claim names khác nhau
                string userIdStr = null;

                // Standard claims
                if (tokenData["sub"] != null)
                    userIdStr = tokenData["sub"].ToString();
                else if (tokenData["nameid"] != null)
                    userIdStr = tokenData["nameid"].ToString();
                else if (tokenData["userId"] != null)
                    userIdStr = tokenData["userId"].ToString();
                else if (tokenData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] != null)
                    userIdStr = tokenData["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"].ToString();
                else if (tokenData["unique_name"] != null)
                    userIdStr = tokenData["unique_name"].ToString();

                // Debug: Print all claims
                Console.WriteLine("JWT Token Claims:");
                foreach (var claim in tokenData)
                {
                    Console.WriteLine($"  {claim.Key}: {claim.Value}");
                }

                if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out Guid userId))
                {
                    return userId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing user ID from token: {ex.Message}");
            }

            return Guid.Empty;
        }

        public void Dispose()
        {
            Reporter.GenerateDetailedReport();
            ApiClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}