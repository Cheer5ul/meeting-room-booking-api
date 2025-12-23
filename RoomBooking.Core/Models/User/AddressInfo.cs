namespace RoomBooking.Core.Models.User;

public class AddressInfo
{
    private AddressInfo(string street, string city, string state, string postalCode ,string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
    public string Street { get; }
    public string City { get; }
    public string State { get; }
    public string PostalCode { get; }
    public string Country { get; }

    internal static AddressInfo Create
        (string street, string city, string state, string postalCode, string country)
    {
        return new AddressInfo(street, city, state, postalCode, country);
    }
}