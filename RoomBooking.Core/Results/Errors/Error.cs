namespace RoomBooking.Core.Results.Errors;

public record Error(string Code, string? Description = null)
{
    public static readonly Error None = new(string.Empty);

    //public static implicit operator Result(List<Error> errors) => Result.Failures(errors);
}