using Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;

namespace Front.Components.Pages;

public partial class Signup : ComponentBase
{
    /*
    private readonly IHttpClientFactory _httpClientFactory;

    public Signup(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private static Uri GatewaysServiceUri => new Uri("http://localhost:5000/");
    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = GatewaysServiceUri;
        return client;
    }

    private UserCreateModel userCreateModel = new UserCreateModel();

    private async Task HandleSignup()
    {
        var apiUrl = "https://your-other-microservice-url.com/api/User/register";

        HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, userCreateModel);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<User>();
            // Process the successful registration here if needed
        }
        else
        {
            var errorMessage = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Signup failed: {errorMessage}");
            // Handle failed signup
        }
    }*/
}