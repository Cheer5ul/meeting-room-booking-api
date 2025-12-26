using RoomBooking.Core.Models;

namespace RoomBooking.Core.Results.Errors;

public static class UserErrors
{
    public static readonly Error UserNotFound = new (
        "User.UserNotFound",
        "User does not exist.");

    public static readonly Error NameExceedsCharacterAmount = new (
        "User.NameExceedsCharacterAmount",
        "Name exceeds character amount");
    public static readonly Error InvalidEmail = new(
        "User.InvalidEmail",
        "Invalid email format");

    public static readonly Error TooShortDepartmentName = new(
        "User.TooShortDepartmentName",
        "Department name is too short");

    public static readonly Error TooLongDepartmentName = new(
        "User.TooLongDepartmentName",
        "Department name is too long");
    
    public static readonly Error EmailRequired = new(
        "User.EmailRequired",
        "Email is required");
    
    public static readonly Error NameRequired = new(
        "User.NameRequired",
        "Name is required");
    
    public static readonly Error DepartmentRequired = new(
        "User.DepartmentRequired",
        "Department is required");
    
    
    
}