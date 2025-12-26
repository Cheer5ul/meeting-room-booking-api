using FluentValidation;
using RoomBooking.Core.Models.User;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Validations.Validators.Users;

public sealed class UserCreationValidator : AbstractValidator<User>
{
    public UserCreationValidator()
    {
        // Email validation
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithErrorCode(UserErrors.NameRequired.Code)
                .WithMessage(UserErrors.NameRequired.Description)
            .MaximumLength(100)
                .WithErrorCode(UserErrors.NameExceedsCharacterAmount.Code)
                .WithMessage(UserErrors.NameExceedsCharacterAmount.Description);

        // Password validation
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithErrorCode(UserErrors.EmailRequired.Code)
                .WithMessage(UserErrors.EmailRequired.Description)
            .EmailAddress()
                .WithErrorCode(UserErrors.InvalidEmail.Code)
                .WithMessage(UserErrors.InvalidEmail.Description);

        // Department validation
        RuleFor(x => x.Department)
            .NotEmpty()
                .WithErrorCode(UserErrors.DepartmentRequired.Code)
                .WithMessage(UserErrors.DepartmentRequired.Description)
            .MinimumLength(2)
                .WithErrorCode(UserErrors.TooShortDepartmentName.Code)
                .WithMessage(UserErrors.TooShortDepartmentName.Description)
            .MaximumLength(50)
                .WithErrorCode(UserErrors.TooLongDepartmentName.Code)
                .WithMessage(UserErrors.TooLongDepartmentName.Description);
        
        // AddressInfo validation 
        // RuleFor(x => x.AddressInfo)
        //     .NotNull().WithMessage("Address information is required")
        //     .SetValidator(new AddressInfoValidator());

    }
}