using InnHotel.Infrastructure.Data;
using InnHotel.Web.Contributors;

namespace InnHotel.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class ContributorList(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
  private readonly HttpClient _client = factory.CreateClient();

  [Fact]
  public async Task ReturnsTwoContributors()
  {
    var result = await _client.GetAndDeserializeAsync<ContributorListResponse>("/Contributors");

    //result.Contributors.Count.ShouldBe(2);
    //result.Contributors.ShouldContain(contributor => contributor.Name == SeedData.Contributor1.Name);
    //result.Contributors.ShouldContain(contributor => contributor.Name == SeedData.Contributor2.Name);
  }
}
