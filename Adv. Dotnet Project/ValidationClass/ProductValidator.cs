using FluentValidation;
using GroceryStoreManagementSystem.Models;
namespace GroceryStoreManagementSystem.ValidationClass
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(c => c.ProductName)
                .NotNull().WithMessage("Product must not be empty.");
            RuleFor(c => c.Price)
                .NotNull().WithMessage("Price must not be empty.");
            RuleFor(c => c.UsedStock)
               .NotNull().WithMessage("Used stock must not be empty.");
            RuleFor(c => c.RemainingStock)
               .NotNull().WithMessage("Remaining stock must not be empty.");
        }
    }
}
