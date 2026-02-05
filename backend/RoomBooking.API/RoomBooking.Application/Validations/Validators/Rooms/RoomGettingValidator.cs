using RoomBooking.Application.Validations.Abstractions.Rooms;
using RoomBooking.Core.Abstractions.Repositories;

namespace RoomBooking.Application.Validations.Validators.Rooms;

public class RoomGettingValidator(IRoomRepository repository) : IRoomGettingValidator
{
    public async Task<bool> IsExisting(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetById(id, cancellationToken);
        
        return result != null;
    }
}