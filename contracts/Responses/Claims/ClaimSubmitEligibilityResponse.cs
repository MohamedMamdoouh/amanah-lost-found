namespace Amanah.Contracts.Responses.Claims;

public sealed class ClaimSubmitEligibilityResponse
{
    public required bool CanSubmit { get; init; }

    public string? BlockCode { get; init; }
}
