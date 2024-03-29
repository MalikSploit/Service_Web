﻿using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Entities;
using GatewayService.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers;

[Route("api/[controller]")]
[ApiController]

/* 
    * Class : BookController
    * -----------------------
    * This class is the controller for the Book API. It contains methods for
    * GET, POST, PUT, and DELETE requests.
*/
public class BookController(IHttpClientFactory httpClientFactory, JwtTokenValidationService jwtTokenValidationService) : ControllerBase
{
    private readonly HttpClient _client = httpClientFactory.CreateClient("BookService");

    // GET: api/Book
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> Get()
    {
        IEnumerable<Book>? books = null;
        try
        {
            var response = await _client.GetAsync("http://localhost:5002/api/Books");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);

            var json = await response.Content.ReadAsStringAsync();
            books = JsonSerializer.Deserialize<IEnumerable<Book>>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return Ok(books);
    }

    // GET: api/Book/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Book>> Get(int id)
    {
        Book? book = null;

        try
        {
            var response = await _client.GetAsync($"http://localhost:5002/api/Books/{id}");

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);

            var json = await response.Content.ReadAsStringAsync();
            book = JsonSerializer.Deserialize<Book>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return Ok(book);

    }

    // POST: api/Book
    [HttpPost]
    public async Task<IActionResult> CreateBook([FromBody] Book book)
    {
        var errorMessage = string.Empty;
        var tokenWithQuotes = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
        var token = tokenWithQuotes?.Trim('"');

        if (string.IsNullOrEmpty(token) || !jwtTokenValidationService.IsUserAdmin(token, out errorMessage))
        {
            return Unauthorized(errorMessage);
        }
        var response = await _client.PostAsJsonAsync("http://localhost:5002/api/Books", book);

        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        var createdBook = await response.Content.ReadFromJsonAsync<Book>();
        Debug.Assert(createdBook != null, nameof(createdBook) + " != null");
        return CreatedAtAction(nameof(Get), new { id = createdBook.Id }, createdBook);
    }

    // PUT api/Book/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateBook(int id, Book book)
    {
        try
        {
            var errorMessage = string.Empty;
            var tokenWithQuotes = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            var token = tokenWithQuotes?.Trim('"');

            if (string.IsNullOrEmpty(token) || !jwtTokenValidationService.IsUserAdmin(token, out errorMessage))
            {
                return Unauthorized(errorMessage);
            }
            if (id != book.Id)
            {
                return BadRequest();
            }

            var response = await _client.PutAsJsonAsync($"http://localhost:5002/api/Books/{id}", book);

            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }

            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        }
        catch(Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }

    // DELETE api/Book/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try 
        {
            var errorMessage = string.Empty;
            var tokenWithQuotes = HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            var token = tokenWithQuotes?.Trim('"');

            if (string.IsNullOrEmpty(token) || !jwtTokenValidationService.IsUserAdmin(token, out errorMessage))
            {
                return Unauthorized(errorMessage);
            }
            var response = await _client.DeleteAsync($"http://localhost:5002/api/Books/{id}");

            if (response.IsSuccessStatusCode)
            {
                return NoContent();
            }

            return StatusCode((int)response.StatusCode, response.ReasonPhrase);
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ex);
        }
    }
}