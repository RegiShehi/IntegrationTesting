using System.Net;
using System.Net.Http.Json;
using Bogus;
using Customers.Api.Contracts.Requests;
using Customers.Api.Contracts.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Customers.Api.Tests.Integration.CustomerController;

// [Collection("CustomerApi Collection")]
public class CreateCustomerControllerTests : IClassFixture<CustomerApiFactory>
{
    private readonly CustomerApiFactory _apiFactory;
    private readonly HttpClient _client;

    private readonly Faker<CustomerRequest> _customerGenerator = new Faker<CustomerRequest>()
        .RuleFor(x => x.Email, faker => faker.Person.Email)
        .RuleFor(x => x.FullName, faker => faker.Person.FullName)
        .RuleFor(x => x.GitHubUsername, CustomerApiFactory.ValidGitHubUsername)
        .RuleFor(x => x.DateOfBirth, faker => faker.Person.DateOfBirth.Date);

    public CreateCustomerControllerTests(CustomerApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
        _client = apiFactory.CreateClient();
    }

    [Fact]
    public async Task Create_CreatesUser_WhenDataIsValid()
    {
        // arrange
        var customer = _customerGenerator.Generate();

        // act
        var response = await _client.PostAsJsonAsync("customers", customer);

        // assert
        var customerResponse = await response.Content.ReadFromJsonAsync<CustomerResponse>();
        customerResponse.Should().BeEquivalentTo(customer);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().Be($"http://localhost/customers/{customerResponse!.Id}");
    }

    [Fact]
    public async Task Create_ReturnsValidationError_WhenEmailIsInvalid()
    {
        // arrange
        var invalidEmail = new Faker().Random.Word();

        var customer = _customerGenerator.Clone()
            .RuleFor(x => x.Email, invalidEmail)
            .Generate();

        // act
        var response = await _client.PostAsJsonAsync("customers", customer);

        // assert
        var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error!.Status.Should().Be((int)HttpStatusCode.BadRequest);
        error!.Title.Should().Be("One or more validation errors occurred.");
        error.Errors["Email"][0].Should().Be($"{invalidEmail} is not a valid email address");
    }

    [Fact]
    public async Task Create_ReturnsValidationError_WhenGithubUserDoesNotExist()
    {
        // arrange
        var invalidGithubUser = new Faker().Random.Word();

        var customer = _customerGenerator.Clone()
            .RuleFor(x => x.GitHubUsername, invalidGithubUser)
            .Generate();

        // act
        var response = await _client.PostAsJsonAsync("customers", customer);

        // assert
        var error = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        error!.Status.Should().Be((int)HttpStatusCode.BadRequest);
        error!.Title.Should().Be("One or more validation errors occurred.");
        error.Errors["Customer"][0].Should().Be($"There is no GitHub user with username {invalidGithubUser}");
    }
}