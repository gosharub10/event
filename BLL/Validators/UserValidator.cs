using BLL.DTO;
using FluentValidation;

namespace BLL.Validators;

internal class UserValidator: AbstractValidator<UserRegistrationDTO>
{
    public UserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("email is required.")
            .EmailAddress().WithMessage("email is invalid.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("password is required.")
            .MinimumLength(6).WithMessage("password must be at least 6 characters.");
        
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("first name is required.")
            .MinimumLength(3).WithMessage("first name must be at least 3 characters.");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("last name is required.")
            .MinimumLength(3).WithMessage("last name must be at least 3 characters.");

        RuleFor(x => x.Birthday)
            .NotEmpty().WithMessage("birthday is required.")
            .Must(BeAValidDate).WithMessage("birthday is invalid.")
            .LessThan(DateOnly.FromDateTime(DateTime.Now)).WithMessage("birthday must be in the future.")
            .Must(BeAtLeast18YearsOld).WithMessage("birthday is invalid.");
    }

    private bool BeAValidDate(DateOnly date)
    {
        return date != default;
    }

    private bool BeAtLeast18YearsOld(DateOnly date)
    {
        var age = DateOnly.FromDateTime(DateTime.Now).Year - date.Year;
        return age >= 18;
    }
}