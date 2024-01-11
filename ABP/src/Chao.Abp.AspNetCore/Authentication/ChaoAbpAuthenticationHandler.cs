using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace Chao.Abp.AspNetCore.Authentication;

public class ChaoAbpAuthenticationHandler : IAuthenticationHandler
{
    private HttpContext? _context;
    private AuthenticationScheme? _scheme;

    public virtual async Task<AuthenticateResult> AuthenticateAsync()
    {
        if (_context!.User!.Identity!.IsAuthenticated == true)
        {
            return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(_context.User, _scheme!.Name)));
        }
        else
        {
            return AuthenticateResult.Fail("No principal.");
        }
    }

    public virtual async Task ChallengeAsync(AuthenticationProperties? properties)
    {
        _context!.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        await Task.CompletedTask;
    }

    public virtual async Task ForbidAsync(AuthenticationProperties? properties)
    {
        _context!.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        await Task.CompletedTask;
    }

    public virtual async Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
    {
        _scheme = scheme;
        _context = context;
        await Task.CompletedTask;
    }
}