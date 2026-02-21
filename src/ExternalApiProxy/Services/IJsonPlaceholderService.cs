using ExternalApiProxy.Models;

namespace ExternalApiProxy.Services;

public interface IJsonPlaceholderService
{
    Task<Todo?> GetTodoAsync(int id);
    Task<Post?> GetPostAsync(int id);
    Task<User?> GetUserAsync(int id);
}
