using Microsoft.AspNetCore.Components;

namespace Front.Components.Pages;

public partial class Index : ComponentBase
{
    private bool _isDropdownVisible;

    private void ToggleDropdown()
    {
        Console.WriteLine("Dropdown visibility toggled: " + _isDropdownVisible);
        _isDropdownVisible = !_isDropdownVisible;
    }
    
    private string GetDropdownClass()
    {
        return _isDropdownVisible ? "block" : "hidden";
    }
}