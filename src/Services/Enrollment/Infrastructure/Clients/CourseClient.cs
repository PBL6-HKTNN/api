using System.Net.Http.Json;
using Codemy.Enrollment.Application.DTOs;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Codemy.Enrollment.Infrastructure.Clients
{
    public class CourseClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CourseClient(
            HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<CourseDto>> GetCoursesByIdsAsync(List<Guid> ids)
        {
            var courses = new List<CourseDto>();

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var tasks = ids.Select(async id =>
            {
                try
                {
                    var response = await _httpClient.GetAsync($"api/course/get/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var envelope = await response.Content.ReadFromJsonAsync<CourseApiResponse>(options);
                    return envelope?.Data;
                }
                catch
                {
                    return null;
                }
            });

            var results = await Task.WhenAll(tasks);
            courses.AddRange(results.Where(c => c != null)!);

            return courses;
        }
    }
}   
