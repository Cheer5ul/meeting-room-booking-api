namespace RoomBooking.Core.Results.Errors;

public sealed record Error(string Code, string? Description = null)
{
    public static readonly Error None = new(string.Empty);

    // public static implicit operator Result(Error errors) => Result.Failure(errors);
}