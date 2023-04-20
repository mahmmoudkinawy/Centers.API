namespace Centers.API.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserById(this ClaimsPrincipal claims)
        => Guid.Parse(claims.FindFirst(ClaimTypes.NameIdentifier)?.Value);
}
