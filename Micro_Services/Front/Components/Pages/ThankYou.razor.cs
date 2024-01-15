using Front.Services;
using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages;

public partial class ThankYou : ComponentBase
{
    #pragma warning disable CS8618
    [Inject] private CheckoutService CheckoutService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    protected override void OnInitialized()
    {
        if (!CheckoutService.HasCompletedCheckout)
        {
            NavigationManager.NavigateTo("/");
        }
    }
    
    private void NavigateHome()
    {
        NavigationManager.NavigateTo("/");
    }
}