using FluentValidation;
using Order_Management.DTOs;

namespace Order_Management.Validators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderDTO>
    {
        public CreateOrderValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required")
                .MaximumLength(255).WithMessage("Customer name must not exceed 255 characters");

            RuleFor(x => x.OrderDetails)
                .NotEmpty().WithMessage("Order details at least 1 item");
        }
    }

    public class UpdateOrderValidator : AbstractValidator<UpdateOrderDTO>
    {
        public UpdateOrderValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Customer name is required")
                .MaximumLength(255).WithMessage("Customer name must not exceed 255 characters");
        }
    }
}
