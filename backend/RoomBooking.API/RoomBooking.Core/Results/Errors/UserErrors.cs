namespace RoomBooking.Core.Results.Errors;

public static class UserErrors
{
    public static Error UserNotFound => new (
        "User.UserNotFound",
        "User does not exist.");

    public static Error NameExceedsCharacterAmount => new (
        "User.NameExceedsCharacterAmount",
        "Name exceeds character amount");
    public static Error InvalidEmail => new(
        "User.InvalidEmail",
        "Invalid email format");

    public static Error TooShortDepartmentName => new(
        "User.TooShortDepartmentName",
        "Department name is too short");

    public static Error TooLongDepartmentName => new(
        "User.TooLongDepartmentName",
        "Department name is too long");
    
    public static Error EmailRequired => new(
        "User.EmailRequired",
        "Email is required");
    
    public static Error NameRequired => new(
        "User.NameRequired",
        "Name is required");
    
    public static Error DepartmentRequired => new(
        "User.DepartmentRequired",
        "Department is required");

    public static Error EmailAlreadyUsed => new(
        "User.EmailAlreadyUsed",
        "Email is already in use");

    public static Error IncorrectPassword => new(
        "User.IncorrectPassword",
        "Incorrect password");

}