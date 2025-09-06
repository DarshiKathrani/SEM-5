using GroceryMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroceryMvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
    

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _client = httpClientFactory.CreateClient();
            _client.BaseAddress = new Uri("http://localhost:5134/api/Statistics/");
            _configuration = configuration;   
        }

        public async Task<IActionResult> Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
                return RedirectToAction("AccessDenied", "Login");

            int totalCustomers = await GetCountFromApi("total-customers");
            int totalOrders = await GetCountFromApi("total-orders");
            int totalCategories = await GetCountFromApi("total-categories");
            int totalProducts = await GetCountFromApi("total-products");

            var response = await _client.GetAsync("monthly-sales");
            List<MonthlySalesDto> monthlySales = new List<MonthlySalesDto>();
            if (response.IsSuccessStatusCode)
            {
                monthlySales = await response.Content.ReadFromJsonAsync<List<MonthlySalesDto>>();
            }

            
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalCategories = totalCategories;
            ViewBag.TotalProducts = totalProducts;
            ViewBag.MonthlySales = monthlySales ?? new List<MonthlySalesDto>();

         
            var recentOrders = await GetListFromApi<OrderModel>("recent-orders");
            ViewBag.RecentOrders = recentOrders;

           
            List<TopProductDto> topProducts = new List<TopProductDto>();
            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = @"
                    SELECT TOP 5 p.ProductName, SUM(od.Quantity) AS TotalQuantity
                    FROM OrderDetails od
                    INNER JOIN Products p ON od.ProductID = p.ProductID
                    GROUP BY p.ProductName
                    ORDER BY TotalQuantity DESC;";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    topProducts.Add(new TopProductDto
                    {
                        ProductName = rdr["ProductName"].ToString(),
                        TotalQuantity = Convert.ToInt32(rdr["TotalQuantity"])
                    });
                }
            }

            ViewBag.TopProducts = topProducts;

            List<LowStockProductDto> lowStockProducts = new List<LowStockProductDto>();

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                string query = @"
        SELECT TOP 5 
            p.ProductName, 
            p.RemainingStock AS StockQuantity
        FROM Products p
        WHERE p.RemainingStock < @Threshold
        ORDER BY p.RemainingStock ASC;";

                using var cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Threshold", 10); // show products below 10 stock

                conn.Open();
                using var rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    lowStockProducts.Add(new LowStockProductDto
                    {
                        ProductName = rdr["ProductName"].ToString(),
                        StockQuantity = Convert.ToInt32(rdr["StockQuantity"])
                    });
                }
            }

            ViewBag.LowStockProducts = lowStockProducts;


            return View();
        }

        
        private async Task<int> GetCountFromApi(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var countString = await response.Content.ReadAsStringAsync();
                if (int.TryParse(countString, out int count))
                    return count;
            }
            return 0;
        }

        private async Task<List<T>> GetListFromApi<T>(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<T>();
            }
            return new List<T>();
        }
    }
}
