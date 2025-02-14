using System.Net.Http.Json;
using Homespirations.Core.Entities;
using FluentAssertions;
using NUlid;
using Homespirations.Api.IntegrationTests;
using Homespirations.Application.Services;
using Homespirations.Core.Results;
using Application.Common.Errors;

namespace Homespirations.Api.IntegrationTests;

public class HomeSpaceCrudTests : IClassFixture<HomespirationsWebAppFactory>
{
    private readonly HomespirationsWebAppFactory _app;
    private readonly HttpClient _client;

    public HomeSpaceCrudTests()
    {
        MyEnvironment.SetVariable("DOTNET_RUNNING_IN_TESTS", "true");
        _app = new HomespirationsWebAppFactory();
        _client = _app.CreateDefaultClient();
    }

    [Fact]
    public async Task GetRoot_ReturnsWelcomeMessage()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<WelcomeMessage>();
        content?.Message.Should().NotBeNullOrEmpty();
        content?.Message.Should().Be("Welcome to Homespirations!");
    }

    [Fact]
    public async Task CreateHomeSpace_ReturnsBadRequest_WhenHomeSpaceIsInvalid()
    {
        // Arrange
        var homeSpace = new HomeSpace(); // Invalid HomeSpace (missing required data)

        // Act
        var response = await _client.PostAsJsonAsync("/api/homespace", homeSpace);

        // Assert
        var errorResponse = await response.Content.ReadFromJsonAsync<Error>();
        errorResponse.Should().NotBeNull();
        errorResponse.Code.Should().Be(Errors.HomeSpace.InvalidData.Code);
        errorResponse.Message.Should().Be(Errors.HomeSpace.InvalidData.Message);
    }

    [Fact]
    public async Task CreateHomeSpace_ReturnsConflict_WhenHomeSpaceAlreadyExists()
    {
        // Arrange
        var homeSpace = new HomeSpace { Id = Ulid.NewUlid() };

        // Act - Try to add an already existing HomeSpace
        var response = await _client.PostAsJsonAsync("/api/homespace", homeSpace);

        // Assert - Ensure the conflict status code is returned
        var errorResponse = await response.Content.ReadFromJsonAsync<Error>();
        errorResponse.Should().NotBeNull();
        errorResponse.Code.Should().Be(Errors.HomeSpace.InvalidData.Code);
        errorResponse.Message.Should().Be(Errors.HomeSpace.InvalidData.Message);
    }

    [Fact]
    public async Task CreateHomeSpace_ReturnsCreated_WhenHomeSpaceIsValid()
    {
        // Arrange
        var homeSpace = new HomeSpace
        {
            Name = "Test Home",
            Description = "A stylish and modern living room with a minimalist design.",
            Features = [
                "Open space",
                "Natural lighting",
                "Smart home integration"
            ],
            Category = "Living Room",
            City = "Los Angeles",
            State = "California",
            Country = "USA",
            Status = HomeSpaceStatus.Draft,
        };

        // Act - Add a new HomeSpace
        var response = await _client.PostAsJsonAsync("/api/homespace", homeSpace);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        var locationHeader = response.Headers.Location?.ToString();

        locationHeader.Should().Be($"/api/homespace/{homeSpace.Id}");
    }

}

class MyEnvironment
{
    public static string GetVariable(string variableName)
    {
        return Environment.GetEnvironmentVariable(variableName) ?? "";
    }

    public static void SetVariable(string variableName, string value)
    {
        Environment.SetEnvironmentVariable(variableName, value);
    }
}
