using System.Text;
using GroceryMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryMvc.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _client;
        public ProductController(IHttpClientFactory httpClientFactory) { 
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5134/api/ProductAPI/");
        }
        //private void AddJwtToken()
        //{
        //    var token = HttpContext.Session.GetString("JWTToken");
        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        _client.DefaultRequestHeaders.Authorization =
        //            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        //    }
        //}

        public async Task<IActionResult> ProductList()
        {
            var response = await _client.GetAsync("");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ProductModel>>(json);
            return View(list);
        }
        public async Task<IActionResult> Delete(int id)
        {
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

            return RedirectToAction("ProductList");
        }
        public async Task<IActionResult> AddEdit(int? id)
        {
            // Fetch category dropdown
            var categoryResponse = await _client.GetAsync("dropdown");
            var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<CategoryDropDownModel>>(categoryJson);

            ProductModel product;

            if (id == null)
            {
                product = new ProductModel();
            }
            else
            {
                var response = await _client.GetAsync($"{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                product = JsonConvert.DeserializeObject<ProductModel>(json);
            }
            product.CategoryList = categories;
            return View(product);
        }

       
        [HttpPost]
        public async Task<IActionResult> AddEdit(ProductModel state)
        {
            if (!ModelState.IsValid)
            {
                var categoryResponse = await _client.GetAsync("dropdown");
                var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
                state.CategoryList = JsonConvert.DeserializeObject<List<CategoryDropDownModel>>(categoryJson);
                return View(state);
            }

            var rawJson = JsonConvert.SerializeObject(state);
            Console.WriteLine("🔍 Sending JSON:");
            Console.WriteLine(rawJson);

            var content = new StringContent(rawJson, Encoding.UTF8, "application/json");

            HttpResponseMessage result;

            if (state.ProductID == 0)
            {
                result = await _client.PostAsync("", content);
            }
            else
            {
                result = await _client.PutAsync($"{state.ProductID}", content);
            }

            if (result != null && result.IsSuccessStatusCode)
            {
                return RedirectToAction("ProductList");
            }

            string apiResponse = await result.Content.ReadAsStringAsync();
            Console.WriteLine("❌ API Error Response:");
            Console.WriteLine(apiResponse);

            ModelState.AddModelError("", "Error saving product: " + apiResponse);

            var dropdownResponse = await _client.GetAsync("dropdown");
            var dropdownJson = await dropdownResponse.Content.ReadAsStringAsync();
            state.CategoryList = JsonConvert.DeserializeObject<List<CategoryDropDownModel>>(dropdownJson);
            return View(state);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string searchTerm, string category)
        {
            //AddJwtToken(); // attach JWT

            // Build the correct API URL
            var apiUrl = $"search?search={searchTerm}&category={category}";
            var response = await _client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Failed to load products" });
            }

            var json = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<ProductModel>>(json);

            return Json(new { success = true, data = products });
        }



    }
}