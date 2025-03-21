using FluentValidation;
using Order_Management.DTOs;

namespace Order_Management.Validators
{
    public class CreateOrderDetailValidator : AbstractValidator<CreateOrderDetailDTO>
    {
        public CreateOrderDetailValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required")
                .MaximumLength(255).WithMessage("Product name must not exceed 255 characters");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");
        }
    }
}
