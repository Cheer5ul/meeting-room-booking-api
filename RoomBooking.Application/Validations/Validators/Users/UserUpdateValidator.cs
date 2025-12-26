using FluentValidation;
using RoomBooking.Application.DTOs.User;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Validations.Validators.Users;

public sealed class UserUpdateValidator : AbstractValidator<UserUpdateDto>
{
    public UserUpdateValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");
        
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Invalid email format");
        
        RuleFor(x => x.Department)
            .NotEmpty()
            .MinimumLength(2).WithMessage("Department length should be at least 2 character")
            .MaximumLength(50).WithMessage("Department length can be maximum 50 characters");
    }
}