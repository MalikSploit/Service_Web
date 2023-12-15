

using System.Text.Json.Serialization;
using System.Xml.Linq;

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

    public static bool IsPasswordAsciiLetterOnly(this string s) => s.Length > 8 && s.ToList().TrueForAll(c => char.IsAsciiLetter(c));

    public static bool IsPasswordRobust(this string s) => s.IsPasswordLengthOkay() && s.IsPasswordAsciiLetterOnly();
}