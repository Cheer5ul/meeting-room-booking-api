namespace RoomBooking.Core.Results.Errors;

public sealed record Error(string Code, string? Description = null)
{
    public static readonly Error None = new(string.Empty);
    public static explicit operator Result(Error error) => Result.Failure(error);
    // public static explicit operator Result(List<Error> errors) => Result.MultipleFailure(errors);
}