using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Customers.Api.Tests.Integration.CustomerController;

// [Collection("CustomerApi Collection")]
public class GetCustomerControllerTests : IClassFixture<WebApplicationFactory<IApiMarker>>
{
    private readonly HttpClient _httpClient;

    private readonly List<Guid> _createdIds = [];

    public GetCustomerControllerTests(WebApplicationFactory<IApiMarker> webApplicationFactory)
    {
        _httpClient = webApplicationFactory.CreateClient();
    }


    [Fact]
    public async Task Get_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange

        // Act
        var response = await _httpClient.GetAsync($"customers/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        problem!.Title.Should().Be("Not Found");
        problem.Status.Should().Be((int)HttpStatusCode.NotFound);
    }
}