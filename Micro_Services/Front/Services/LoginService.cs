using Entities;
using Newtonsoft.Json;
using System.Text;

namespace Front.Services;

public class LoginService
{
    public static readonly string Urlprefix = "http://localhost:5000/";
    private readonly HttpClient _httpClient;

    public LoginService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(bool isSuccess, UserDTO? userDto, string errorMessage)> AuthenticateUserAsync(string email, string pass)
    {
        var payload = new { email, pass };
        var jsonPayload = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var apiUrl = Urlprefix + "api/User/login";
        var response = await _httpClient.PostAsync(apiUrl, content);
    
        if (response.IsSuccessStatusCode)
        {
            var userDto = JsonConvert.DeserializeObject<UserDTO>(await response.Content.ReadAsStringAsync());
            return (true, userDto, string.Empty);
        }
       
        var errorMessage = "Authentication failed. Please check your credentials and try again.";
        
        Console.WriteLine("Authentication failed with status code: " + response.StatusCode);
        return (false, null, errorMessage);
    }
}