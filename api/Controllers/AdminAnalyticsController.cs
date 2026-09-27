using Amanah.Api.Auth;
using Amanah.Api.Services.Admin;
using Amanah.Contracts.Errors;
using Amanah.Contracts.Responses.Admin;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amanah.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/analytics")]
[Authorize(AuthPolicies.Admin)]
public sealed class AdminAnalyticsController(AdminAnalyticsService adminAnalyticsService) : ControllerBase
{
    [HttpGet("overview")]
    [EndpointName(nameof(GetAdminAnalyticsOverview))]
    [EndpointSummary("Snapshot counts for the admin analytics overview.")]
    [ProducesResponseType(typeof(AdminAnalyticsOverviewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiError), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAdminAnalyticsOverview(CancellationToken cancellationToken)
    {
        var overview = await adminAnalyticsService.GetOverviewAsync(cancellationToken);
        return Ok(overview);
    }
}
