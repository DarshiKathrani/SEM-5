using System.Text;
using GroceryMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace GroceryMvc.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HttpClient _client;

        public CustomerController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5134/api/CustomerAPI/");
        }
        private void AddJwtToken()
        {
            var token = HttpContext.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
                return RedirectToAction("AccessDenied", "Login");

            AddJwtToken(); // attach token from Session

            HttpResponseMessage response;

            if (role == "Admin")
            {
                // ✅ Call paged API for admin
                response = await _client.GetAsync($"paged?page={page}&pageSize={pageSize}");
            }
            else
            {
                var customerId = HttpContext.Session.GetString("CustomerId");
                response = await _client.GetAsync($"{customerId}");
            }

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Login");
            }

            var json = await response.Content.ReadAsStringAsync();

            if (role == "Admin")
            {
                //  Expecting paged response
                var result = JsonConvert.DeserializeObject<PagedResult<CustomerModel>>(json);

                // filter users only
                result.Data = result.Data.Where(c => c.Role == "User").ToList();

                ViewBag.Page = result.Page;
                ViewBag.TotalPages = result.TotalPages;
                ViewBag.PageSize = result.PageSize;

                return View(result.Data);
            }
            else
            {
                // normal single customer response
                var customer = JsonConvert.DeserializeObject<CustomerModel>(json);
                return View(new List<CustomerModel> { customer });
            }
        }

        // helper class
        public class PagedResult<T>
        {
            public List<T> Data { get; set; }
            public int TotalCount { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalPages { get; set; }
        }


        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
                return RedirectToAction("AccessDenied", "Login");

            var response = await _client.DeleteAsync($"{id}");
            var message = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Customer deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Delete failed: {message}";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            //// Fetch country dropdown
            //var countryResponse = await _client.GetAsync("State/dropdown");
            //var countryJson = await countryResponse.Content.ReadAsStringAsync();
            //var countries = JsonConvert.DeserializeObject<List<CountryDropDownModel>>(countryJson);
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
                return RedirectToAction("AccessDenied", "Login");

            CustomerModel customer;

            if (id == null)
            {
                customer = new CustomerModel();
            }
            else
            {
                var response = await _client.GetAsync($"{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                customer = JsonConvert.DeserializeObject<CustomerModel>(json);
            }
            return View(customer);
        }
      
        [HttpPost]
        public async Task<IActionResult> AddEdit(CustomerModel customer)
        {
            // Always enforce Role = User
            customer.Role = "User";

            var role = HttpContext.Session.GetString("UserRole");

            // Only Admins allowed
            if (role != "Admin")
                return RedirectToAction("AccessDenied", "Login");

            if (!ModelState.IsValid)
            {
                return View(customer); // return form with errors
            }

            // Attach JWT token
            AddJwtToken();

            var content = new StringContent(JsonConvert.SerializeObject(customer), Encoding.UTF8, "application/json");
            HttpResponseMessage response;

            if (customer.CustomerID == 0)
            {
                // Create new customer
                response = await _client.PostAsync("", content);
            }
            else
            {
                // Update existing customer
                response = await _client.PutAsync($"{customer.CustomerID}", content);
            }

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                // 👇 Capture API error response for debugging
                var error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"API Error: {response.StatusCode} - {error}");
                return View(customer);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm, string address)
        {
            AddJwtToken(); // attach JWT from Session

            // Build the correct API URL
            var apiUrl = $"http://localhost:5134/api/CustomerAPI?search={searchTerm}&address={address}";

            var response = await _client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Failed to load customers" });
            }

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<CustomerModel>>(json);
            list = list.Where(c => c.Role == "User").ToList();
            return Json(new { success = true, data = list });
        }
    }
}
    