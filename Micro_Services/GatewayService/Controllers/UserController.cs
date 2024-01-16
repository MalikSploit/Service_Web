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

/*
    * Class: UserController
    * -----------------------
    * This class is the controller for the User API. It contains methods for
    * GET, POST, PUT, and DELETE requests.
    * By default there are 3 admin users.
    * This class also contains a method to generate a JWT token.
*/
public class UserController(
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    JwtTokenValidationService jwtTokenValidationService)
    : ControllerBase
{
    private static Uri UserServiceUri => new ("http://localhost:5001/");
    private readonly List<string> adminEmails =
    [
        "Malik@gmail.com",
        "thomas@gmail.com",
        "Thibault@gmail.com"
    ];

    private HttpClient CreateClient() 
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = UserServiceUri;
        return client;
    }
    
    private string? GenerateJwtToken(UserDTO userDto)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];

        if (secretKey == null) return null;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        if (userDto.Name == null) return null;
        if (userDto is not { Surname: not null, Email: not null }) return null;
        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Email, userDto.Email),
            new ("surname", userDto.Surname),
            new ("name", userDto.Name),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Sub, userDto.Id.ToString())
        };

        var role = adminEmails.Contains(userDto.Email) ? "Admin" : "User";
        claims.Add(new Claim(ClaimTypes.Role, role));
        
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // api/User/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLogin model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Pass))
        {
            return BadRequest("Email and password are required.");
        }

        try
        {
            using var client = httpClientFactory.CreateClient("ApiService");

            var response = await client.PostAsJsonAsync("api/Users/login", model);

            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, "Login failed");
            var userDto = await response.Content.ReadFromJsonAsync<UserDTO>();
            if (userDto == null) return Ok(userDto);
            var token = GenerateJwtToken(userDto);
            userDto.Token = token;

            return Ok(userDto);
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

        var response = await client.PostAsJsonAsync("api/Users/register", accountToCreate);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<UserDTO>();
            return Ok(result);
        }
       
        var errorResponse = await response.Content.ReadAsStringAsync();
        return BadRequest(errorResponse);
    }
    
    // api/User/id
    [HttpPut("{id:int}")]
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
            principal = jwtTokenValidationService.ValidateToken(token);
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
            using var client = httpClientFactory.CreateClient("ApiService");
            var response = await client.PutAsJsonAsync($"api/Users/{id}", userUpdate);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            
            var updatedUser = await GetUserDetails(id);

            if (updatedUser == null) return StatusCode(500, "Failed to retrieve updated user data.");
            var newToken = GenerateJwtToken(updatedUser); 
            updatedUser.Token = newToken;
            return Ok(updatedUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while updating the user: {ex.Message}");
        }
    }
    
    private async Task<UserDTO?> GetUserDetails(int id)
    {
        using var client = httpClientFactory.CreateClient("ApiService");
        var response = await client.GetAsync($"api/Users/{id}");

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserDTO>();
        }
        return null;
    }
    
    // api/User/id
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var tokenWithQuotes = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(tokenWithQuotes))
            {
                return Unauthorized("Authorization token is missing.");
            }
            
            var token = tokenWithQuotes.Trim('"');

            ClaimsPrincipal principal;
            try
            {
                principal = jwtTokenValidationService.ValidateToken(token);
            }
            catch (Exception)
            {
                return Unauthorized("Invalid token.");
            }
            
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
            if (string.IsNullOrEmpty(userIdClaim) || int.Parse(userIdClaim) != id)
            {
                return Unauthorized("You can only delete your own account.");
            }
            
            using var client = httpClientFactory.CreateClient("ApiService");
            var response = await client.DeleteAsync($"api/Users/{id}");

            return response.IsSuccessStatusCode ? Ok("User deleted successfully.") : StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while deleting the user: {ex.Message}");
        }
    }
    
    [HttpGet("cart/{userId:int}")]
    public async Task<ActionResult<string>> GetCart(int userId)
    {
        using var client = httpClientFactory.CreateClient("ApiService");
        var response = await client.GetAsync($"api/Users/cart/{userId}");

        if (!response.IsSuccessStatusCode)
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        var cartJson = await response.Content.ReadAsStringAsync();
        return Ok(cartJson); 
    }
    
    [HttpPut("cart/{userId:int}")]
    public async Task<IActionResult> UpdateCart(int userId, [FromBody] CartUpdateModel model)
    {
        using var client = httpClientFactory.CreateClient("ApiService");
        var response = await client.PutAsJsonAsync($"api/Users/cart/{userId}", model);

        if (response.IsSuccessStatusCode)
        {
            return NoContent();
        }
        return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
    }
}

public class CartUpdateModel
{
    public string CartJson { get; set; } = string.Empty;
}
