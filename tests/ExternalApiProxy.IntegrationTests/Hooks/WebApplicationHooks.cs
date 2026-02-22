using ExternalApiProxy.IntegrationTests.Support;
using Reqnroll;
using Reqnroll.BoDi;
using WireMock.Server;

namespace ExternalApiProxy.IntegrationTests.Hooks;

/// <summary>
/// Reqnroll hooks that manage the WebApplicationFactory and HttpClient lifecycle.
/// The factory is configured to point the application's HttpClient at the WireMock
/// server, so all outgoing requests from the API are intercepted by WireMock.
/// </summary>
[Binding]
public sealed class WebApplicationHooks(IObjectContainer container)
{
    [BeforeScenario(Order = 10)]
    public void CreateHttpClient()
    {
        var wireMockServer = container.Resolve<WireMockServer>();

        var factory = new TestWebApplicationFactory()
            .WithJsonPlaceholderUrl(wireMockServer.Url!);

        var httpClient = factory.CreateClient();

        container.RegisterInstanceAs(factory);
        container.RegisterInstanceAs(httpClient);
    }

    [AfterScenario(Order = 10)]
    public void DisposeHttpClient()
    {
        container.Resolve<HttpClient>().Dispose();
        container.Resolve<TestWebApplicationFactory>().Dispose();
    }
}
