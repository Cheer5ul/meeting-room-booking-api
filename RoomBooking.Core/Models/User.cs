using RoomBooking.Core.Models;

namespace RoomBooking.Core;

public class User
{
    private User(Guid id, string name, string email, string department, List<Booking>? bookings = default(List<Booking>))
    {
        Id = id;
        Name = name;
        Email = email;
        Department = department;
        Bookings = bookings ?? [];
    }
    public Guid Id { get; }
    public string Name { get; } = string.Empty;
    public string Email { get; } = string.Empty;
    public string Department { get; } = string.Empty;
    
    public List<Booking> Bookings { get; set; } = [];

    public static (User user, string? error) Create
        (Guid id, string name, string email, string department, List<Booking>? bookings = default(List<Booking>))
    {
        string error = string.Empty;
        if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(department))
            error = "Invalid name or email or department";

        var user = new User
        (
            id: id,
            name: name,
            email: email,
            department: department,
            bookings: bookings
        );
        
        return (user, error);
    }
}