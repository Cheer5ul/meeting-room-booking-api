using FluentValidation.Results;
using RoomBooking.Application.Validations.Abstractions.Validators;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Validations.Converters;

public class ValidationToErrorConverter : IValidationToErrorConverter
{
    public List<Error> ValidationToErrors(List<ValidationFailure> validationErrors)
    {
        var errorsDictionary = validationErrors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => string.IsNullOrEmpty(g.Key) ? "" : g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        List<Error> errors = new();

        foreach (var error in errorsDictionary)
        {
            errors.Add(new Error(error.Key, error.Value.FirstOrDefault()));
        }
        return errors;
    }
}