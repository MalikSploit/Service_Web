

using System.Text.Json.Serialization;
using System.Xml.Linq;

using System;
using System.Net.Mail;

namespace Entities;


public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";

    public override string ToString()
    {
        return $"Id: ${Id} Name: ${Name} Email : ${Email} Pass: ${PasswordHash}";
    }
}

public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";

    public override string ToString() => Id + " " + Name + " " + Email;

}

public class UserCreateModel
{
    public string Password { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public UserCreateModel(string password = "", string name = "", string email = "")
    {
        Password = password;
        Name = name;
        Email = email;
    }

    public override string ToString() => Name + " " + Email + " " + Password;
}
public class UserUpdateModel
{
    public int Id { get; set; }
    public string Password { get; set; } = "";
    public string Name { get; set; }  = "";
    public string Email { get; set; } = "";

    public override string ToString() => Id + " " + Name + " " + Email + " " + Password;

}

public class UserLogin
{
    public string Name { get; set; } = "";
    public string Pass { get; set; } = "";


    public override string ToString() => Name + " " + Pass;

}

public static class Extension
{
    public static bool IsPasswordLengthOkay(this string s) => s.Length > 8;

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
        return s.Length > 3 && s.Length < 20 && s.All(char.IsLetterOrDigit);

    public static bool IsNameValid(this string s) => s.Length > 3 && s.Length < 20 && s.ToList().TrueForAll(c => char.IsLetter(c));
}