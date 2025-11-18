using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Codemy.BuildingBlocks.Test
{
    public class ApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private string _token;

        public ApiClient(string baseUrl, int timeoutSeconds = 30, bool ignoreSslErrors = false)
        {
            _baseUrl = baseUrl.TrimEnd('/');

            var handler = new HttpClientHandler();

            // Bỏ qua SSL certificate validation nếu cần (cho test environment)
            if (ignoreSslErrors)
            {
                handler.ServerCertificateCustomValidationCallback =
                    (message, cert, chain, errors) => true;
            }

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_baseUrl),
                Timeout = TimeSpan.FromSeconds(timeoutSeconds)
            };

            // Set default headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void SetAuthToken(string token)
        {
            _token = token;

            // ✅ FIX: Handle null token (for unauthorized tests)
            if (string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string endpoint)
        {
            Console.WriteLine($"GET: {_baseUrl}{endpoint}");
            return await _httpClient.GetAsync(endpoint);
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"POST: {_baseUrl}{endpoint}");
            return await _httpClient.PostAsync(endpoint, content);
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(
            string endpoint, TRequest data)
        {
            var response = await PostAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            Console.WriteLine($"PUT: {_baseUrl}{endpoint}");
            return await _httpClient.PutAsync(endpoint, content);
        }

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var response = await PutAsync(endpoint, data);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
        {
            Console.WriteLine($"DELETE: {_baseUrl}{endpoint}");
            return await _httpClient.DeleteAsync(endpoint);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}