namespace InnHotel.UseCases.Branches.List;

public record ListBranchesQuery(int PageNumber, int PageSize) : IQuery<Result<(List<BranchDTO> Items, int TotalCount)>>;