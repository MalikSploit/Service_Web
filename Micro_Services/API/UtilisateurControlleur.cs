using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using UserLib;

namespace Api;

public class UtilisateurControlleur
{
    private readonly DataContext _context;
    public UtilisateurControlleur(DataContext context)
    {
        _context = context;
    }
}