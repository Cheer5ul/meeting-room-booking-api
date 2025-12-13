using Microsoft.EntityFrameworkCore;
using RoomBooking.Core;
using RoomBooking.Core.Abstractions;
using RoomBooking.Core.Abstractions.Repositories;
using RoomBooking.DataAccess.DbContext;
using RoomBooking.DataAccess.Entities;

namespace RoomBooking.DataAccess.Repositories;

public class RoomRepository(RoomBookingDbContext dbContext) : IRoomRepository
{
    public async Task<List<Room>> Get(CancellationToken cancellationToken = default)
    {
        var roomEntities = await dbContext.Rooms
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var rooms = roomEntities
            .Select(r => Room.Create(
                r.Id,
                r.Name,
                r.Capacity,
                r.HasProjector,
                r.HasTv,
                r.HasWhiteBoard).room)
            .ToList();

        return rooms;
    }

    public async Task<Room?> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var roomEntities = await dbContext.Rooms
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        //Null check
        if (roomEntities == null)
            return null;

        var room = Room.Create(
            roomEntities.Id,
            roomEntities.Name,
            roomEntities.Capacity,
            roomEntities.HasTv,
            roomEntities.HasTv,
            roomEntities.HasWhiteBoard).room;
        
        return room;

    }

    public async Task<Guid> Create(Room room, CancellationToken cancellationToken = default)
    {
        var roomEntity = new RoomEntity
        {
            Id = room.Id,
            Name = room.Name,
            Capacity = room.Capacity,
            HasProjector = room.HasProjector,
            HasTv = room.HasTv,
            HasWhiteBoard = room.HasWhiteBoard
        };

        await dbContext.Rooms.AddAsync(roomEntity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return roomEntity.Id;
    }

    public async Task<Guid> Update(Guid id, string name, int capacity, bool hasProjector,
        bool hasTv, bool hasWhiteBoard, CancellationToken cancellationToken = default)
    {
        await dbContext.Rooms.Where(r => r.Id == id)
            .ExecuteUpdateAsync(r => r
                .SetProperty(p => p.Name, name)
                .SetProperty(p => p.Capacity, capacity)
                .SetProperty(p => p.HasProjector, hasProjector)
                .SetProperty(p => p.HasTv, hasTv)
                .SetProperty(p => p.HasWhiteBoard, hasWhiteBoard), cancellationToken);
        return id;
    }

    public async Task<Guid> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        await dbContext.Rooms.Where(r => r.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return id;
    }
        
}