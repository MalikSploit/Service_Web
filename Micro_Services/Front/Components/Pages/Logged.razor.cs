﻿using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace Front.Components.Pages;

public partial class Logged
{
    private string _userName = "";

    [Inject]
    NavigationManager? NavigationManager { set; get; }
    [Inject]
    private ILocalStorageService? LocalStorage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var jwtToken = await LocalStorage.GetItemAsStringAsync("jwtToken");

        if (!string.IsNullOrEmpty(jwtToken) && IsTokenValid(jwtToken))
        {
            try
            {
                var parts = jwtToken.Split('.');
                if (parts.Length == 3)
                {
                    var payload = parts[1];
                    var jsonBytes = ParseBase64WithoutPadding(payload);
                    var jsonPayload = Encoding.UTF8.GetString(jsonBytes);
                    var jwtPayload = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonPayload);

                    if (jwtPayload != null && jwtPayload.ContainsKey("name") && jwtPayload.ContainsKey("surname"))
                    {
                        var name = jwtPayload["surname"].ToString();
                        var surname = jwtPayload["name"].ToString();
                        _userName = $"{name} {surname}";
                    }
                    else
                    {
                        Console.WriteLine("Name or surname claim not found in JWT token");
                    }
                }
                else
                {
                    Console.WriteLine("JWT token does not have three parts");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error processing JWT token: " + ex.Message);
            }
        }
        else
        {
            NavigationManager.NavigateTo("/Login");
        }
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

    private bool IsTokenValid(string token)
    {
        // Implement token validation logic here
        return !string.IsNullOrEmpty(token);
    }
}