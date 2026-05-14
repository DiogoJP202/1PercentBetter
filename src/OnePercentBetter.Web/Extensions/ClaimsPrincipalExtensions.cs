using System.Security.Claims;

namespace OnePercentBetter.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetRequiredUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Authenticated user id was not found.");
    }
}
