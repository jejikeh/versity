using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers;

[Route("/health-check")]
public class HealthCheckDockerController : ControllerBase
{
    /// <summary>
    /// This need for Google Cloud deployment.
    /// When deploying to GKE, the backend check pods to 200 Result.
    /// This Endpoint is used in identity-deployment.yaml file `livenessProbe` section.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult HealthCheck()
    {
        return Ok();
    }
}