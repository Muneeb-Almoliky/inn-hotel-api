using InnHotel.Core.GuestAggregate;
using InnHotel.Core.GuestAggregate.Specifications;
using Microsoft.EntityFrameworkCore;

namespace InnHotel.UseCases.Guests.Update;

public class UpdateGuestHandler(IRepository<Guest> _repository)
    : ICommandHandler<UpdateGuestCommand, Result<GuestDTO>>
{
    public async Task<Result<GuestDTO>> Handle(UpdateGuestCommand request, CancellationToken cancellationToken)
    {
        var spec = new GuestByIdSpec(request.GuestId);
        var guest = await _repository.FirstOrDefaultAsync(spec, cancellationToken);
        
        if (guest == null)
            return Result.NotFound();

        // Check if ID proof number is already used by another guest
        var idProofSpec = new GuestByIdProofNumberSpec(request.IdProofNumber);
        var existingGuest = await _repository.FirstOrDefaultAsync(idProofSpec, cancellationToken);
        if (existingGuest != null && existingGuest.Id != request.GuestId)
        {
            return Result.Error("ID proof number is already registered with another guest.");
        }

        guest.UpdateDetails(
            request.FirstName,
            request.LastName,
            request.IdProofType,
            request.IdProofNumber,
            request.Email,
            request.Phone,
            request.Address);

        try
        {
            await _repository.UpdateAsync(guest, cancellationToken);
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UX_guests_idproof_number") == true)
        {
            return Result.Error("ID proof number is already registered with another guest.");
        }

        return new GuestDTO(
            guest.Id,
            guest.FirstName,
            guest.LastName,
            guest.IdProofType,
            guest.IdProofNumber,
            guest.Email,
            guest.Phone,
            guest.Address ?? "");
    }
}