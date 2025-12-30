namespace RoomBooking.DataAccess.Constraints;

public static class CustomPostgresErrorCodes
{
    public const string ExclusionConstraintViolation = "23P01";
    public const string UniqueViolation = "23505";
}