using Microsoft.AspNetCore.Mvc;
using CarRentalManagement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRepository;

namespace CarRentalManagement.Controllers
{
    public class AdminController : Controller
    {
        private readonly IGenericRepository<Admin> _genericRepository;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger, IGenericRepository<Admin> genericRepository)
        {
            _logger = logger;
            _genericRepository = genericRepository;
            _logger.LogInformation("AdminController initialized.");
        }

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("Login (GET) action called.");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            _logger.LogInformation("Register (GET) action called.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Registration registrationModel)
        {
            try
            {
                _logger.LogInformation("Register (POST) action called.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Register (POST) action failed: Model validation failed.");
                    return View(registrationModel);
                }

                var existingAdmin = (await _genericRepository.GetAll())
                    .FirstOrDefault(a => a.Username == registrationModel.Username);

                if (existingAdmin != null)
                {
                    _logger.LogWarning("Register (POST) action failed: Username {Username} already exists.", registrationModel.Username);
                    ModelState.AddModelError("Username", "This username is already taken.");
                    return View(registrationModel);
                }

                var admin = new Admin
                {
                    Username = registrationModel.Username,
                    Password = registrationModel.Password,
                    PhoneNumber = registrationModel.PhoneNumber,
                };

                await _genericRepository.Register(admin);
                _logger.LogInformation("Register (POST) action succeeded: Admin {Username} registered successfully.", registrationModel.Username);

                TempData["SuccessMessage"] = "Registration successful. Please log in.";
                return RedirectToAction(nameof(Login));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Register (POST) action failed with an exception.");
                ViewBag.ErrorMessage = "An error occurred while processing your request. Please try again later.";
                return View(registrationModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                _logger.LogInformation("Login (POST) action called: Attempting login for Username {Username}.", username);

                var admin = (await _genericRepository.GetAll())
                    .FirstOrDefault(a => a.Username == username && a.Password == password);

                if (admin != null)
                {
                    var authToken = GenerateAuthToken(admin.Username);
                    HttpContext.Session.SetString("Username", username);
                    HttpContext.Session.SetString("AdminName", admin.Username);
                    HttpContext.Session.SetString("Role", "Admin");

                    Response.Cookies.Append("AdminUsername", admin.Username, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.Now.AddHours(1)
                    });

                    Response.Cookies.Append("AdminAuthToken", authToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.Now.AddHours(1)
                    });

                    TempData["AdminLoggedIn"] = true;
                    _logger.LogInformation("Login (POST) action succeeded: Admin {Username} logged in successfully.", username);

                    return RedirectToAction(nameof(AdminDashboard));
                }

                _logger.LogWarning("Login (POST) action failed: Invalid credentials for Username {Username}.", username);
                ViewBag.ErrorMessage = "Invalid Username or Password.";
                return View();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Login (POST) action failed with an exception.");
                ViewBag.ErrorMessage = "An error occurred while processing your request. Please try again later.";
                return View();
            }
        }

        public IActionResult AdminDashboard()
        {
            try
            {
                _logger.LogInformation("AdminDashboard action called.");

                var adminName = HttpContext.Session.GetString("AdminName");
                if (string.IsNullOrEmpty(adminName))
                {
                    _logger.LogWarning("AdminDashboard action failed: Admin not logged in.");
                    return RedirectToAction(nameof(Login));
                }

                ViewBag.AdminName = adminName;
                return View();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "AdminDashboard action failed with an exception.");
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";
                return RedirectToAction(nameof(Login));
            }
        }

        public IActionResult Logout()
        {
            try
            {
                _logger.LogInformation("Logout action called: Logging out user/admin.");
                HttpContext.Session.Clear();
                Response.Cookies.Delete("AdminUsername");
                Response.Cookies.Delete("AdminAuthToken");

                _logger.LogInformation("Logout action succeeded: User/Admin logged out successfully.");
                return RedirectToAction(nameof(Login));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Logout action failed with an exception.");
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";
                return RedirectToAction(nameof(Login));
            }
        }

        private string GenerateAuthToken(string username)
        {
            try
            {
                _logger.LogInformation("Generating auth token for Username {Username}.", username);
                var expirationTime = DateTime.UtcNow.AddHours(1);
                var token = $"{username}-{DateTime.UtcNow.Ticks}-{expirationTime}";
                var tokenBytes = Encoding.UTF8.GetBytes(token);
                return Convert.ToBase64String(tokenBytes);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to generate auth token for Username {Username}.", username);
                throw;
            }
        }
    }
}
