using Microsoft.AspNetCore.Mvc;
using Entities;
using BookService.Data;
using Microsoft.EntityFrameworkCore;

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

    // GET api/Books/id
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var book = Books.FirstOrDefault(t => t.Id == id);

        if (book == null)
        {
            return NotFound();
        }

        return Ok(book);
    }

    // POST api/Books
    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook(Book book)
    {
        Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
    }
    
    // PUT api/Books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, Book bookToUpdate)
    {
        if (id != bookToUpdate.Id)
        {
            return BadRequest("Mismatched book ID");
        }

        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        book.Title = bookToUpdate.Title;
        book.Author = bookToUpdate.Author;
        book.Description = bookToUpdate.Description;
        book.ImageUrl = bookToUpdate.ImageUrl;
        book.Price = bookToUpdate.Price;

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
            throw;
        }

        return NoContent();
    }

    // DELETE api/Books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool BookExists(int id) => Books.Any(t => t.Id == id);
}