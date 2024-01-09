using System.IdentityModel.Tokens.Jwt;
using Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Blazored.LocalStorage;

namespace Front.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
    private readonly ILocalStorageService _localStorageService;

    public CustomAuthenticationStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task<ClaimsPrincipal> MarkUserAsAuthenticated(UserDTO user, string jwtToken)
    {
        Console.WriteLine("MarkUserAsAuthenticated: Starting authentication process");
        jwtToken = jwtToken.Trim('"');
        await _localStorageService.SetItemAsStringAsync("jwtToken", jwtToken);

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtTokenObject = tokenHandler.ReadJwtToken(jwtToken);
        var claims = jwtTokenObject.Claims.ToList();

        Console.WriteLine($"MarkUserAsAuthenticated: JWT token parsed with {claims.Count} claims");

        if (!claims.Any(c => c.Type == ClaimTypes.Role))
        {
            claims.Add(new Claim(ClaimTypes.Role, "User"));
            Console.WriteLine("MarkUserAsAuthenticated: Added default 'User' role");
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        _currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return _currentUser;
    }
    
    public async Task<ClaimsPrincipal> Logout()
    {
        Console.WriteLine("Logout: User logging out");
        await _localStorageService.RemoveItemAsync("jwtToken");
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return _currentUser;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Console.WriteLine("GetAuthenticationStateAsync: Method called");
        var jwtTokenWithQuotes = await _localStorageService.GetItemAsStringAsync("jwtToken");
        Console.WriteLine($"GetAuthenticationStateAsync: Retrieved JWT token: {jwtTokenWithQuotes}");

        if (!string.IsNullOrEmpty(jwtTokenWithQuotes))
        {
            var jwtToken = jwtTokenWithQuotes.Trim('"');
            var tokenHandler = new JwtSecurityTokenHandler();

            if (tokenHandler.CanReadToken(jwtToken))
            {
                var jwtTokenObject = tokenHandler.ReadJwtToken(jwtToken);
                var claims = jwtTokenObject.Claims.ToList();
                Console.WriteLine($"GetAuthenticationStateAsync: JWT token parsed with {claims.Count} claims");

                if (!claims.Any(c => c.Type == ClaimTypes.Role))
                {
                    claims.Add(new Claim(ClaimTypes.Role, "User"));
                    Console.WriteLine("GetAuthenticationStateAsync: Added default 'User' role");
                }

                var identity = new ClaimsIdentity(claims, "jwtAuthType");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }
            else
            {
                Console.WriteLine("GetAuthenticationStateAsync: Unable to read JWT token");
            }
        }
        else
        {
            Console.WriteLine("GetAuthenticationStateAsync: JWT token is null or empty");
        }

        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
}
