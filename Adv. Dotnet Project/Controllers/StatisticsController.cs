using GroceryStoreManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroceryStoreManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly GroceryStoreManagementContext _context;

        public StatisticsController(GroceryStoreManagementContext context)
        {
            _context = context;
        }

        [HttpGet("total-customers")]
        public async Task<IActionResult> GetTotalCustomers()
        {
            var count = await _context.Customers.CountAsync(c => c.Role == "User");
            return Ok(count);
        }


        [HttpGet("total-orders")]
        public async Task<IActionResult> GetTotalOrders()
        {
            var count = await _context.Orders.CountAsync();
            return Ok(count);
        }

        [HttpGet("total-products")]
        public async Task<IActionResult> GetTotalProducts()
        {
            var count = await _context.Products.CountAsync();
            return Ok(count);
        }

        [HttpGet("total-categories")]
        public async Task<IActionResult> GetTotalCategories()
        {
            var count = await _context.ProductCategories.CountAsync();
            return Ok(count);
        }
        [HttpGet("recent-orders")]
        public async Task<IActionResult> GetRecentOrders()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .Select(o => new
                {
                    o.OrderId,
                    o.CustomerId,
                    CustomerName = o.Customer.CustomerName,  // Assuming navigation property
                    o.OrderDate,
                    o.TotalAmount
                })
                .ToListAsync();

            return Ok(orders);
        }
        [HttpGet("monthly-sales")]
        public async Task<IActionResult> GetMonthlySales()
        {
            var sales = await _context.Orders
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    TotalSales = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToListAsync();

            return Ok(sales);
        }


    }

}
