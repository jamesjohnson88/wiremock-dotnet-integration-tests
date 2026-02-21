using ExternalApiProxy.Models;
using ExternalApiProxy.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExternalApiProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IJsonPlaceholderService service) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await service.GetUserAsync(id);
        return user is null ? NotFound() : Ok(user);
    }
}
