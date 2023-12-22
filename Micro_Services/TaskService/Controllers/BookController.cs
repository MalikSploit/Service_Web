using Microsoft.AspNetCore.Mvc;
using Entities;
using BookService.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BookService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class  BookController : ControllerBase
{
    private readonly BookServiceContext _context;
    private DbSet<Book> Books => _context.Books;

    public BookController(BookServiceContext context)
    {
        _context = context;
    }

    // GET: api/Books
    [HttpGet]
    public IEnumerable<Book> Get() => Books.ToList();

    // GET api/Books/5
    [HttpGet("{id}")]
    public Book? Get(int id) => Books.FirstOrDefault(t => t.Id == id);


    // POST api/Books
    [HttpPost]
    public async Task<ActionResult<Book>> CreateTask(BookCreate bc)
    {
        var b = new Book(0, bc.Title, bc.Author, bc.Description, bc.ImageUrl, bc.Price);
        Books.Add(b);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetBook", new { id = b.Id }, b);
    }


    private bool BookExists(int id) => Books.Any(t => t.Id == id);

    // PUT api/Books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, Book bookToUpdate)
    {
        if (id != bookToUpdate.Id)
        {
            return BadRequest("Mismatched book ID");
        }

        var b = await Books.FindAsync(id);
        if(b == null) { return BadRequest("Unknow book ID"); }
        _context.Entry(b!).State = EntityState.Modified;
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookExists(id))
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

    // DELETE api/Books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var user = await Books.FindAsync(id);
        if (user == null) { return NotFound(); }
        Books.Remove(user);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
