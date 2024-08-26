using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CompanyClient.Models;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace CompanyClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient("CompanyApiClient");
            AttachAuthTokenToClient(client);
            return client;
        }

        public async Task<IActionResult> Index(string searchType, string searchValue)
        {
            // Check for authentication token
            var token = HttpContext.Request.Cookies["AuthToken"];
            if (string.IsNullOrEmpty(token))
            {
                // Redirect to login page if not authenticated
                return RedirectToAction("Login", "Account");
            }

            IEnumerable<Company> companies = Enumerable.Empty<Company>();
            var client = CreateClient();

            try
            {
                if (string.IsNullOrEmpty(searchValue))
                {
                    // Fetch all companies if no search value is provided
                    var response = await client.GetStringAsync("Companies");
                    companies = JsonSerializer.Deserialize<IEnumerable<Company>>(response, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })!;
                }
                else
                {
                    // Fetch filtered companies
                    if (searchType == "Id" && int.TryParse(searchValue, out int id))
                    {
                        var response = await client.GetStringAsync($"companies/{id}");
                        var company = JsonSerializer.Deserialize<Company>(response, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })!;
                        companies = company != null ? new List<Company> { company } : Enumerable.Empty<Company>();
                    }
                    else if (searchType == "Isin")
                    {
                        var response = await client.GetStringAsync($"companies/isin/{searchValue}");
                        var company = JsonSerializer.Deserialize<Company>(response, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        })!;
                        companies = company != null ? new List<Company> { company } : Enumerable.Empty<Company>();
                    }
                    else
                    {
                        companies = Enumerable.Empty<Company>();
                    }
                }
            }
            catch (HttpRequestException)
            {
                companies = Enumerable.Empty<Company>();
            }

            ViewData["SearchType"] = searchType;
            return View(companies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = CreateClient();

            try
            {
                var response = await client.GetStringAsync($"companies/{id}");
                var company = JsonSerializer.Deserialize<Company>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                })!;

                if (company == null)
                {
                    return NotFound();
                }

                return View(company);
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching data");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Company company)
        {
            if (ModelState.IsValid)
            {
                var client = CreateClient();

                var response = await client.PostAsJsonAsync("companies", company);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "An error occurred while creating the company.");
            }
            return View(company);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string GetAuthToken()
        {
            return HttpContext.Request.Cookies["AuthToken"]!;
        }

        private void AttachAuthTokenToClient(HttpClient client)
        {
            var token = GetAuthToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
