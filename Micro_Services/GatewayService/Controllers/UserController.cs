using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Entities;
using Microsoft.AspNetCore.Http.HttpResults;

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
        // Create an HttpClient instance using the factory
        using var client = CreateClient();

        // Send a POST request to the login endpoint
        HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/login", model);

        // Check if the response status code is 200 (OK)
        if (response.StatusCode == HttpStatusCode.OK)
        {
            // You can deserialize the response content here if needed
            var result = await response.Content.ReadFromJsonAsync<UserDTO>();
            return Ok(result);
        }
        else
        {
            return BadRequest("Login failed");
        }
    }

    // api/User/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserCreateModel accountToCreate)
    {
        Console.WriteLine("Tentative de register de " + accountToCreate);
        // Create an HttpClient instance using the factory
        using var client = CreateClient();

        // Send a POST request to the login endpoint
        HttpResponseMessage response = await client.PostAsJsonAsync("api/Users/register", accountToCreate);

        // Check if the response status code is 201 (Created)
        if (response.StatusCode == HttpStatusCode.Created)
        {
            // You can deserialize the response content here if needed
            var result = await response.Content.ReadFromJsonAsync<IActionResult>();
            Console.WriteLine("ok");

            return Ok(result);
        }
        else
        {
            Console.WriteLine("Pas ok");
            return BadRequest("Signup failed");
        }
    }

}
