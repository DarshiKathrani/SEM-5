using GroceryStoreManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GroceryStoreManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailsAPIController : ControllerBase
    {
        private readonly GroceryStoreManagementContext context;

        public OrderDetailsAPIController(GroceryStoreManagementContext context)
        {
            this.context = context;
        }

        #region GetAllOrderDetails
        [HttpGet]
        public IActionResult GetAllOrderDetails()
        {
            var orderDetails = context.OrderDetails.ToList();
            return Ok(orderDetails);
        }
        #endregion

        #region GetOrderDetailByID
        [HttpGet("{id}")]
        public IActionResult GetOrderDetailById(int id)
        {
            var orderDetail = context.OrderDetails.Find(id);
            if (orderDetail == null)
            {
                return NotFound();
            }
            return Ok(orderDetail);
        }

        #endregion

        #region DeleteOrderDetailById
        [HttpDelete("{id}")]
        public IActionResult DeleteOrderDetailById(int id)
        {
            var orderDetails = context.OrderDetails.Find(id);
            if (orderDetails == null)
            {
                return NotFound();
            }
            context.OrderDetails.Remove(orderDetails);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertOrderDetail
        [HttpPost]
        public IActionResult InsertOrderDetail(OrderDetail orderDetail)
        {
            context.OrderDetails.Add(orderDetail);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region UpdateOrderDetail
        [HttpPut("{id}")]
        public IActionResult UpdateOrderDetail(int id, OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailsId)
            {
                return BadRequest();
            }
            var existingOrderDetail = context.OrderDetails.Find(id);
            if (existingOrderDetail == null)
            {
                return NotFound();
            }
            existingOrderDetail.OrderId= orderDetail.OrderId;
            existingOrderDetail.ProductId = orderDetail.ProductId;
            existingOrderDetail.Quantity = orderDetail.Quantity;
            existingOrderDetail.Status = orderDetail.Status;
            existingOrderDetail.Order = orderDetail.Order;
            existingOrderDetail.Product = orderDetail.Product;
            context.OrderDetails.Update(existingOrderDetail);
            context.SaveChanges();
            return NoContent();
        }
        #endregion
    }
}
