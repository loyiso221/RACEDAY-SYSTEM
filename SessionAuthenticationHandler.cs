using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
namespace RaceDay.Api.Security;
public sealed class SessionAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public SessionAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder) { }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var id=Context.Session.GetInt32("UserId");
        var role=Context.Session.GetString("Role");
        if(id is null || string.IsNullOrWhiteSpace(role)) return Task.FromResult(AuthenticateResult.NoResult());
        var claims=new[]{new Claim(ClaimTypes.NameIdentifier,id.Value.ToString()),new Claim(ClaimTypes.Name,Context.Session.GetString("Email")??""),new Claim(ClaimTypes.Role,role)};
        var identity=new ClaimsIdentity(claims,Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity),Scheme.Name)));
    }
    protected override Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode=StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    }
    protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode=StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    }
}
