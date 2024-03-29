﻿using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Blazored.LocalStorage;
using Newtonsoft.Json;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Front.Services;

public class CartStateService(ILocalStorageService localStorage, HttpClient httpClient)
{
    public event Action? OnChange;

    public async Task<int> GetCartItemCountAsync()
    {
        try
        {
            var cart = await localStorage.GetItemAsync<Dictionary<int, int>>("cart");
            return cart?.Sum(c => c.Value) ?? 0;
        }
        catch (System.Text.Json.JsonException ex)
        {
            Console.WriteLine("Error calculating cart items: " + ex.Message);
            await ClearCartAsync(); // Reset the cart if malformed JSON is found
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error in GetCartItemCountAsync: " + ex.Message);
            return 0;
        }
    }

    private async Task<int> GetUserIdFromJwtToken()
    {
        try
        {
            var jwtTokenWithQuotes = await localStorage.GetItemAsStringAsync("jwtToken");
            if (string.IsNullOrEmpty(jwtTokenWithQuotes))
            {
                return 0; // No user ID found
            }

            var jwtToken = jwtTokenWithQuotes.Trim('"');
            var tokenHandler = new JwtSecurityTokenHandler();

            if (!tokenHandler.CanReadToken(jwtToken))
            {
                return 0;
            }

            var jwtSecurityToken = tokenHandler.ReadJwtToken(jwtToken);

            // Check for expiration
            var expClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
            if (expClaim is null
                || !long.TryParse(expClaim.Value, out var exp)
                || DateTimeOffset.FromUnixTimeSeconds(exp) <= DateTimeOffset.UtcNow)
            {
                return 0;
            }

            // Extract User ID from the 'sub' claim
            var userIdClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == "sub");
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error processing JWT token: " + ex.Message);
            return 0;
        }
    }
    
    public async Task<Dictionary<int, int>> FetchCart()
    {
        try
        {
            var userId = await GetUserIdFromJwtToken();
            if (userId == 0)
            {
                throw new InvalidOperationException("User is not authenticated.");
            }

            var jwtToken = await localStorage.GetItemAsStringAsync("jwtToken");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken?.Trim('"'));

            var response = await httpClient.GetAsync($"http://localhost:5000/api/User/cart/{userId}");
            if (!response.IsSuccessStatusCode) 
            {
                throw new HttpRequestException("Failed to fetch cart from the server.");
            }
            var cartJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<int, int>>(cartJson) ?? new Dictionary<int, int>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching cart: " + ex.Message);
            return new Dictionary<int, int>();
        }
    }

    private async Task SynchronizeCartAsync(Dictionary<int, int> cart)
    {
        try
        {
            var userId = await GetUserIdFromJwtToken();
            if (userId == 0)
            {
                Console.WriteLine("User ID not found in JWT token.");
                return;
            }

            var jwtToken = await localStorage.GetItemAsStringAsync("jwtToken");
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken?.Trim('"'));

            var cartJson = JsonConvert.SerializeObject(cart);
            var payload = new { cartJson };
            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

            var response = await httpClient.PutAsync($"http://localhost:5000/api/User/cart/{userId}", content);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Error synchronizing cart with server: " + response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error synchronizing cart: " + ex.Message);
        }
    }

    public async Task UpdateCartAsync(Dictionary<int, int> newCart, bool updateServer = false)
    {
        try
        {
            await localStorage.SetItemAsync("cart", newCart);
            if (updateServer)
            {
                await SynchronizeCartAsync(newCart);
            }
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error updating cart: " + ex.Message);
        }
    }
    private void NotifyStateChanged() => OnChange?.Invoke();
    
    public async Task ClearCartAsync()
    {
        try
        {
            await localStorage.RemoveItemAsync("cart");
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error clearing cart: " + ex.Message);
        }
    }
}