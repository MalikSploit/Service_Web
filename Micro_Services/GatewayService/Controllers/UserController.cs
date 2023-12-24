using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Entities;
using GatewayService.Services;
using Microsoft.IdentityModel.Tokens;

namespace GatewayService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly JwtTokenValidationService _jwtTokenValidationService;

    public UserController(IHttpClientFactory httpClientFactory, IConfiguration configuration, JwtTokenValidationService jwtTokenValidationService)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _jwtTokenValidationService = jwtTokenValidationService;
    }

    private static Uri UserServiceUri => new ("http://localhost:5001/");
    private HttpClient CreateClient() 
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = UserServiceUri;
        return client;
    }
    
    private string GenerateJwtToken(UserDTO userDto)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, userDto.Email),
            new Claim("surname", userDto.Surname),
            new Claim("name", userDto.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userDto.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLogin model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Pass))
        {
            return BadRequest("Email and password are required.");
        }

        try
        {
            using var client = _httpClientFactory.CreateClient("ApiService");

            var response = await client.PostAsJsonAsync("api/Users/login", model);

            if (response.IsSuccessStatusCode)
            {
                var userDto = await response.Content.ReadFromJsonAsync<UserDTO>();
                var token = GenerateJwtToken(userDto);
                userDto.Token = token;
                
                return Ok(userDto);
            }
            return StatusCode((int)response.StatusCode, "Login failed");
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal server error occurred");
        }
    }

    // api/User/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserCreateModel accountToCreate)
    {
        if (!accountToCreate.Email.IsEmailValid())
        {
            return BadRequest("Invalid email format.");
        }
        if (!accountToCreate.Name.IsNameValid())
        {
            return BadRequest("Invalid name format, minimum 3 characters");
        }
        if (!accountToCreate.Surname.IsSurnameValid())
        {
            return BadRequest("Invalid surname format, minimum 3 characters");
        }
        if (!accountToCreate.Password.IsPasswordRobust())
        {
            return BadRequest("Invalid password format, minimum 8 characters, at least one uppercase letter, one lowercase letter, one digit, and one special character");
        }
        
        using var client = CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/register", accountToCreate);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<UserDTO>();
            return Ok(result);
        }
       
        var errorResponse = await response.Content.ReadAsStringAsync();
        return BadRequest(errorResponse);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateModel userUpdate)
    {
        var tokenWithQuotes = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(tokenWithQuotes))
        {
            return Unauthorized("Authorization token is missing.");
        }
            
        var token = tokenWithQuotes.Trim('"');

        ClaimsPrincipal principal;
        try
        {
            principal = _jwtTokenValidationService.ValidateToken(token);
        }
        catch
        {
            return Unauthorized("Invalid token.");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || int.Parse(userIdClaim) != id)
        {
            return Unauthorized("You can only update your own account.");
        }

        if (!userUpdate.Name.IsNameValid())
        {
            return BadRequest("Invalid name format, minimum 3 characters");
        }
        if (!userUpdate.Surname.IsSurnameValid())
        {
            return BadRequest("Invalid surname format, minimum 3 characters");
        }
        if (!userUpdate.Email.IsEmailValid())
        {
            return BadRequest("Invalid email format.");
        }

        try
        {
            using var client = _httpClientFactory.CreateClient("ApiService");
            var response = await client.PutAsJsonAsync($"api/Users/{id}", userUpdate);

            if (response.IsSuccessStatusCode)
            {
                return Ok("User updated successfully.");
            } 
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the user: {ex.Message}");
        }
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var tokenWithQuotes = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(tokenWithQuotes))
            {
                return Unauthorized("Authorization token is missing.");
            }
            
            var token = tokenWithQuotes.Trim('"');

            ClaimsPrincipal principal;
            try
            {
                Console.WriteLine("Before");
                principal = _jwtTokenValidationService.ValidateToken(token);
                Console.WriteLine("After");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error {ex}");
                return Unauthorized("Invalid token.");
            }
            
            // Extract user ID from token
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
            if (string.IsNullOrEmpty(userIdClaim) || int.Parse(userIdClaim) != id)
            {
                return Unauthorized("You can only delete your own account.");
            }

            // Proceed with user deletion
            using var client = _httpClientFactory.CreateClient("ApiService");
            var response = await client.DeleteAsync($"api/Users/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Ok("User deleted successfully.");
            }

            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the user: {ex.Message}");
        }
    }
}
