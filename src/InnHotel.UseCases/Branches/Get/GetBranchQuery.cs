namespace InnHotel.UseCases.Branches.Get;

public record GetBranchQuery(int branchId) : IQuery<Result<BranchDTO>>;