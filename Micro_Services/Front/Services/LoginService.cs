using Entities;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace Front.Services;

public class LoginService
{
    public static readonly string URIprefix = "http://localhost:5000/";
    private readonly HttpClient _httpClient;

    public LoginService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserDTO> AuthenticateUserAsync(string email, string password)
    {
        var content = new StringContent(JsonConvert.SerializeObject(new { email, password }), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/api/Users/login", content);
        
        if (response.IsSuccessStatusCode)
        {
            // Deserialize the response if authentication is successful
            var userDto = JsonConvert.DeserializeObject<UserDTO>(await response.Content.ReadAsStringAsync());
            return userDto;
        }
        else
        {
            // Handle authentication failure
            return null;
        }
    }

    public UserDTO AuthenticateUser(string email, string password)
    {
        return new UserDTO
        {
            Id = 0,
            Email = email
        };
    }
}
