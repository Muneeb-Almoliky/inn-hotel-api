namespace InnHotel.UseCases.Branches.Update;

public record UpdateBranchCommand(
    int BranchId,
    string Name,
    string Location) : ICommand<Result<BranchDTO>>;