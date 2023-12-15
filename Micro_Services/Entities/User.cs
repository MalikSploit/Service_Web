

using System.Text.Json.Serialization;

using System;
using System.Net.Mail;

namespace Entities;


public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }

    public override string ToString()
    {
        return $"Id: ${Id} Name: ${Name} Email : ${Email} Pass: ${PasswordHash}";
    }
}

public class UserDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}

public class UserCreateModel
{
    public required string Password { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}

public class UserUpdateModel
{
    public int Id { get; set; }
    public string? Password { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}

public class UserLogin
{
    public required string Name { get; set; }
    public required string Pass { get; set; }

}

public static class Extension
{
    // public static bool IsPasswordRobust(this string s) => s.Length > 8 && s.ToList().TrueForAll(c => char.IsAsciiLetter(c));

    public static bool IsPasswordRobust(this string s) => s.Length > 8 && s.ToList().TrueForAll(c => char.IsLetter(c));

    public static bool IsEmailValid(this string s)
    {
        try
        {
            // Tentative de création d'une instance de MailAddress
            MailAddress mailAddress = new MailAddress(s);
            return true; // Si la création réussit, l'adresse e-mail est valide
        }
        catch (FormatException)
        {
            return false; // Une exception indique que l'adresse e-mail n'est pas valide
        }
    }

    public static bool IsNameValid(this string s)
    {
        // Vérification de la longueur et des caractères
        bool isLengthValid = s.Length > 3 && s.Length < 20 && s.All(char.IsLetterOrDigit);

        if (!isLengthValid)
        {
            return false; // Le nom ne respecte pas les conditions de longueur ou de caractères
        }

        // Vérification que le nom n'existe pas dans la base de données
        bool isNotInDatabase = !IsUsernameInDatabase(s);

        return isNotInDatabase;
    }

    // Méthode de vérification de l'existence du nom d'utilisateur dans la base de données
    private static bool IsUsernameInDatabase(string username)
    {
        // Code de vérification de l'existence du nom d'utilisateur dans la base de données
        return false;
    }
}