namespace InnHotel.UseCases.Branches.Delete;

public record DeleteBranchCommand(int BranchId) : ICommand<Result>;