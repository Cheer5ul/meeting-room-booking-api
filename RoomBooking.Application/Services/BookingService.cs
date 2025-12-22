using RoomBooking.Application.Validations.Abstractions.Bookings;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.Core.Abstractions.Services;
using RoomBooking.Core.Models;
using RoomBooking.Core.Results;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Services;

public class BookingService(IBookingRepository repository, IBookingCreationValidator validator) : IBookingService
{
    public async Task<Result<List<Booking>>> GetAllBookings(CancellationToken cancellationToken = default)
    {
        return await repository.Get(cancellationToken);
    }

    public async Task<Result<List<Booking>>> GetByRoom(Guid id, CancellationToken cancellationToken = default)
    {
        return await repository.GetByRoom(id, cancellationToken);
    }

    public async Task<Result<List<Booking>>> GetByUser(Guid id, CancellationToken cancellationToken = default)
    {
        return await repository.GetByUser(id, cancellationToken);
    }

    public async Task<Result<Guid>> Create(Booking booking, CancellationToken cancellationToken = default)
    {
        var errors = await validator.Validate(booking, cancellationToken);
        
        if (errors.Any())
        {
            return Result<Guid>.MultipleFailures(errors);
        }
        
        var id = await repository.Create(booking, cancellationToken);
        return id;
    }
    
    public async Task<Result<Guid>> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        
        var wasDeleted = await repository.Delete(id, cancellationToken);
        
        if (!wasDeleted)
        {
            return Result<Guid>.MultipleFailures([BookingErrors.NotFound]);
        }

        return id;
    }
}