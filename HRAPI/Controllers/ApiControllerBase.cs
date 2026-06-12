using HRAPI.Models.Hateoas;
using HRAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace HRAPI.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToActionResult(ServiceResult result)
    {
        if (result.Succeeded)
            return NoContent();

        return result.NotFound ? NotFound() : BadRequest(result.ErrorMessage);
    }

    protected async Task EvictCacheAsync(string tag)
    {
        if (HttpContext?.RequestServices == null) return;
        var store = HttpContext.RequestServices.GetRequiredService<IOutputCacheStore>();
        await store.EvictByTagAsync(tag, CancellationToken.None);
    }

    protected List<Link> Links(params (string rel, string? href, string method)[] linkDefs)
    {
        var links = new List<Link>();
        foreach (var (rel, href, method) in linkDefs)
        {
            if (href != null)
                links.Add(new Link(href, rel, method));
        }
        return links;
    }
}
