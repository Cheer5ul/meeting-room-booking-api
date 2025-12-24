using FluentValidation;
using RoomBooking.Core.Models;
using RoomBooking.Core.Models.User;

namespace RoomBooking.Application.Validations.Validators.Users;

public sealed class UserCreationDtoValidator : AbstractValidator<User>
{
    public UserCreationDtoValidator()
    {
        // Email validation
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        // Password validation
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        // Department validation
        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required")
            .MinimumLength(2).WithMessage("Department length should be at least 2 character")
            .MaximumLength(50).WithMessage("Department length can be maximum 50 characters");
        
        // AddressInfo validation 
        RuleFor(x => x.AddressInfo)
            .NotNull().WithMessage("Address information is required")
            .SetValidator(new AddressInfoValidator());

        // RuleFor(x => x.AddressInfo)
        //     .NotEmpty().WithMessage("Address information is required")
        //     .SetValidator(new AddressInfoValidator());
    }
}