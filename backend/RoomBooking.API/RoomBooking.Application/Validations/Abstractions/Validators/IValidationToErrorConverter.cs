using FluentValidation.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Validations.Abstractions.Validators;

public interface IValidationToErrorConverter
{
    List<Error> ValidationToErrors(List<ValidationFailure> validationErrors);
}