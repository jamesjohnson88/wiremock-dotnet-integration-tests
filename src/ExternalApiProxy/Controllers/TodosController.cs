using ExternalApiProxy.Models;
using ExternalApiProxy.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExternalApiProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodosController(IJsonPlaceholderService service) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Todo>> GetById(int id)
    {
        var todo = await service.GetTodoAsync(id);
        return todo is null ? NotFound() : Ok(todo);
    }
}
