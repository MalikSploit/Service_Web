using Entities;
using Microsoft.AspNetCore.Components;
using Front.Services;

namespace Front.Components.Pages;

public partial class Signup : ComponentBase
{
    #pragma warning disable CS8618 
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private CartStateService CartStateService { get; set; }

    private readonly UserCreateModel _userCreateModel = new ();
    private string _errorMessage="";
    private bool isDropdownOpen;
    private int CartItemCount { get; set; }
    
    protected override Task OnInitializedAsync()
    {
        CartStateService.OnChange += UpdateCartCount;
        UpdateCartCount();
        return Task.CompletedTask;
    }
    
    private async void UpdateCartCount()
    {
        CartItemCount = await CartStateService.GetCartItemCountAsync();
        StateHasChanged();
    }
    
    private bool ValidateInput()
    {
        if (!_userCreateModel.Email.IsEmailValid())
        {
            _errorMessage = "Please enter a valid email address.";
            return false;
        }
        if (!_userCreateModel.Password.IsPasswordRobust())
        {
            _errorMessage = "Password must be at least 8 characters, include an uppercase letter, a lowercase letter, a number, and a symbol.";
            return false;
        }
        if (!_userCreateModel.Surname.IsNameValid())
        {
            _errorMessage = "Please enter a valid name (minimum 3 characters).";
            return false;
        }
        if (!_userCreateModel.Surname.IsSurnameValid())
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

        await Client.PostAsJsonAsync(apiUrl, _userCreateModel);
        
        NavigationManager.NavigateTo("/Login");
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