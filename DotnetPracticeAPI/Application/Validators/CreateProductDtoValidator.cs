using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Name is required");

            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage("Price must be positive");

            RuleFor(p => p.Description)
                .NotEmpty().WithMessage("Description is required");
        }
    }
}
