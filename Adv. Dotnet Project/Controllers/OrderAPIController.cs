using GroceryStoreManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroceryStoreManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly GroceryStoreManagementContext context;

        public OrderAPIController(GroceryStoreManagementContext context) { 
            this.context = context;
        }
        #region GetAllOrders
        [HttpGet]
        public IActionResult GetAllorders()
        {
            var orders = context.Orders
     .Include(o=> o.Customer) // eager load Category
     .Select(o => new Order
     {
         OrderId = o.OrderId,
         CustomerName=o.Customer!=null ? o.Customer.CustomerName : string.Empty,
         OrderDate = o.OrderDate,
         TotalAmount = o.TotalAmount
         
         //Price = p.Price,
         //UsedStock = p.UsedStock,
         //RemainingStock = p.RemainingStock,
         //CategoryName = p.Category != null ? p.Category.CategoryName : string.Empty
     })
     .ToList();

            return Ok(orders);
        }
        #endregion

        #region GetOrderByID
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            var order = context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        #endregion

        #region DeleteOrderById
        [HttpDelete("{id}")]
        public IActionResult DeleteOrderById(int id)
        {
            var orders = context.Orders.Find(id);
            if (orders == null)
            {
                return NotFound();
            }
            context.Orders.Remove(orders);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region Insertorder
        [HttpPost]
        public IActionResult InsertOrder(Order order)
        {
            context.Orders.Add(order);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region UpdateOrder
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }
            var existingOrder = context.Orders.Find(id);
            if (existingOrder == null)
            {
                return NotFound();
            }
            existingOrder.CustomerId = order.CustomerId;
            existingOrder.OrderDate = order.OrderDate;
            existingOrder.TotalAmount = order.TotalAmount;
            existingOrder.Customer = order.Customer;
            existingOrder.OrderDetails = order.OrderDetails;
            context.Orders.Update(existingOrder);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region SearchOrders
        // Get orders by filters (OrderId)
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Order>>> Filter([FromQuery] int? orderId)
        {
            var query = context.Orders.AsQueryable();
            if (orderId.HasValue)
                query = query.Where(c => c.OrderId == orderId);

            return await query.ToListAsync();
        }

        #endregion

        #region OrdersCascadeDropdown
        // Get orders by customers (for cascading)
        [HttpGet("dropdown/orders/customerId")]
        public async Task<ActionResult<IEnumerable<object>>> GetOrdersByCustomers(int customerId)
        {
            return await context.Orders
                .Where(s => s.CustomerId == customerId
                )
                .Select(s => new { s.OrderId, s.OrderDetails })
                .ToListAsync();
        }
        #endregion

        #region TopOrders
        // Get top 3 orders
        [HttpGet("Top3")]
        public async Task<ActionResult<IEnumerable<Order>>> GetTop3()
        {
            return await context.Orders
                .Include(c => c.Customer)
                .Take(3)
                .ToListAsync();
        }

        #endregion

        #region CustomerDropDown
        // Get all customers (for dropdown)
        [HttpGet("dropdown/customers")]
        public async Task<ActionResult<IEnumerable<object>>> GetCustomers()
        {
            return await context.Customers
                .Select(c => new { c.CustomerId, c.CustomerName })
                .ToListAsync();
        }
        #endregion
    }
}