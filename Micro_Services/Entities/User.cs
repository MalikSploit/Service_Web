using System.Text.RegularExpressions;

namespace Entities;


public class User
{
    public int Id { get; set; }
    public string? Name { get; set; } = "";
    public string? Surname { get; set; } = "";
    public string? Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";

    public override string ToString()
    {
        return $"Id: ${Id} Name: ${Name} Surname: ${Surname} Email : ${Email} Pass: ${PasswordHash}";
    }
}

public class UserDTO
{
    public int Id { get; set; }
    public string? Name { get; set; } = "";
    public string? Surname { get; set; } = "";
    public string? Email { get; set; } = "";
    public string? Token { get; set; } = "";
    public string Role { get; set; } = "";

    public override string ToString() => Id + " " + Name + " " + Surname + " " + Email + " " + Token + " " + Role;
}

public class UserCreateModel
{
    public string Password { get; set; }
    public string Surname { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }

    public UserCreateModel(string password = "", string name = "", string email = "", string surname = "")
    {
        Password = password;
        Name = name;
        Surname = surname;
        Email = email;
    }

    public override string ToString() => Name + " " + Surname + " " + Email + " " + Password;
}
public class UserUpdateModel
{
    public int Id { get; set; }
    public string Password { get; set; } = "";
    public string? Name { get; set; }  = "";
    public string? Email { get; set; } = "";
    public string? Surname { get; set; } = "";
    
    public override string ToString() => Id + " " + Name + " " + Surname + " " + Email + " " + Password;

}

public class UserLogin
{
    public string Email { get; set; } = "";
    public string Pass { get; set; } = "";


    public override string ToString() => Email + " " + Pass;

}

public static class Extension
{
    private static readonly Regex EmailRegex = new Regex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
    private static readonly Regex NameRegex = new Regex(@"^[a-zA-Z]+([ '-][a-zA-Z]+)*$", RegexOptions.Compiled);
    private static readonly Regex PasswordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$", RegexOptions.Compiled);

    public static bool IsPasswordRobust(this string password)
    {
        return PasswordRegex.IsMatch(password);
    }
    
    public static bool IsEmailValid(this string? email)
    {
        return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
    }

    public static bool IsNameValid(this string? name)
    {
        return !string.IsNullOrWhiteSpace(name) && NameRegex.IsMatch(name) && name.Length is >= 3 and <= 20;
    }

    public static bool IsSurnameValid(this string? surname)
    {
        return IsNameValid(surname);
    }
}