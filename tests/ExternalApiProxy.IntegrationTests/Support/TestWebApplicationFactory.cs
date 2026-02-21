using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ExternalApiProxy.IntegrationTests.Support;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private string? _jsonPlaceholderBaseUrl;

    public TestWebApplicationFactory WithJsonPlaceholderUrl(string url)
    {
        _jsonPlaceholderBaseUrl = url;
        return this;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var overrides = new Dictionary<string, string?>
            {
                ["ExternalApis:JsonPlaceholder:BaseUrl"] = _jsonPlaceholderBaseUrl
            };
            config.AddInMemoryCollection(overrides);
        });
    }
}
