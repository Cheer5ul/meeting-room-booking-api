using FluentValidation;
using RoomBooking.Core.Models.User;

namespace RoomBooking.Application.Validations.Validators.Users;

// public interface IAddressApi
// {
//     Task<bool> ValidateAsync(AddressInfo addressInfo);
// }
public sealed class AddressInfoValidator : AbstractValidator<AddressInfo>
{
    public AddressInfoValidator(/*IAddressApi addressApi*/)
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