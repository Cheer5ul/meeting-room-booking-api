using FluentValidation;
using RoomBooking.Core.Models.Room;
using RoomBooking.Core.Results.Errors;

namespace RoomBooking.Application.Validations.Validators.Rooms;

public sealed class RoomCreationValidator : AbstractValidator<Room>
{
    public RoomCreationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithErrorCode(RoomErrors.NameRequired.Code)
                .WithMessage(RoomErrors.NameRequired.Description)
            .MaximumLength(50)
                .WithErrorCode(RoomErrors.NameTooLong.Code)
                .WithMessage(RoomErrors.NameTooLong.Description);
        
        RuleFor(x => x.Capacity)
            .NotEmpty()
                .WithErrorCode(RoomErrors.CapacityRequired.Code)
                .WithMessage(RoomErrors.CapacityRequired.Description)
            .GreaterThan(0)
                .WithErrorCode(RoomErrors.CapacityGreaterThanZero.Code)
                .WithMessage(RoomErrors.CapacityGreaterThanZero.Description);

        // RuleFor(x => x.HasProjector)
        //     .NotEmpty()
        //         .WithErrorCode(RoomErrors.ProjectorInfoRequired.Code)
        //         .WithMessage(RoomErrors.ProjectorInfoRequired.Description);
        //
        // RuleFor(x => x.HasTv)
        //     .NotEmpty()
        //         .WithErrorCode(RoomErrors.TvInfoRequired.Code)
        //         .WithMessage(RoomErrors.TvInfoRequired.Description);
        //
        // RuleFor(x => x.HasWhiteBoard)
        //     .NotEmpty()
        //         .WithErrorCode(RoomErrors.WhiteBoardInfoRequired.Code)
        //         .WithMessage(RoomErrors.WhiteBoardInfoRequired.Description);
    }
}