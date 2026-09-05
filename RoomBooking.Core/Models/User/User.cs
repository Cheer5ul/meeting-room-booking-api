using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Core.Models.User;

public class User
{
    private User(
        Guid id, string name, string email, string department, string passwordHash,
        List<Booking.Booking>? bookings = default(List<Booking.Booking>))
    {
        Id = id;
        Name = name;
        Email = email;
        Department = department;
        PasswordHash = passwordHash;
        Bookings = bookings ?? [];
    }
    public Guid Id { get; }
    public string Name { get; }
    public string Email { get; } 
    public string Department { get; }
    public string PasswordHash { get; set; }
    
    public AddressInfo? AddressInfo { get; private set;}
    
    public List<Booking.Booking> Bookings { get; set; }

    public static Result<User> Create
        (string name, string email, string department, string passwordHash,
            List<Booking.Booking>? bookings = default(List<Booking.Booking>))
    {
        //possible basic validation ???
        
        var user = new User
        (
            id: Guid.NewGuid(),
            name: name,
            email: email,
            department: department,
            passwordHash: passwordHash,
            bookings: bookings
        );
        
        return Result<User>.Success(user);
    }

    public AddressInfo AddAddressInfo
        (string street, string city, string state, string postalCode,
            string country)
    {
        var addressInfo = AddressInfo.Create(
             street, city, state, postalCode, country);
        
        AddressInfo = addressInfo;
        
        return addressInfo;
    }

    public static User Assebmle(Guid id, string name, string email, string department, string passwordHash)
    {
        var user = new User
        (
            id: id,
            name: name,
            email: email,
            department: department, 
            passwordHash: passwordHash
        );
        
        return user;
    }
}