using GroceryStoreManagementSystem.Models;
using FluentValidation;

namespace GroceryStoreManagementSystem.ValidationClass
{
    public class CustomerValidator:AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(c => c.CustomerName)
                .NotNull().WithMessage("Customer name must not be empty.")
                .Length(3, 20).WithMessage("Customer name must be between 3 and 20 characters.")
                .Matches("^[A-Za-z ]*$").WithMessage("Customer name must contain only letters and spaces.");

            RuleFor(c => c.Phone)
                .NotNull().WithMessage("Phone number must not be empty.")
                .Length(10).WithMessage("Phone number must be  of 10 characters")
                .Matches("^[0-9]{10}$").WithMessage("Phone number must contain only digits and must of 10 digits");

            RuleFor(c => c.Address)
               .NotNull().WithMessage("Address must not be empty.")
               .MinimumLength(4).WithMessage("Address must be at least of  4 characters");

            RuleFor(c => c.Email)
               .NotNull().WithMessage("Email must not be empty.")
               .Matches("^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$").WithMessage("Email must be in proper format");
        }
    }
}
