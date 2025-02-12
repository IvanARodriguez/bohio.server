using System.Net.Http.Json;
using Homespirations.Core.Entities;

namespace Homespirations.Api.IntegrationTests;

public class HomeSpaceCrudTests
{
    [Fact]
    public async Task GetAllHomeSpaces_ShouldReturnAllHomeSpaces()
    {
        // Arrange
        var app = new HomespirationsWebAppFactory();
        var client = app.CreateClient();
        var homeSpace = new HomeSpace
        {
            Name = "Sample HomeSpace",
            Description = "A description of the sample home space",
            Features = ["Feature 1", "Feature 2"],
            Tags = ["Tag1", "Tag2"],
            Category = "Living Room",
            City = "Sample City",
            State = "Sample State",
            Country = "Sample Country",
            Status = HomeSpaceStatus.Published
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/homespace", homeSpace);
        response.EnsureSuccessStatusCode();

        // Act - Retrieve all home spaces
        var getAllResponse = await client.GetAsync("/api/homespace");
        getAllResponse.EnsureSuccessStatusCode();

        // Assert
        var getAllResult = await getAllResponse.Content.ReadFromJsonAsync<IEnumerable<HomeSpace>>();
        Assert.NotNull(getAllResult);
        Assert.NotEmpty(getAllResult);
    }

}