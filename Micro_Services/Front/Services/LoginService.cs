using System.IdentityModel.Tokens.Jwt;
using Entities;
using Newtonsoft.Json;
using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Front.Services;

public class LoginService(HttpClient httpClient, ILocalStorageService localStorage)
{
    [Inject] private ILocalStorageService LocalStorage { get; set; } = localStorage;

    public const string Urlprefix = "http://localhost:5000/";

    public async Task<(bool isSuccess, UserDTO? userDto, string errorMessage)> AuthenticateUserAsync(string email, string pass)
    {
        var payload = new { email, pass };
        var jsonPayload = JsonConvert.SerializeObject(payload);
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

        var apiUrl = Urlprefix + "api/User/login";
        var response = await httpClient.PostAsync(apiUrl, content);
    
        if (response.IsSuccessStatusCode)
        {
            var userDto = JsonConvert.DeserializeObject<UserDTO>(await response.Content.ReadAsStringAsync());
            return (true, userDto, string.Empty);
        }
       
        var errorMessage = response.StatusCode switch
        {
            System.Net.HttpStatusCode.Unauthorized => "Invalid credentials. Please try again.",
            System.Net.HttpStatusCode.BadRequest => "Bad request. Check your input.",
            System.Net.HttpStatusCode.InternalServerError => "Server error. Try again later.",
            _ => "Authentication failed. Please try again."
        };
        
        Console.WriteLine($"Authentication failed with status code: {response.StatusCode}");
        return (false, null, errorMessage);
    }
    
    public async Task<bool> IsUserLoggedIn()
    {
        var jwtTokenWithQuotes = await LocalStorage.GetItemAsStringAsync("jwtToken");
        if (string.IsNullOrEmpty(jwtTokenWithQuotes))
        {
            return false;
        }

        var jwtToken = jwtTokenWithQuotes.Trim('"');
        var tokenHandler = new JwtSecurityTokenHandler();

        if (!tokenHandler.CanReadToken(jwtToken))
        {
            await LocalStorage.RemoveItemAsync("jwtToken");
            return false;
        }

        try
        {
            var jwtSecurityToken = tokenHandler.ReadJwtToken(jwtToken);
            var expClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);

            if (expClaim != null && long.TryParse(expClaim.Value, out var exp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(exp) > DateTimeOffset.UtcNow;
            }

            await LocalStorage.RemoveItemAsync("jwtToken");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error processing JWT token: " + ex.Message);
            await LocalStorage.RemoveItemAsync("jwtToken");
            return false;
        }
    }
}