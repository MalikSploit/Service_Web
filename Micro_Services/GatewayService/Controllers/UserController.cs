using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourVeryLongSecureKeyHere123456789."));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, userDto.Email),
            new Claim("surname", userDto.Surname),
            new Claim("name", userDto.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "Malik",
            audience: "ISIMA",
            claims: claims,
            expires: DateTime.Now.AddMinutes(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLogin model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Pass))
        {
            //Console.WriteLine("Login attempt with incomplete credentials.");
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

                //Console.WriteLine($"Successful login for user: {model.Email}");
                return Ok(userDto);
            }
            //Console.WriteLine($"Failed login attempt for user: {model.Email}. Status code: {response.StatusCode}");
            return StatusCode((int)response.StatusCode, "Login failed");
        }
        catch (Exception)
        {
            //Console.WriteLine($"Exception occurred during login for user: {model.Email}. Error: {ex.Message}");
            return StatusCode(500, "An internal server error occurred");
        }
    }

    // api/User/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserCreateModel accountToCreate)
    {
        //Console.WriteLine("Tentative de register de " + accountToCreate);
        using var client = CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/register", accountToCreate);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<UserDTO>();
            //Console.WriteLine("ok");
            return Ok(result);
        }
       
        var errorResponse = await response.Content.ReadAsStringAsync();
        //Console.WriteLine($"Registration error response: {errorResponse}");
        return BadRequest(errorResponse);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateModel userUpdate)
    {
        if (id != userUpdate.Id)
        {
            return BadRequest("Mismatched user ID");
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
