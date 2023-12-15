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

    public async Task<UserDTO> AuthenticateUserAsync(string email, string pass)
    {
        var payload = new { email, pass }; // Create an anonymous object with email and password
        var jsonPayload = JsonConvert.SerializeObject(payload); // Serialize the object to JSON
        //Console.WriteLine("The content is: " + jsonPayload); // Log the JSON string

        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var apiUrl = Urlprefix + "api/User/login";

        var response = await _httpClient.PostAsync(apiUrl, content);
    
        if (response.IsSuccessStatusCode)
        {
            var userDto = JsonConvert.DeserializeObject<UserDTO>(await response.Content.ReadAsStringAsync());
            return userDto;
        }
        else
        {
            // Handle authentication failure
            Console.WriteLine("Authentication failed with status code: " + response.StatusCode);
            return null;
        }
    }

}
