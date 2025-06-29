using Application.DTOs;
using FluentValidation;

namespace Application.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(u => u.UserName)
                .NotEmpty().WithMessage("UserName is required")
                .MinimumLength(3);

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must have email format");

            RuleFor(u => u.PasswordHash)
                .NotEmpty().MinimumLength(6).WithMessage("Password is required and must have 6 characters at least");
        }
    }
}
