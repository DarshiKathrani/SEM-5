using GroceryMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryMvc.Controllers
{
    public class OrderDetailController : Controller
    {
        private readonly HttpClient _client;
        public OrderDetailController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5134/api/OrderDetailsAPI/");
        }
        public async Task<IActionResult> OrderDetailsList()
        {
            var response = await _client.GetAsync("");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<OrderDetailModel>>(json);
            return View(list);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _client.DeleteAsync($"{id}");
            var message = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Order Detail deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Delete failed: {message}";
            }

            return RedirectToAction("OrderDetailsList");
        }
    }
}
