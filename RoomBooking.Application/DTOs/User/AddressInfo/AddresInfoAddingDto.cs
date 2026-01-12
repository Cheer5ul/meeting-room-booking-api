namespace RoomBooking.Application.DTOs.AddressInfo;

public class AddresInfoAddingDto
{
    public AddresInfoAddingDto(
        string street, string city, string state, string postalCode ,string country)
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
}