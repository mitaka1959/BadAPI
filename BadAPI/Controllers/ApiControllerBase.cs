using BadAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BadAPI.Controllers
{
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult HandleFailure(Result result) =>
            result.Status switch
            {
                ResultStatus.NotFound => NotFound(new { error = result.Error }),
                ResultStatus.Conflict => Conflict(new { error = result.Error }),
                ResultStatus.Invalid => BadRequest(new { error = result.Error }),
                _ => BadRequest(new { error = result.Error })
            };
    }
}
