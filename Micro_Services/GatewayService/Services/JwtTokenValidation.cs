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
public class JwtTokenValidationService
{
    private readonly TokenValidationParameters _tokenValidationParameters;

    public JwtTokenValidationService(IOptions<TokenValidationParameters> tokenValidationParameters)
    {
        _tokenValidationParameters = tokenValidationParameters.Value;
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.ValidateToken(token, _tokenValidationParameters, out var validatedToken);
    }
}