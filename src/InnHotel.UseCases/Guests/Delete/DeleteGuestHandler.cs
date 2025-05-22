namespace InnHotel.UseCases.Guests.Delete;
using InnHotel.Core.Interfaces;

public class DeleteGuestHandler(IDeleteGuestService _deleteGuestService)
  : ICommandHandler<DeleteGuestCommand, Result>
{
  public async Task<Result> Handle(DeleteGuestCommand request, CancellationToken cancellationToken) =>
    await _deleteGuestService.DeleteGuest(request.GuestId);
}
