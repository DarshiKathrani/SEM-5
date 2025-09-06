using GroceryMvc.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryMvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly AuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(AuthService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string CustomerName, string Password)
        {
            var jsonData = await _authService.AuthenticateUserAsync(CustomerName, Password);

            if (jsonData == null)
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }

            var data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(jsonData);
            string token = data["token"];
            string role = data["user"]["role"];
            string customerId = data["user"]["customerId"].ToString();

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Invalid credentials.";
                return View();
            }

            // ✅ Save to session
            HttpContext.Session.SetString("JWTToken", token);
            HttpContext.Session.SetString("UserRole", role);
            HttpContext.Session.SetString("CustomerId", customerId);

            // ✅ Redirect based on Role
            if (role == "Admin")
                return RedirectToAction("Index", "Home");  // goes to AdminController → Dashboard
            else if (role == "User")
                return RedirectToAction("Index", "UserHome");   // goes to UserController → Dashboard
            else
                return RedirectToAction("AccessDenied", "Login");       // fallback
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string CustomerName, string Email, string Phone, string Address, string Password, string ConfirmPassword, string Role)
        {
            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View();
            }

            var newCustomer = new
            {
                CustomerName,
                Email,
                Phone,
                Address,
                Password,
                Role
            };

            var result = await _authService.RegisterUserAsync(newCustomer);

            if (string.IsNullOrEmpty(result))
            {
                ViewBag.Error = "Registration failed. Try again.";
                return View();
            }

            // Auto login after registration
            var loginData = await _authService.AuthenticateUserAsync(CustomerName, Password);

            if (loginData == null)
            {
                ViewBag.Error = "Auto-login failed. Please login manually.";
                return RedirectToAction("Login");
            }

            var data = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(loginData);
            string token = data["token"];
            string role = data["user"]["role"];
            string customerId = data["user"]["customerId"].ToString();

            HttpContext.Session.SetString("JWTToken", token);
            HttpContext.Session.SetString("UserRole", role);
            HttpContext.Session.SetString("CustomerId", customerId);

            if (role == "Admin")
                return RedirectToAction("Index", "Home");
            else if (role == "User")
                return RedirectToAction("Index", "UserHome");
            else
                return RedirectToAction("AccessDenied", "Login");
        }


        public IActionResult Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
