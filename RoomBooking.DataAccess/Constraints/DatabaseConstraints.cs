namespace RoomBooking.DataAccess.Constraints;

public static class DatabaseConstraints
{
    public const string BookingOverlapConstraint = "no_overlapping_bookings";
    public const string BookingOverlapIndex = "IX_Bookings_RoomId_Times";
}