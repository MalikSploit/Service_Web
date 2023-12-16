using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserServiceContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public UsersController(UserServiceContext context, PasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }
    
    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourVeryLongSecureKeyHere123456789."));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("surname", user.Surname),
            new Claim("name", user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "Malik",
            audience: "ISIMA",
            claims: claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
    {
        return await _context.User
            .Select(u => UserToDto(u))
            .ToListAsync();
    }

    // GET: api/Users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUser(int id)
    {
        var user = await _context.User.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        return UserToDto(user);
    }

    // PUT: api/Users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateModel userUpdate)
    {
        if (id != userUpdate.Id)
        {
            return BadRequest("Mismatched user ID");
        }

        var user = await _context.User.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        // Update name if provided
        if (!string.IsNullOrWhiteSpace(userUpdate.Name))
        {
            user.Name = userUpdate.Name;
        }
        
        // Update name if provided
        if (!string.IsNullOrWhiteSpace(userUpdate.Surname))
        {
            user.Surname = userUpdate.Surname;
        }

        // Update email if provided and valid
        if (!string.IsNullOrWhiteSpace(userUpdate.Email))
        {
            if (!userUpdate.Email.IsEmailValid())
            {
                return BadRequest("Invalid email format");
            }

            // Check for email uniqueness
            if (await _context.User.AnyAsync(u => u.Email == userUpdate.Email && u.Id != id))
            {
                return BadRequest("Email already in use by another user");
            }

            user.Email = userUpdate.Email;
        }

        // Update password if provided
        if (!string.IsNullOrWhiteSpace(userUpdate.Password))
        {
            if (!userUpdate.Password.IsPasswordRobust())
            {
                return BadRequest("Password is not robust enough");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, userUpdate.Password);
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Users/register
    [HttpPost("register")]
    public async Task<ActionResult<User>> CreateUser(UserCreateModel userPayload)
    {
        if (!userPayload.Password.IsPasswordRobust())
        {
            return BadRequest("Password is not robust enough, Please enter 8 characters with 1 Uppercase, 1 Lowercase, 1 nNumber and 1 Symbol");
        }
        if (!userPayload.Email.IsEmailValid())
        {
            return BadRequest("Email is not valid");
        }
        if (await _context.User.AnyAsync(u => u.Email == userPayload.Email))
        {
            return BadRequest("Email already in use");
        }
        
        var user = new User
        {
            Email = userPayload.Email,
            Name = userPayload.Name,
            Surname = userPayload.Surname
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, userPayload.Password);

        _context.User.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUser", new { id = user.Id }, UserToDto(user));
    }

    // POST: api/Users/login
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(UserLogin userLogin)
    {
        var user = await _context.User.FirstOrDefaultAsync(u => u.Email == userLogin.Email);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userLogin.Pass);

        if (passwordVerificationResult == PasswordVerificationResult.Success)
        {
            var token = GenerateJwtToken(user);
            var userDto = UserToDto(user);
            userDto.Token = token; 
            return Ok(userDto);
        }

        return Unauthorized("Invalid credentials.");
    }

    // DELETE: api/Users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.User.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.User.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id)
    {
        return _context.User.Any(e => e.Id == id);
    }

    private static UserDTO UserToDto(User user)
    {
        return new UserDTO
        {
            Id = user.Id,
            Name = user.Name,
            Surname = user.Surname,
            Email = user.Email,
        };
    }
}
