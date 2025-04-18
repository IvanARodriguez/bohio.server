using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Bohio.Core.Entities;
using Bohio.Core.Results;
using NUlid;
using Application.Common.Errors;

namespace Bohio.Api.IntegrationTests;

public class HomeSpaceCrudTests : IClassFixture<BohioWebAppFactory>
{
  private readonly HttpClient _client;

  public HomeSpaceCrudTests()
  {
    Environment.SetEnvironmentVariable("DOTNET_RUNNING_IN_TESTS", "true");
    Environment.SetEnvironmentVariable("JWT_SECRET", "ThisIsAValidTestKeyThatIsDefinitelyLongEnough123!");
    Environment.SetEnvironmentVariable("DATABASE_CONNECTION_STRING", "this_connection_string:not_being_used");
    var app = new BohioWebAppFactory();
    _client = app.CreateDefaultClient();
  }

  [Fact]
  public async Task GetRoot_ReturnsWelcomeMessage()
  {
    // Act
    var response = await _client.GetAsync("/");

    // Assert
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadFromJsonAsync<WelcomeMessage>();
    content?.Message.Should().Be("Welcome to Bohio!");
  }

  [Fact]
  public async Task CreateHomeSpace_ReturnsBadRequest_WhenHomeSpaceIsInvalid()
  {
    // Arrange
    var homeSpace = new HomeSpace();

    // Act
    var response = await _client.PostAsJsonAsync("/api/homespace", homeSpace);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

    var errorResponse = await response.Content.ReadFromJsonAsync<List<Error>>();
    errorResponse.Should().NotBeNull();
    errorResponse.Should().Contain(e => e.Code == "Name" && e.Message == "Name is required.");
    errorResponse.Should().Contain(e => e.Code == "Description" && e.Message == "Description is required.");
  }

  [Fact]
  public async Task CreateHomeSpace_ReturnsConflict_WhenHomeSpaceAlreadyExists()
  {
    // Arrange
    var homeSpace = new HomeSpace
    {
      Id = Ulid.NewUlid(),
      Name = "Existing Home",
      Description = "Already exists",
      Features = ["Feature1"],
      Category = "Living Room",
      City = "New York",
      State = "NY",
      Country = "USA",
      Status = HomeSpaceStatus.Draft,
    };

    var createResponse = await _client.PostAsJsonAsync("/api/homespace", homeSpace);
    createResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

    // Act - Try to add the same HomeSpace again
    var response = await _client.PostAsJsonAsync("/api/homespace", homeSpace);

    // Assert - Ensure the conflict status code is returned
    response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);
    var errorResponse = await response.Content.ReadFromJsonAsync<Error>();
    errorResponse.Should().NotBeNull();
    errorResponse.Code.Should().Be(Errors.HomeSpace.AlreadyExists.Code);
    errorResponse.Message.Should().Be(Errors.HomeSpace.AlreadyExists.Message);
  }


  [Fact]
  public async Task CreateHomeSpace_ReturnsCreated_WhenHomeSpaceIsValid()
  {
    // Arrange - Valid HomeSpace object
    var homeSpace = new HomeSpace
    {
      Name = "Test Home",
      Description = "A stylish and modern living room with a minimalist design.",
      Features = ["Open space", "Natural lighting", "Smart home integration"],
      Category = "Living Room",
      City = "Los Angeles",
      State = "California",
      Country = "USA",
      Status = HomeSpaceStatus.Draft
    };

    // Act
    var response = await _client.PostAsJsonAsync("/api/homespace", homeSpace);

    // Assert
    response.StatusCode.Should().Be(HttpStatusCode.Created);
    var locationHeader = response.Headers.Location?.ToString();
    locationHeader.Should().NotBeNull();
  }
}
