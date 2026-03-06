using System.Net;
using AwesomeAssertions;
using Reqnroll;

namespace ExternalApiProxy.IntegrationTests.StepDefinitions;

/// <summary>
/// Step definitions shared across all feature files.
/// The HttpResponseMessage is stored in ScenarioContext by each
/// resource-specific step definition and read here for assertions.
/// </summary>
[Binding]
public sealed class CommonStepDefinitions(ScenarioContext scenarioContext)
{
    [Then("the response status should be {int}")]
    public void ThenTheResponseStatusShouldBe(int statusCode)
    {
        var response = scenarioContext.Get<HttpResponseMessage>();
        response.StatusCode.Should().Be((HttpStatusCode)statusCode);
    }
}
