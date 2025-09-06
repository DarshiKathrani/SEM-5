using FluentValidation;
using GroceryStoreManagementSystem.Models;
namespace GroceryStoreManagementSystem.ValidationClass
{
    public class ProductCategoryValidator:AbstractValidator<ProductCategory>
    {
        public ProductCategoryValidator() {
            RuleFor(c => c.CategoryName)
                .NotNull().WithMessage("Category must not be empty.");       
        }
    }
}
