using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages;

public partial class TermsOfServiceModal : ComponentBase
{
    private bool IsVisible { get; set; }
    private bool IsAgreed { get; set; }
    private string ErrorMessage { get; set; } = "";
    public bool TermsAgreed { get; private set; } = false;

    public void Show()
    {
        IsVisible = true;
        IsAgreed = false;
        ErrorMessage = "";
        StateHasChanged();
    }

    private void OnClose() 
    {
        IsVisible = false;
        StateHasChanged();
    }

    private async Task OnConfirm()
    {
        if (!IsAgreed)
        {
            ErrorMessage = "Please agree to the Terms of Service to continue.";
            StateHasChanged();
        }
        else
        {
            ErrorMessage = "";
            OnClose();
        }

        await Task.CompletedTask;
    }

    private void ConfirmAgreement()
    {
        if (IsAgreed)
        {
            TermsAgreed = true;
            OnClose();
        }
        else
        {
            ErrorMessage = "Please agree to the Terms of Service to continue.";
        }
    }
}