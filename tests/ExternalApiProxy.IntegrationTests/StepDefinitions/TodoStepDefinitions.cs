using System.Net.Http.Json;
using ExternalApiProxy.Models;
using AwesomeAssertions;
using Reqnroll;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace ExternalApiProxy.IntegrationTests.StepDefinitions;

[Binding]
public sealed class TodoStepDefinitions(
    WireMockServer wireMock,
    HttpClient httpClient,
    ScenarioContext scenarioContext)
{
    [Given("the upstream service will return a todo with id {int}")]
    public void GivenTheUpstreamServiceWillReturnATodoWithId(int id)
    {
        var todo = new { id, userId = 1, title = $"Todo item {id}", completed = false };

        wireMock.Given(
            Request.Create()
                .WithPath($"/todos/{id}")
                .UsingGet())
            .WithTitle($"GET /todos/{id}")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(200)
                    .WithBodyAsJson(todo));
    }

    [Given("the upstream service will return 404 for todo with id {int}")]
    public void GivenTheUpstreamServiceWillReturn404ForTodoWithId(int id)
    {
        wireMock.Given(
            Request.Create()
                .WithPath($"/todos/{id}")
                .UsingGet())
            .WithTitle($"GET /todos/{id} - 404")
            .RespondWith(
                Response.Create()
                    .WithStatusCode(404));
    }

    [When("I request the todo with id {int}")]
    public async Task WhenIRequestTheTodoWithId(int id)
    {
        var response = await httpClient.GetAsync($"/api/todos/{id}");
        scenarioContext.Set(response);
    }

    [Then("the response body should contain a todo with id {int}")]
    public async Task ThenTheResponseBodyShouldContainATodoWithId(int id)
    {
        var response = scenarioContext.Get<HttpResponseMessage>();
        var todo = await response.Content.ReadFromJsonAsync<Todo>();
        todo.Should().NotBeNull();
        todo!.Id.Should().Be(id);
    }
}
