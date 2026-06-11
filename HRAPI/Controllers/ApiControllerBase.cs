using HRAPI.Services;
using Microsoft.AspNetCore.Mvc;

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
}
