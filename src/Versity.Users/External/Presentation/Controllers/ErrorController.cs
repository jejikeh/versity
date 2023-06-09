using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public sealed class ErrorController : ControllerBase
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error")]
    public IActionResult HandleError()
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception is HttpResponseException httpResponseException)
            return Problem(
                title: httpResponseException.Value?.ToString(), 
                statusCode: httpResponseException.StatusCode);

        return Problem(title: "Ops 🤨! Something went wrong...");
    }
    
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error-development")]
    public IActionResult HandleErrorDevelopment([FromServices] IHostEnvironment hostEnvironment)
    {
        if (!hostEnvironment.IsDevelopment())
            return NotFound();

        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
        var title = exceptionHandlerFeature.Error.Message;
        var statusCode = StatusCodes.Status500InternalServerError;

        if (exceptionHandlerFeature.Error is HttpResponseException httpResponseException)
        {
            title = httpResponseException.Value?.ToString();
            statusCode = httpResponseException.StatusCode;
        }

        return Problem(
            detail: exceptionHandlerFeature.Error.StackTrace,
            title: title,
            statusCode: statusCode);
    }
}