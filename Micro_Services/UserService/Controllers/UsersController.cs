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
public class UsersController(UserServiceContext context, PasswordHasher<User> passwordHasher)
    : ControllerBase
{
    // GET: api/Users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllUsers()
    {
        try
        {
            return await context.User
                .Select(u => UserToDto(u))
                .ToListAsync();
        }
        catch
        {
            return NotFound();
        }

    }

    // GET: api/Users/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDTO>> GetUser(int id)
    {
        try
        {
            var user = await context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return UserToDto(user);
        }
        catch
        {
            return NotFound();
        }
    }

    // PUT: api/Users/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(int id, UserUpdateModel userUpdate)
    {
        if (id != userUpdate.Id)
        {
            return BadRequest("Mismatched user ID");
        }

        var user = await context.User.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        // Update name if provided
        if (!string.IsNullOrWhiteSpace(userUpdate.Name))
        {
            user.Name = userUpdate.Name;
        }
        
        // Update surname if provided
        if (!string.IsNullOrWhiteSpace(userUpdate.Surname))
        {
            user.Surname = userUpdate.Surname;
        }

        // Update password if provided
        if (!string.IsNullOrWhiteSpace(userUpdate.Password))
        {
            if (!userUpdate.Password.IsPasswordRobust())
            {
                return BadRequest("Password is not robust enough");
            }

            user.PasswordHash = passwordHasher.HashPassword(user, userUpdate.Password);
        }

        context.Entry(user).State = EntityState.Modified;

        try
        {
            await context.SaveChangesAsync();
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
    
    // POST: api/Users/login
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(UserLogin userLogin)
    {
        var user = await context.User.FirstOrDefaultAsync(u => u.Email == userLogin.Email);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, userLogin.Pass);

        if (passwordVerificationResult != PasswordVerificationResult.Success)
            return Unauthorized("Invalid credentials.");
        var userDto = UserToDto(user);
        return Ok(userDto);
    }

    // POST: api/Users/register
    [HttpPost("register")]
    public async Task<ActionResult<User>> CreateUser(UserCreateModel userPayload)
    {
        if (await context.User.AnyAsync(u => u.Email == userPayload.Email))
        {
            return BadRequest("Email already in use");
        }
        
        var user = new User
        {
            Email = userPayload.Email,
            Name = userPayload.Name,
            Surname = userPayload.Surname
        };
        user.PasswordHash = passwordHasher.HashPassword(user, userPayload.Password);

        try
        {
            context.User.Add(user);
            await context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, UserToDto(user));
        }
        catch
        {
            return BadRequest("Server Error");
        }

    }

    // DELETE: api/Users/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            context.User.Remove(user);
            await context.SaveChangesAsync();

            return NoContent();
        }
        catch
        {
            return NoContent();
        }
    }

    private bool UserExists(int id)
    {
        return context.User.Any(e => e.Id == id);
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
    
    // GET: api/Users/cart/id
    [HttpGet("cart/{userId:int}")]
    public async Task<ActionResult<string>> GetCart(int userId)
    {
        var user = await context.User.FindAsync(userId);
        if (user == null) 
        {
            return NotFound("User not found.");
        }
        
        return string.IsNullOrEmpty(user.Cart) ? Ok(new Dictionary<int, int>()) : // Return an empty cart if no cart data is present
            Ok(user.Cart);
    }
    
    // POST: api/Users/cart/id
    [HttpPut("cart/{userId:int}")]
    public async Task<IActionResult> UpdateCart(int userId, [FromBody] CartUpdateModel model)
    {
        var user = await context.User.FindAsync(userId);
        if (user == null) return NotFound();

        // Check if the cartJson is not empty or null
        if (string.IsNullOrEmpty(model.CartJson))
        {
            return BadRequest("Cart data is required.");
        }

        try
        {
            user.Cart = model.CartJson;
            await context.SaveChangesAsync();

            return NoContent(); // Or return updated cart data
        }
        catch
        {
            return NoContent();
        }

    }
}

public class CartUpdateModel
{
    public string CartJson { get; set; } = string.Empty;
}