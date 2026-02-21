using System.Net;
using System.Net.Http.Json;
using ExternalApiProxy.Models;

namespace ExternalApiProxy.Services;

public class JsonPlaceholderService(HttpClient httpClient) : IJsonPlaceholderService
{
    public Task<Todo?> GetTodoAsync(int id) => GetAsync<Todo>($"todos/{id}");

    public Task<Post?> GetPostAsync(int id) => GetAsync<Post>($"posts/{id}");

    public Task<User?> GetUserAsync(int id) => GetAsync<User>($"users/{id}");

    private async Task<T?> GetAsync<T>(string path)
    {
        var response = await httpClient.GetAsync(path);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>();
    }
}
