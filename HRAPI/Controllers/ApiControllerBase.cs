using HRAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HRAPI.Controllers;

[ApiController]
// Shared controller base that converts service-layer results into standard HTTP responses.
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToActionResult(ServiceResult result)
    {
        if (result.Succeeded)
            return NoContent();

        return result.NotFound ? NotFound() : BadRequest(result.ErrorMessage);
    }
}
