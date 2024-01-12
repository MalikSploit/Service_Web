using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Blazored.LocalStorage;
using Entities;
using Front.Services;

namespace Front.Components.Pages;

public partial class Checkout : ComponentBase
{
    #pragma warning disable CS8618
    [Inject] private IJSRuntime JSRuntime { get; set; }
    [Inject] private ILocalStorageService LocalStorage { get; set; }
    [Inject] private BookService BookService { get; set; }
    
    [Required, MaxLength(100)]
    private string CustomerName { get; set; }

    [Required, EmailAddress]
    private string Email { get; set; }

    [Required, MaxLength(200)]
    private string BillingAddress { get; set; }

    private EditContext _editContext;
    private readonly List<Book> cartItems = new List<Book>();
    private Dictionary<int, int> cartItemQuantities = new Dictionary<int, int>();
    private decimal totalPrice;

    protected override void OnInitialized()
    {
        _editContext = new EditContext(this);
    }
    
    protected override async Task OnInitializedAsync()
    {
        await LoadCartItems();
        CalculateTotalPrice();
    }
    
    private async Task LoadCartItems()
    {
        var storedCart = await LocalStorage.GetItemAsync<Dictionary<int, int>>("cart");
        if (storedCart != null)
        {
            cartItemQuantities = storedCart;
            foreach (var itemId in cartItemQuantities.Keys)
            {
                var book = await BookService.GetBookByIdAsync(itemId);
                if (book != null)
                {
                    cartItems.Add(book);
                }
            }
        }
    }
    
    private void CalculateTotalPrice()
    {
        totalPrice = cartItems.Sum(item => item.Price * cartItemQuantities[item.Id]);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync("stripeIntegration.initializeStripe", "pk_test_51OXTe7JQfiCJfWzJoRvyQiEJZxI29bd2WSugPd23b38Rk4xmDoBjMfcXFxf1hTmQ9VbHjkT9qrWQAeRb14pldW8e00mNUCvSVq");
        }
    }

    private async Task ValidateCard()
    {
        if (!_editContext.Validate())
        {
            Console.WriteLine("Form is not valid.");
            return;
        }

        try
        {
            var cardValidationResponse = await JSRuntime.InvokeAsync<bool>("stripeIntegration.validateCard");
            Console.WriteLine(cardValidationResponse ? "The card is valid" : "The card is invalid");
        }
        catch (JSException)
        {
            Console.WriteLine("Failed to validate the card.");
        }
    }
}