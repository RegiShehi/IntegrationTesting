namespace Customers.Api.Tests.Integration.CustomerController;

// [Collection("CustomerApi Collection")]
public class CreateCustomerControllerTests : IClassFixture<CustomerApiFactory>
{
    private readonly CustomerApiFactory _apiFactory;

    public CreateCustomerControllerTests(CustomerApiFactory apiFactory)
    {
        _apiFactory = apiFactory;
    }

    [Fact]
    public async Task Test()
    {
        await Task.Delay(2000);
    }
}