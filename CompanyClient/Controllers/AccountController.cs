using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CompanyClient.Services;
using CompanyClient.Models;
using Microsoft.AspNetCore.Authorization;

namespace CompanyClient.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;

        public AccountController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            try
            {
                var token = await _authService.LoginAsync(model);
                if (string.IsNullOrEmpty(token))
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }

                // Save the token in a secure place, such as a cookie
                HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true, // Prevent client-side script access
                    Secure = true, // Use HTTPS
                    SameSite = SameSiteMode.Strict, // Prevent CSRF attacks
                    Expires = DateTimeOffset.UtcNow.AddHours(1) // Set expiration time
                });

                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
                return View(model);
            }

            try
            {
                await _authService.RegisterAsync(model);
                return RedirectToAction("Login");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Login");
        }
    }
 }
