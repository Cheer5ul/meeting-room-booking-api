using RoomBooking.Application.Validations.Abstractions;
using RoomBooking.Application.Validations.Abstractions.Bookings;
using RoomBooking.Application.Validations.Exceptions;
using RoomBooking.Core;
using RoomBooking.Core.Abstractions;
using RoomBooking.Core.Abstractions.Repositories;

namespace RoomBooking.Application.Services;

public class BookingService(IBookingRepository repository, IBookingCreationValidator validator) : IBookingService
{
    public async Task<List<Booking>> GetAllBookings(CancellationToken cancellationToken = default)
    {
        return await repository.Get(cancellationToken);
    }

    public async Task<List<Booking>> GetByRoom(Guid id, CancellationToken cancellationToken = default)
    {
        return await repository.GetByRoom(id, cancellationToken);
    }

    public async Task<List<Booking>> GetByUser(Guid id, CancellationToken cancellationToken = default)
    {
        return await repository.GetByUser(id, cancellationToken);
    }

    public async Task<(Guid? Guid, List<string> Errors)> Create(Booking booking, CancellationToken cancellationToken = default)
    {
        var errors = await validator.Validate(booking, cancellationToken);
        
        if (errors.Any())
        {
            return (null, errors);
        }
        
        var id = await repository.Create(booking, cancellationToken);
        return (id, errors);
    }
    
    public async Task<Guid> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        return await repository.Delete(id, cancellationToken);
    }
}