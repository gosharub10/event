using BLL.DTO;
using FluentValidation;

namespace BLL.Validators;

internal class EventNewValidator: AbstractValidator<EventNewDTO>
{
    public EventNewValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("title is required.")
            .MinimumLength(4).WithMessage("title must be at least 4 characters.")
            .MaximumLength(60).WithMessage("title must be between 4 and 100 characters.");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("description is required.")
            .MinimumLength(10).WithMessage("description must be at least 10 characters.")
            .MaximumLength(500).WithMessage("description must be between 10 and 500 characters.");
        
        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("category is required.")
            .MinimumLength(3).WithMessage("category must be at least 3 characters.")
            .MaximumLength(50).WithMessage("category must be between 3 and 50 characters.");
        
        RuleFor(x => x.EventDateTime)
            .NotEmpty().WithMessage("event date is required.")
            .Must(BeAValidDate).WithMessage("event date must be a valid date.");
        
        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).WithMessage("max participants must be greater than zero.");
        
        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("location is required.")
            .MinimumLength(3).WithMessage("location must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("location must not exceed 50 characters.");
    }
    
    private bool BeAValidDate(DateOnly date)
    {
        return date > DateOnly.FromDateTime(DateTime.Now);
    }
}