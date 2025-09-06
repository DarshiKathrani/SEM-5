using System.Text;
using GroceryMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryMvc.Controllers
{
    public class ProductCategoryController : Controller
    {
        private readonly HttpClient _client;

        public ProductCategoryController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5134/api/ProductCategoryAPI/");
        }
        public async Task<IActionResult> CategoryList()
        {
            var response = await _client.GetAsync("");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ProductCategoryModel>>(json);
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

            return RedirectToAction("CategoryList");
        }
        public async Task<IActionResult> AddEdit(int? id)
        {
            // Fetch category dropdown
            //var categoryResponse = await _client.GetAsync("dropdown/productCategories");
            //var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
            //var categories = JsonConvert.DeserializeObject<List<CategoryDropDownModel>>(categoryJson);

            ProductCategoryModel category;

            if (id == null)
            {
                category = new ProductCategoryModel();
            }
            else
            {
                var response = await _client.GetAsync($"{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                category = JsonConvert.DeserializeObject<ProductCategoryModel>(json);
            }
            //category.CategoryList = categories;
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(ProductCategoryModel category)
        {
            //if (!ModelState.IsValid)
            //{
            //    // Reload country list since it won't be preserved across postbacks
            //    var categoryResponse = await _client.GetAsync("dropdown/productCategories");
            //    var categoryJson = await categoryResponse.Content.ReadAsStringAsync();
            //    category.CategoryList = JsonConvert.DeserializeObject<List<CategoryDropDownModel>>(categoryJson);

            //    return View(category); // Return view with validation errors
            //}

            if (category.CategoryID == 0)
            {
                var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                await _client.PostAsync("", content);
            }
            else
            {

                var content = new StringContent(JsonConvert.SerializeObject(category), Encoding.UTF8, "application/json");
                await _client.PutAsync($"{category.CategoryID}", content);
            }

            return RedirectToAction("CategoryList");
        }
    }
}