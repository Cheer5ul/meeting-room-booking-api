using FluentValidation;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.User;

namespace RoomBooking.Application.Validations.Validators.Users;

public class UserCreationDtoValidator : AbstractValidator<User>
{
    public UserCreationDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required")
            .MinimumLength(2).WithMessage("Department length should be at least 2 character");
    }
}