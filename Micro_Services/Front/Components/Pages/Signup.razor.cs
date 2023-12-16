using Entities;
using Microsoft.AspNetCore.Components;
using Front.Services;
using System.Text.RegularExpressions;

namespace Front.Components.Pages;

public partial class Signup : ComponentBase
{
    [Inject] private NavigationManager? NavigationManager { get; set; }
    
    private readonly UserCreateModel _userCreateModel = new ();
    
    private string _errorMessage="";
    
    private bool ValidateInput()
    {
        if (!Regex.IsMatch(_userCreateModel.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            _errorMessage = "Please enter a valid email address.";
            return false;
        }
        if (!Regex.IsMatch(_userCreateModel.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$"))
        {
            _errorMessage = "Password must be at least 8 characters, include an uppercase letter, a lowercase letter, a number, and a symbol.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(_userCreateModel.Name) || _userCreateModel.Name.Length < 3)
        {
            _errorMessage = "Please enter a valid name (minimum 3 characters).";
            return false;
        }
        if (string.IsNullOrWhiteSpace(_userCreateModel.Surname) || _userCreateModel.Surname.Length < 3)
        {
            _errorMessage = "Please enter a valid surname (minimum 3 characters).";
            return false;
        }

        return true;
    }

    private async Task HandleSignup()
    {
        if (!ValidateInput())
        {
            return;
        }
        var apiUrl = LoginService.Urlprefix +  "api/User/register";

        HttpResponseMessage response = await Client.PostAsJsonAsync(apiUrl, _userCreateModel);
        
        NavigationManager.NavigateTo("/Login");
    }
}