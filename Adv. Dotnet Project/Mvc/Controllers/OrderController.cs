using System.Text;
using GroceryMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GroceryMvc.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient _client;
        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5134/api/OrderAPI/");
        }
        public async Task<IActionResult> OrderList()
        {
            var response = await _client.GetAsync("");
            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<OrderModel>>(json);
            return View(list);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _client.DeleteAsync($"{id}");
            var message = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Order deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = $"Delete failed: {message}";
            }

            return RedirectToAction("OrderList");
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            // Fetch customer dropdown
            var customerResponse = await _client.GetAsync("dropdown/customers");
            var customerJson = await customerResponse.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<List<CustomerDropDownModel>>(customerJson);

            OrderModel order;

            if (id == null)
            {
                order = new OrderModel();
            }
            else
            {
                var response = await _client.GetAsync($"{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                order = JsonConvert.DeserializeObject<OrderModel>(json);
            }
            order.CustomerList = customers;
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(OrderModel order)
        {
            if (!ModelState.IsValid)
            {
                //    // Reload country list since it won't be preserved across postbacks
                var customerResponse = await _client.GetAsync("dropdown/customers");
                var customerJson = await customerResponse.Content.ReadAsStringAsync();
                order.CustomerList = JsonConvert.DeserializeObject<List<CustomerDropDownModel>>(customerJson);

                return View(order); // Return view with validation errors
            }

            if (order.OrderID == 0)
            {
                var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
                await _client.PostAsync("", content);
            }
            else
            {

                var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
                await _client.PutAsync($"{order.OrderID}", content);
            }

            return RedirectToAction("OrderList");
        }
    }
}
