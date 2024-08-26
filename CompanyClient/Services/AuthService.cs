using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CompanyClient.Models;

namespace CompanyClient.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            var jsonContent = JsonSerializer.Serialize(loginModel);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                return tokenResponse?.token;
            }

            throw new HttpRequestException("Invalid login attempt.");
        }

        public async Task RegisterAsync(RegisterModel registerModel)
        {
            var jsonContent = JsonSerializer.Serialize(registerModel);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("auth/register", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Registration failed: {errorContent}");
            }
        }
    }
}
