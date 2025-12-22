using RoomBooking.Core.Models;

namespace RoomBooking.Core.Results.Errors;

public static class UserErrors
{
    public static readonly Error UserNotFound = new Error(
        "User.UserNotFound",
        "User does not exist.");
}