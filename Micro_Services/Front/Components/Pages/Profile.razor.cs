using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Entities;

namespace Front.Components.Pages;

public partial class Profile : ComponentBase
{
    [Inject] private UserService UserService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }

    private UserUpdateModel _userUpdateModel = new UserUpdateModel();
    private User user;

    // Variables pour contrôler l'affichage de la fenêtre d'erreur
    private bool showNameError = false;
    private bool showEmailError = false;
    private bool showPasswordError = false;

    protected override async Task OnInitializedAsync()
    {
        user = await UserService.GetCurrentUser();
        _userUpdateModel = new UserUpdateModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }

    private async Task UpdateProfile()
    {
        // Validation des champs ici avec la classe Extension
        if (!_userUpdateModel.Name.IsNameValid())
        {
            showNameError = true;
            return;
        }

        if (!_userUpdateModel.Email.IsEmailValid())
        {
            showEmailError = true;
            return;
        }

        if (!_userUpdateModel.Password.IsPasswordRobust())
        {
            showPasswordError = true;
            return;
        }

        // Mise à jour du profil
        await UserService.UpdateUserAsync(_userUpdateModel);

        // Redirection vers la page de profil ou une autre page
        NavigationManager.NavigateTo("/profile");
    }
}
