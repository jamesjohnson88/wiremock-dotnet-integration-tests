using System.Net.Http.Json;
using ExternalApiProxy.Models;
using FluentAssertions;
using Reqnroll;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ExternalApiProxy.IntegrationTests.StepDefinitions;

[Binding]
public sealed class UserStepDefinitions(
    WireMockServer wireMock,
    HttpClient httpClient,
    ScenarioContext scenarioContext)
{
    [Given("the upstream service will return a user with id {int}")]
    public void GivenTheUpstreamServiceWillReturnAUserWithId(int id)
    {
        var user = new { id, name = $"User {id}", username = $"user{id}", email = $"user{id}@example.com" };

        wireMock.Given(
            Request.Create()
                .WithPath($"/users/{id}")
                .UsingGet())
            .WithTitle($"GET /users/{id}")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(user));
    }

    [Given("the upstream service will return 404 for user with id {int}")]
    public void GivenTheUpstreamServiceWillReturn404ForUserWithId(int id)
    {
        wireMock.Given(
            Request.Create()
                .WithPath($"/users/{id}")
                .UsingGet())
            .WithTitle($"GET /users/{id} - 404")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(404));
    }

    [When("I request the user with id {int}")]
    public async Task WhenIRequestTheUserWithId(int id)
    {
        var response = await httpClient.GetAsync($"/api/users/{id}");
        scenarioContext.Set(response);
    }

    [Then("the response body should contain a user with id {int}")]
    public async Task ThenTheResponseBodyShouldContainAUserWithId(int id)
    {
        var response = scenarioContext.Get<HttpResponseMessage>();
        var user = await response.Content.ReadFromJsonAsync<User>();
        user.Should().NotBeNull();
        user!.Id.Should().Be(id);
    }
}
