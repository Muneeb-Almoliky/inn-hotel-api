namespace InnHotel.UseCases.Branches.Create;

public record CreateBranchCommand(
    string Name,
    string Location) : ICommand<Result<BranchDTO>>;