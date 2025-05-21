namespace InnHotel.Web.Auth;

public class UserCreatedResponse(string id, string email, string name)
{
  public string Id { get; set; } = id;
  public string Email { get; set; } = email;
  public string Name { get; set; } = name;
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
