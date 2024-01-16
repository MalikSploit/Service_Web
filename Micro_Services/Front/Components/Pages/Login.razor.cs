using Front.Services;
using Microsoft.AspNetCore.Components;
using Blazored.LocalStorage;
using Entities;

namespace Front.Components.Pages;

public partial class Login : ComponentBase
{
    #pragma warning disable CS8618 
    [Inject] private LoginService LoginService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private ILocalStorageService LocalStorage { get; set; }
    [Inject] private CartStateService CartStateService { get; set; }
    private string PasswordInputType { get; set; } = "password";
    private string PasswordButtonIcon { get; set; } = "visibility_off";


    private string _email="";
    private string _pass="";
    private string _errorMessage="";
    private int _attemptCount;
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

    private async Task HandleLoginAsync()
    {
        if (!_email.IsEmailValid() || !_pass.IsPasswordRobust())
        {
            _errorMessage = "Invalid input for email or password";
            return;
        }

        if (_attemptCount >= 5)
        {
            _errorMessage = "You've reached the maximum number of login attempts. Please try again later.";
            return;
        }

        var (isSuccess, userDto, error) = await LoginService.AuthenticateUserAsync(_email, _pass);

        if (isSuccess && userDto?.Token != null)
        {
            await LocalStorage.SetItemAsync("jwtToken", userDto.Token);

            try
            {
                // Fetch the user's cart from the database
                var cart = await CartStateService.FetchCart();
            
                // Update the local storage with the fetched cart
                await LocalStorage.SetItemAsync("cart", cart);
            }
            catch (Exception ex)
            {
                _errorMessage = "Failed to fetch cart: " + ex.Message;
            }

            NavigationManager.NavigateTo("/");
        }
        else
        {
            _attemptCount++;
            _errorMessage = error;
        }
    }
    
    private string GetDropdownClass()
    {
        return isDropdownOpen ? "block z-10" : "hidden";
    }

    private void ToggleDropdown()
    {
        isDropdownOpen = !isDropdownOpen;
    }
    
    private void TogglePasswordView()
    {
        if (PasswordInputType == "password")
        {
            PasswordInputType = "text";
            PasswordButtonIcon = "visibility";
        }
        else
        {
            PasswordInputType = "password";
            PasswordButtonIcon = "visibility_off";
        }
    }
}