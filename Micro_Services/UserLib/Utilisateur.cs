using System.ComponentModel.DataAnnotations;

namespace API;

public class Utilisateur
{
    public int Id { get; set; }
    public string Prenom { get; set; }
    public string Nom { get; set; }
    public string Email { get; set; }
    public string Pass { get; private set; }
    public string NomComplet => Nom + " " + Prenom;
    // Constructeur
    public Utilisateur(int id, string nom, string prenom, string email, string pass)
    {
        Id = id;
        Prenom = prenom;
        Nom = nom;
        Email = email;
        Pass = pass;
    }
    public void changePass()
    {
        this.Pass = "muchSecure";
    }
    public bool isPassSecure()
    {
        if (Pass.Length > 6)
        {
            return true;
        }
        return false;
    }
}
