using FluentValidation;
using FluentValidation.Results;

namespace RoomBooking.Application.Validations.Validators;

public static class InlineValidationExtensions
{
    public static Task<ValidationResult> ValidateInlineAsync<T>(
        this T obj,
        Action<InlineValidator<T>> configure)
    {
        var validator = new InlineValidator<T>();
        configure(validator);

        return validator.ValidateAsync(obj);
    }
}