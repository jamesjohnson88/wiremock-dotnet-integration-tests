using System.Net.Http.Json;
using ExternalApiProxy.Models;
using FluentAssertions;
using Reqnroll;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ExternalApiProxy.IntegrationTests.StepDefinitions;

[Binding]
public sealed class PostStepDefinitions(
    WireMockServer wireMock,
    HttpClient httpClient,
    ScenarioContext scenarioContext)
{
    [Given("the upstream service will return a post with id {int}")]
    public void GivenTheUpstreamServiceWillReturnAPostWithId(int id)
    {
        var post = new { id, userId = 1, title = $"Post title {id}", body = $"Post body for {id}" };

        wireMock.Given(
            Request.Create()
                .WithPath($"/posts/{id}")
                .UsingGet())
            .WithTitle($"GET /posts/{id}")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(post));
    }

    [Given("the upstream service will return 404 for post with id {int}")]
    public void GivenTheUpstreamServiceWillReturn404ForPostWithId(int id)
    {
        wireMock.Given(
            Request.Create()
                .WithPath($"/posts/{id}")
                .UsingGet())
            .WithTitle($"GET /posts/{id} - 404")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(404));
    }

    [When("I request the post with id {int}")]
    public async Task WhenIRequestThePostWithId(int id)
    {
        var response = await httpClient.GetAsync($"/api/posts/{id}");
        scenarioContext.Set(response);
    }

    [Then("the response body should contain a post with id {int}")]
    public async Task ThenTheResponseBodyShouldContainAPostWithId(int id)
    {
        var response = scenarioContext.Get<HttpResponseMessage>();
        var post = await response.Content.ReadFromJsonAsync<Post>();
        post.Should().NotBeNull();
        post!.Id.Should().Be(id);
    }
}
