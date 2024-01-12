using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Entities;
using Blazored.LocalStorage;
using Front.Services;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Front.Components.Pages;

public partial class Profile
{
    #pragma warning disable CS8618
    private string confirmPassword;
    private readonly UserUpdateModel userUpdateModel = new ();
    private string errorMessage = string.Empty;
    private string successMessage = string.Empty;
    private bool isDropdownOpen;
    private bool showConfirmationDialog;
    private bool _isUserAdmin;
    private int CartItemCount { get; set; }

    [Inject] private ILocalStorageService LocalStorage { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private HttpClient HttpClient { get; set; }
    [Inject] private CartStateService CartStateService { get; set; }
    [Inject] private LoginService LoginService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var jwtTokenWithQuotes = await LocalStorage.GetItemAsStringAsync("jwtToken");
        var isLogged = false; // Create the isLogged variable before the if statement
        if (!string.IsNullOrEmpty(jwtTokenWithQuotes))
        {
            var jwtToken = jwtTokenWithQuotes.Trim('"');
            var jwtPayload = ParseJwtPayload(jwtToken);
            if (jwtPayload != null)
            {
                isLogged = jwtPayload.Claims.Any(c => c.Type == ClaimTypes.Role && (c.Value == "User" || c.Value == "Admin"));
                if (isLogged)
                {
                    userUpdateModel.Email = jwtPayload.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
                    userUpdateModel.Name = jwtPayload.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                    userUpdateModel.Surname = jwtPayload.Claims.FirstOrDefault(c => c.Type == "surname")?.Value;
                }
            }
        }
    
        if (!isLogged)
        {
            NavigationManager.NavigateTo("/Login");
            return;
        }
    
        _isUserAdmin = await LoginService.IsUserAdmin();
        CartStateService.OnChange += UpdateCartCount;
        UpdateCartCount();
    }
    
    private async void UpdateCartCount()
    {
        CartItemCount = await CartStateService.GetCartItemCountAsync();
        StateHasChanged();
    }
    
    private static JwtSecurityToken? ParseJwtPayload(string jwtToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.CanReadToken(jwtToken) ? tokenHandler.ReadJwtToken(jwtToken) : null;
    }
    
    private static string? GetUserIdFromJwtToken(string jwtTokenWithQuotes)
    {
        var jwtToken = jwtTokenWithQuotes.Trim('"');
        var tokenHandler = new JwtSecurityTokenHandler();

        if (!tokenHandler.CanReadToken(jwtToken)) return null;
        var jwtTokenObject = tokenHandler.ReadJwtToken(jwtToken);
        return jwtTokenObject.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;
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
            if (userId != null)
            {
                userUpdateModel.Id = int.Parse(userId);

                // Configure the HttpClient with the JWT token
                HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

                var response = await HttpClient.PutAsJsonAsync($"http://localhost:5000/api/User/{userId}", userUpdateModel);

                if (response.IsSuccessStatusCode)
                {
                    var updatedUser = await response.Content.ReadFromJsonAsync<UserDTO>();
                    if (updatedUser != null && !string.IsNullOrWhiteSpace(updatedUser.Token))
                    {
                        // Replace the old token with the new one
                        await LocalStorage.SetItemAsStringAsync("jwtToken", updatedUser.Token);
                    }
                    
                    successMessage = "Profile updated successfully!";
                    NavigationManager.NavigateTo("/Profile");
                }
                else
                {
                    errorMessage = "Failed to update profile. Please try again.";
                }
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"An error occurred: {ex.Message}";
        }
    }
    
    private Task DeleteAccount()
    {
        showConfirmationDialog = true;
        return Task.CompletedTask;
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
