using System.Security.Claims;
namespace RaceDay.Api.Security;
public static class SessionPrincipal
{
    public static ClaimsPrincipal Create(int userId, string role, string email) =>
        new(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, role)
        }, "RaceDaySession"));
}
