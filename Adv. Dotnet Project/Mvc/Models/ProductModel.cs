using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GroceryMvc.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Used stock is required.")]
        public int UsedStock { get; set; }

        [Required(ErrorMessage = "Remaining stock is required.")]
        public int RemainingStock { get; set; }
        
        public List<CategoryDropDownModel>? CategoryList { get; set; }

        public string? CategoryName { get; set; } 
    }


    public class CategoryDropDownModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}
