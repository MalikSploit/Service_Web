using Microsoft.AspNetCore.Mvc;
using Entities;

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

    private static Uri UserServiceUri => new Uri("http://localhost:5001/");
    private HttpClient CreateClient() 
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = UserServiceUri;
        return client;
    }

    // api/User/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLogin model)
    {
        Console.WriteLine(model.Email);
        Console.WriteLine(model.Pass);
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Pass))
        {
            Console.WriteLine("Login attempt with incomplete credentials.");
            return BadRequest("Email and password are required.");
        }

        try
        {
            using var client = _httpClientFactory.CreateClient("ApiService");

            var response = await client.PostAsJsonAsync("api/Users/login", model);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<UserDTO>();
                Console.WriteLine($"Successful login for user: {model.Email}");
                return Ok(result);
            }
            Console.WriteLine($"Failed login attempt for user: {model.Email}. Status code: {response.StatusCode}");
            return StatusCode((int)response.StatusCode, "Login failed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred during login for user: {model.Email}. Error: {ex.Message}");
            return StatusCode(500, "An internal server error occurred");
        }
    }


    // api/User/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserCreateModel accountToCreate)
    {
        Console.WriteLine("Tentative de register de " + accountToCreate);
        using var client = CreateClient();

        HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/register", accountToCreate);

        if (response.IsSuccessStatusCode)
        {
            // Assuming UserDTO is the expected response type
            var result = await response.Content.ReadFromJsonAsync<UserDTO>();
            Console.WriteLine("ok");
            return Ok(result);
        }
        else
        {
            var errorResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Registration error response: {errorResponse}");
            return BadRequest(errorResponse);
        }
    }

}
