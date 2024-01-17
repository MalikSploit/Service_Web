using Microsoft.AspNetCore.Mvc;
using Entities;
using BookService.Data;
using Microsoft.EntityFrameworkCore;

namespace BookService.Controllers;

[Route("api/[controller]")]
[ApiController]


/*
    * Class: BooksController
    * -----------------------
    * This class is the controller for the Books API. It contains methods for
    * GET, POST, PUT, and DELETE requests.
*/
public class BooksController(BookServiceContext context) : ControllerBase
{
    private DbSet<Book> Books => context.Books;

    // GET: api/Books
    [HttpGet]
    public IEnumerable<Book> Get() => Books.ToList();

    // GET api/Books/id
    [HttpGet("{id:int}")]
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
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = book.Id }, book);
    }
    
    // PUT api/Books/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBook(int id, Book bookToUpdate)
    {
        if (id != bookToUpdate.Id)
        {
            return BadRequest("Mismatched book ID");
        }

        var book = await context.Books.FindAsync(id);

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
            await context.SaveChangesAsync();
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
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await Books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        Books.Remove(book);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private bool BookExists(int id) => Books.Any(t => t.Id == id);
}