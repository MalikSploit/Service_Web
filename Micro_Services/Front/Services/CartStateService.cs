using Blazored.LocalStorage;

namespace Front.Services;

public class CartStateService(ILocalStorageService localStorage)
{
    public event Action? OnChange;

    public async Task<int> GetCartItemCountAsync()
    {
        var cart = await localStorage.GetItemAsync<Dictionary<int, int>>("cart");
        try
        {
            return cart?.Sum(c => c.Value) ?? 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("trop de livre : " + ex.Message);
        }
        return int.MaxValue;
    }

    public async Task UpdateCartItemCountAsync()
    {
        var itemCount = await GetCartItemCountAsync();
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}