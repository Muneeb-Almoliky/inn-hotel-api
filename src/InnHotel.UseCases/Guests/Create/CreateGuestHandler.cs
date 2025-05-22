using InnHotel.Core.GuestAggregate;
using InnHotel.Core.GuestAggregate.Specifications;

namespace InnHotel.UseCases.Guests.Create;
internal class CreateGuestHandler(IRepository<Guest> _repository)
  : ICommandHandler<CreateGuestCommand, Result<int>>
{   

    public async Task<Result<int>> Handle(CreateGuestCommand request, CancellationToken cancellationToken)
    {
      var spec = new GuestByProofNumberSpec(request.IdProofNumber);
      var exists = await _repository.AnyAsync(spec, cancellationToken);
      if (exists)
      {
        return Result.Conflict("A guest with that ID proof number already exists.");
      }
      
      var guest = new Guest(request.FirstName, request.LastName, request.IdProofType, request.IdProofNumber, request.Email, request.Phone, request.Address);

      var createdGuest = await _repository.AddAsync(guest, cancellationToken);

      await _repository.SaveChangesAsync(cancellationToken);

      return Result.Success(createdGuest.Id);
  }
}
