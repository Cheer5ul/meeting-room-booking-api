using FluentValidation;
using RoomBooking.Application.DTOs.AddressInfo;

namespace RoomBooking.Application.Validations.Validators.Users.AddressInfo;

// public interface IAddressApi
// {
//     Task<bool> ValidateAsync(AddressInfo addressInfo);
// }
public sealed class AddressInfoAddingDtoValidator : AbstractValidator<Core.Models.User.AddressInfo>
{
    public AddressInfoAddingDtoValidator(/*IAddressApi addressApi*/)
    {
        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required");

        RuleFor(x=> x.City)
            .NotEmpty().WithMessage("City is required");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required");;

        RuleFor(x=> x.PostalCode)
            .NotEmpty().WithMessage("Postal code is required")
            .MaximumLength(50).WithMessage("Postal code should be maximum 50 characters");
        
        RuleFor(x=> x.Country)
            .NotEmpty().WithMessage("Country is required");

        // RuleFor(x => x).MustAsync((a,_) => addressApi.ValidateAsync(a));
    }
}