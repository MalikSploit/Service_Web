using System.Text;
using Microsoft.AspNetCore.Components;
using Entities;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Blazored.LocalStorage;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Front.Components.Pages;

public partial class Profile
{
    #pragma warning disable CS8618
    private string confirmPassword;
    private UserUpdateModel userUpdateModel = new ();
    private string errorMessage = string.Empty;
    private string successMessage = string.Empty;
    private bool isDropdownOpen;
    private bool showConfirmationDialog;

    [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
    [Inject] private ILocalStorageService LocalStorage { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private HttpClient HttpClient { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var jwtToken = await LocalStorage.GetItemAsStringAsync("jwtToken");
        if (!string.IsNullOrEmpty(jwtToken))
        {
            var jwtPayload = ParseJwtPayload(jwtToken);
            userUpdateModel.Email = jwtPayload.GetValueOrDefault("email")?.ToString();
            userUpdateModel.Name = jwtPayload.GetValueOrDefault("name")?.ToString();
            userUpdateModel.Surname = jwtPayload.GetValueOrDefault("surname")?.ToString();
        }
    }

    private Dictionary<string, object> ParseJwtPayload(string jwtToken)
    {
        var parts = jwtToken.Split('.');
        if (parts.Length == 3)
        {
            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var jsonPayload = Encoding.UTF8.GetString(jsonBytes);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonPayload);
        }
        return null;
    }
    
    private byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }
    
    private string GetUserIdFromJwtToken(string jwtToken)
    {
        var parts = jwtToken.Split('.');
        if (parts.Length == 3)
        {
            var payload = parts[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var jsonPayload = Encoding.UTF8.GetString(jsonBytes);
            var jwtPayload = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonPayload);

            if (jwtPayload != null && jwtPayload.TryGetValue(JwtRegisteredClaimNames.Sub, out var userId))
            {
                return userId.ToString();
            }
        }
        return null;
    }

    private async Task HandleValidSubmit()
    {
        errorMessage = string.Empty;
        successMessage = string.Empty;
        
        if (!string.IsNullOrEmpty(userUpdateModel.Password))
        {
            if (!userUpdateModel.Password.IsPasswordRobust())
            {
                errorMessage = "Password must be at least 8 characters, include an uppercase letter, a lowercase letter, a number, and a symbol.";
                return;
            }
            if (userUpdateModel.Password != confirmPassword)
            {
                errorMessage = "Passwords do not match.";
                return;
            }
        }
        
        if (!string.IsNullOrEmpty(userUpdateModel.Name) && !userUpdateModel.Name.IsNameValid())
        {
            errorMessage = "Please enter a valid name (minimum 3 characters).";
            return;
        }
        
        if (!string.IsNullOrEmpty(userUpdateModel.Surname) && !userUpdateModel.Surname.IsSurnameValid())
        {
            errorMessage = "Please enter a valid surname (minimum 3 characters).";
            return;
        }

        try
        {
            var jwtToken = await LocalStorage.GetItemAsStringAsync("jwtToken");
            if (string.IsNullOrEmpty(jwtToken))
            {
                errorMessage = "You must be logged in";
                return;
            }

            var userId = GetUserIdFromJwtToken(jwtToken);
            userUpdateModel.Id = int.Parse(userId);
            
            // Configure the HttpClient with the JWT token
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
            
            var response = await HttpClient.PutAsJsonAsync($"http://localhost:5000/api/User/{userId}", userUpdateModel);

            if (response.IsSuccessStatusCode)
            {
                successMessage = "Profile updated successfully!";
                NavigationManager.NavigateTo("/Profile");
            }
            else
            {
                errorMessage = "Failed to update profile. Please try again.";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"An error occurred: {ex.Message}";
        }
    }
    
    private async Task DeleteAccount()
    {
        showConfirmationDialog = true;
    }

    private async Task ConfirmDelete()
    {
        showConfirmationDialog = false;
        errorMessage = string.Empty;
        successMessage = string.Empty;

        try
        {
            var jwtToken = await LocalStorage.GetItemAsStringAsync("jwtToken");
            if (string.IsNullOrEmpty(jwtToken))
            {
                errorMessage = "You must be logged in to delete your account.";
                return;
            }

            var userId = GetUserIdFromJwtToken(jwtToken);

            // Add the JWT token to the request header
            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await HttpClient.DeleteAsync($"http://localhost:5000/api/User/{userId}");

            if (response.IsSuccessStatusCode)
            {
                await Logout();
            }
            else
            {
                errorMessage = $"Failed to delete the account: {await response.Content.ReadAsStringAsync()}";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"An error occurred while deleting the account: {ex.Message}";
        }
    }
    
    private async Task Logout()
    {
        await LocalStorage.RemoveItemAsync("jwtToken");
        NavigationManager.NavigateTo("/", true);
    }
        
    private string GetDropdownClass()
    {
        return isDropdownOpen ? "block z-10" : "hidden";
    }

    private void ToggleDropdown()
    {
        isDropdownOpen = !isDropdownOpen;
    }
}
