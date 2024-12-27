using Microsoft.AspNetCore.Mvc.Testing;

namespace Customers.Api.Tests.Integration;

[CollectionDefinition("CustomerApi Collection")]
public class CustomerApiCollection : ICollectionFixture<WebApplicationFactory<IApiMarker>>
{
}