namespace InnHotel.Web.Branches;

public record DeleteBranchRequest
{
  public const string Route = "api/Branches/{BranchId:int}";
  public static string BuildRoute(int branchId) => Route.Replace("{BranchId:int}", branchId.ToString());

  public int BranchId { get; set; }
}

