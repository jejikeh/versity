using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

public sealed class ErrorController : ControllerBase
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error")]
    public IActionResult HandleError()
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;
        if (exception is ExceptionWithStatusCode httpResponseException)
        {
            _logger.LogError("Request failure, {@Error}", httpResponseException.Message);
            
            return Problem(
                title: httpResponseException.ErrorMessage,
                statusCode: httpResponseException.StatusCode);
        }
        
        _logger.LogError("Request failure, {@Error}", exception.Message);
        
        return Problem(title: "Ops 🤨! Something went wrong...");
    }
    
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("/error-development")]
    public IActionResult HandleErrorDevelopment([FromServices] IHostEnvironment hostEnvironment)
    {
        if (!hostEnvironment.IsDevelopment())
        {
            return NotFound();
        }
        
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>()!;
        var title = exceptionHandlerFeature.Error.Message;
        var statusCode = StatusCodes.Status500InternalServerError;
        
        if (exceptionHandlerFeature.Error is ExceptionWithStatusCode httpResponseException)
        {
            title = httpResponseException.ErrorMessage;
            statusCode = httpResponseException.StatusCode;
        }
        
        _logger.LogError("Request failure, {@Error}, debug_trace {@Trace}", title, exceptionHandlerFeature.Error.StackTrace);
        
        return Problem(
            detail: exceptionHandlerFeature.Error.StackTrace,
            title: title,
            statusCode: statusCode);
    }
}