using GroceryStoreManagementSystem.Models;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GroceryStoreManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly GroceryStoreManagementContext context;
        private readonly IValidator<Product> validator;
        public ProductAPIController(GroceryStoreManagementContext context, IValidator<Product> validator)
        {
            this.context = context;
            this.validator = validator;
        }
        #region GetAllProducts
        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = context.Products
     .Include(p => p.Category) // eager load Category
     .Select(p => new Product
     {
         ProductId = p.ProductId,
         ProductName = p.ProductName,
         CategoryId = p.CategoryId,
         Price = p.Price,
         UsedStock = p.UsedStock,
         RemainingStock = p.RemainingStock,
         CategoryName = p.Category != null ? p.Category.CategoryName : string.Empty
     })
     .ToList();

            return Ok(products);

        }
        #endregion

        #region GetProductByID
        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        #endregion

        #region DeleteProductById
        [HttpDelete("{id}")]
        public IActionResult DeleteProductById(int id)
        {
            var product= context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            context.Products.Remove(product);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertProduct
        [HttpPost]
        public async Task<IActionResult> InsertProduct([FromBody] Product product)
        {
            var validationResult = await validator.ValidateAsync(product);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            context.Products.Add(product);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }
        #endregion

        #region UpdateProduct
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, Product product)
        {
            if (id != product.ProductId)
            {
                return BadRequest();
            }
            var existingProduct = context.Products.Find(id);
            if (existingProduct == null)
            {
                return NotFound();
            }
            existingProduct.ProductName = product.ProductName;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.Price = product.Price;
            existingProduct.UsedStock = product.UsedStock;
            existingProduct.RemainingStock = product.RemainingStock;
            existingProduct.Category = product.Category;
            existingProduct.OrderDetails = product.OrderDetails;
            context.Products.Update(existingProduct);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region Searchproduct
        // Get products by filters (ProductId and CategoryId)
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Product>>> Filter([FromQuery] int? productId, [FromQuery] int? categoryId)
        {
            var query = context.Products.AsQueryable();
            if (productId.HasValue)
                query = query.Where(c => c.ProductId == productId);

            if (categoryId.HasValue)
                query = query.Where(c => c.CategoryId == categoryId);

            return await query.ToListAsync();
        }

        #endregion

        #region ProductCascadeDropdown
        // Get products by category (for cascading)
        [HttpGet("dropdown/products/categoryId")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductsByCategory(int categoryId)
        {
            return await context.Products
                .Where(s => s.CategoryId == categoryId
                )
                .Select(s => new { s.ProductId, s.ProductName })
                .ToListAsync();
        }
        #endregion

        #region TopProducts
        // Get top 3 products
        [HttpGet("Top3")]
        public async Task<ActionResult<IEnumerable<Product>>> GetTop3()
        {
            return await context.Products
                .Include(c => c.Category)
                .Take(3)
                .ToListAsync();
        }

        #endregion

        [HttpGet("dropdown")]
        public async Task<ActionResult<IEnumerable<object>>> GetDropdown()
        {
            try
            {
                var categories = await context.ProductCategories
                    .Select(c => new { c.CategoryId, c.CategoryName })
                    .ToListAsync();

                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts(string? search, string? category)
        {
            var products = this.context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.ProductName.Contains(search));
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category.CategoryName.Contains(category));
            }

            return await products.ToListAsync();
        }

    }
}
