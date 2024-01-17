using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GatewayService.Services;

/*
    * Class: JwtTokenValidationService
    * -----------------------
    * This class is used to validate a JWT token.
*/
public class JwtTokenValidationService(
    IOptions<TokenValidationParameters> tokenValidationParameters,
    IHttpContextAccessor httpContextAccessor)
{
    private readonly TokenValidationParameters _tokenValidationParameters = tokenValidationParameters.Value;

    private ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.ValidateToken(token, _tokenValidationParameters, out _);
    }
    
    public bool IsUserAuthorized(int userId, out string errorMessage)
    {
        errorMessage = string.Empty;
        var httpContext = httpContextAccessor.HttpContext;
        var tokenWithQuotes = httpContext?.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(tokenWithQuotes))
        {
            errorMessage = "Authorization token is missing.";
            return false;
        }

        var token = tokenWithQuotes.Trim('"');

        try
        {
            var principal = ValidateToken(token);
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || int.Parse
                    (userIdClaim) != userId)
            {
                errorMessage = "You are not authorized to access this resource.";
                return false;
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Invalid token: {ex.Message}";
            return false;
        }
        return true; // User is authorized
    }
    
    public bool IsUserAdmin(string token, out string errorMessage)
    {
        errorMessage = string.Empty;
        ClaimsPrincipal principal;

        try
        {
            principal = ValidateToken(token);
        }
        catch (Exception ex)
        {
            errorMessage = $"Invalid token: {ex.Message}";
            return false;
        }

        var userRoleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;
        if (userRoleClaim == null || !userRoleClaim.Equals("admin", StringComparison.OrdinalIgnoreCase))
        {
            errorMessage = "Access denied. Admin role required.";
            return false;
        }

        return true;
    }
}