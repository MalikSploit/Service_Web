using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using Entities;


namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]

/* 
    * Class : UsersController
    * -----------------------
    * This class is the controller for the User API. It contains methods for
    * GET, POST, PUT, and DELETE requests.
    * This class contains attributes for the UserServiceContext and the
    * PasswordHasher.
*/
public class UsersController : ControllerBase
{
    private readonly UserServiceContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public UsersController(UserServiceContext context, PasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
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
    [HttpGet("{id:int}")]
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
    [HttpPut("{id:int}")]
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
            throw;
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

        if (passwordVerificationResult != PasswordVerificationResult.Success)
            return Unauthorized("Invalid credentials.");
        var userDto = UserToDto(user);
        return Ok(userDto);

    }

    // DELETE: api/Users/5
    [HttpDelete("{id:int}")]
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
            Cart = user.Cart ?? ""
        };
    }
    
    [HttpGet("cart/{userId:int}")]
    public async Task<ActionResult<string>> GetCart(int userId)
    {
        var user = await _context.User.FindAsync(userId);
        if (user == null) 
        {
            return NotFound("User not found.");
        }
        
        return string.IsNullOrEmpty(user.Cart) ? Ok(new Dictionary<int, int>()) : // Return an empty cart if no cart data is present
            Ok(user.Cart);
    }
    
    [HttpPut("cart/{userId:int}")]
    public async Task<IActionResult> UpdateCart(int userId, [FromBody] CartUpdateModel model)
    {
        var user = await _context.User.FindAsync(userId);
        if (user == null) return NotFound();

        // Check if the cartJson is not empty or null
        if (string.IsNullOrEmpty(model.CartJson))
        {
            return BadRequest("Cart data is required.");
        }
        
        user.Cart = model.CartJson;
        await _context.SaveChangesAsync();

        return NoContent(); // Or return updated cart data
    }
}

public class CartUpdateModel
{
    public string CartJson { get; set; } = string.Empty;
}