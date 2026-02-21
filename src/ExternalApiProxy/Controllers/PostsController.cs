using ExternalApiProxy.Models;
using ExternalApiProxy.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExternalApiProxy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController(IJsonPlaceholderService service) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Post>> GetById(int id)
    {
        var post = await service.GetPostAsync(id);
        return post is null ? NotFound() : Ok(post);
    }
}
