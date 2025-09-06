using System.ComponentModel.DataAnnotations;
using GroceryStoreManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace GroceryStoreManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryAPIController : ControllerBase
    {
        private readonly GroceryStoreManagementContext context;
        private readonly IValidator<ProductCategory> validator;

        public ProductCategoryAPIController(GroceryStoreManagementContext context,IValidator<ProductCategory> validator)
        {
            this.context = context;
            this.validator = validator;
        }
        #region GetAllProductCategories
        [HttpGet]
        public IActionResult GetAllProductCategories()
        {
            var productCategories = context.ProductCategories.ToList();
            return Ok(productCategories);
        }
        #endregion

        #region GetProductCategoryByID
        [HttpGet("{id}")]
        public IActionResult GetProductCategoryById(int id)
        {
            var productCategory = context.ProductCategories.Find(id);
            if (productCategory == null)
            {
                return NotFound();
            }
            return Ok(productCategory);
        }

        #endregion

        #region DeleteProductCategoryById
        [HttpDelete("{id}")]
        public IActionResult DeleteProductCategoryById(int id)
        {
            var productCategories = context.ProductCategories.Find(id);
            if (productCategories == null)
            {
                return NotFound();
            }
            context.ProductCategories.Remove(productCategories);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region InsertProductCategory
        [HttpPost]
        public async Task<IActionResult> InsertProductCategory([FromBody] ProductCategory category)
        {
            var validationResult = await validator.ValidateAsync(category);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => new
                {
                    Property = e.PropertyName,
                    Error = e.ErrorMessage
                }));
            }

            context.ProductCategories.Add(category);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductCategoryById), new { id = category.CategoryId }, category);
        }
        #endregion

        #region UpdateProductCategory
        [HttpPut("{id}")]
        public IActionResult UpdateProductCategory(int id, ProductCategory productCategory)
        {
            if (id != productCategory.CategoryId)
            {
                return BadRequest();
            }
            var existingCategory = context.ProductCategories.Find(id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            existingCategory.CategoryName = productCategory.CategoryName;
            context.ProductCategories.Update(existingCategory);
            context.SaveChanges();
            return NoContent();
        }
        #endregion

        #region SearchProductCategory
        // Get product categories by filters (CustomerId)
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> Filter([FromQuery] int? categoryId)
        {
            var query = context.ProductCategories.AsQueryable();
            if (categoryId.HasValue)
                query = query.Where(c => c.CategoryId == categoryId);

            return await query.ToListAsync();
        }

        #endregion

        #region ProductCategoryDropdown
        // Get all product categories (for dropdown)
        [HttpGet("dropdown/productCategories")]
        public async Task<ActionResult<IEnumerable<object>>> GetProductCategories()
        {
            return await context.ProductCategories
                .Select(c => new { c.CategoryId, c.CategoryName })
                .ToListAsync();
        }
        #endregion

        #region TopCategories
        // Get top 3 Categories
        [HttpGet("Top3")]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetTop3()
        {
            return await context.ProductCategories
                .Take(3)
                .ToListAsync();
        }
        #endregion
    }
}