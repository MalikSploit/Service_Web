using System.Text.RegularExpressions;

namespace Entities;

/* 
    * Class : User
    * -----------------------
    * This class is the model for a User object. It contains properties for
    * the ID, name, surname, email and password of a user.
*/
public class User
{
    public int Id { get; init; }
    public string? Name { get; set; } = "";
    public string? Surname { get; set; } = "";
    public string? Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string? Cart { get; set; } = "";

    public override string ToString()
    {
        return $"Id: {Id} Name: {Name} Surname: {Surname} Email : {Email} Pass: {PasswordHash} Cart : {Cart}";
    }
}


/* 
    * Class : UserDTO
    * -----------------------
    * This class is the model for a UserDTO object. It contains properties for
    * the ID, name, surname, email, token and role of a user.
*/
public class UserDTO
{
    public int Id { get; init; }
    public string? Name { get; init; } = "";
    public string? Surname { get; init; } = "";
    public string? Email { get; init; } = "";
    public string? Token { get; set; } = "";
    private static string Role => "";
    public string? Cart { get; set; } = "";

    public override string ToString() => Id + " " + Name + " " + Surname + " " + Email + " " + Token + " " + Role + " " + Cart;
}

/*
    * Class : UserCreateModel
    * -----------------------
    * This class is the model for a UserCreateModel object. It contains properties for
    * the name, surname, email and password of a user.
*/
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

/* 
    * Class : UserUpdateModel
    * -----------------------
    * This class is the model for a UserUpdateModel object. It contains properties for
    * the ID, name, surname, email and password of a user.
*/
public class UserUpdateModel
{
    public int Id { get; set; }
    public string Password { get; set; } = "";
    public string? Name { get; set; }  = "";
    public string? Email { get; set; } = "";
    public string? Surname { get; set; } = "";
    
    public override string ToString() => Id + " " + Name + " " + Surname + " " + Email + " " + Password;
}

/* 
    * Class : UserLogin
    * -----------------------
    * This class is the model for a UserLogin object. It contains properties for
    * the email and password of a user.
*/
public class UserLogin
{
    public string Email { get; set; } = "";
    public string Pass { get; set; } = "";


    public override string ToString() => Email + " " + Pass;

}


/*
    * Class : Extension
    * -----------------------
    * This class is the extension class for the User object. It contains methods for
    * checking if the email, name, surname and password are valid.
*/
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