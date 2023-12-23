using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Entities;
using Microsoft.IdentityModel.Tokens;

namespace GatewayService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{

    private readonly IHttpClientFactory _httpClientFactory;

    public UserController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
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
        var key = new SymmetricSecurityKey("YourVeryLongSecureKeyHere123456789."u8.ToArray());
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Email, userDto.Email),
            new ("surname", userDto.Surname),
            new ("name", userDto.Name),
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new (JwtRegisteredClaimNames.Sub, userDto.Id.ToString())
        };


        var token = new JwtSecurityToken(
            issuer: "Malik",
            audience: "ISIMA",
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
        if (id != userUpdate.Id)
        {
            return BadRequest("Mismatched user ID");
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
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while updating the user.");
        }
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient("ApiService");
            var response = await client.DeleteAsync($"api/Users/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Ok("User deleted successfully.");
            }
         
            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        }
        catch (Exception)
        {
            return StatusCode(500, "An error occurred while deleting the user.");
        }
    }
}
