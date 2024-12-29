﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactLibrary.Server.Data;
using ReactLibrary.Server.Models;

namespace ReactLibrary.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("genres")]
        public async Task<ActionResult<IEnumerable<string>>> GetGenres()
        {
            var genres = from b in _context.Book orderby b.Genre select b.Genre;
            return await genres.Distinct().ToListAsync();
        }


        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> GetBook(string? bookGenre, string? title, string? author)
        {
            var reservations = from r in _context.Reservation select r;

            foreach (var reservation in reservations)
            {
                if (DateTime.Now > reservation.ValidDate)
                {
                    var book = _context.Book.FirstOrDefault(b => b.Id == reservation.BookId);
                    book!.Status = "Available";
                    _context.Book.Update(book);
                    _context.Reservation.Remove(reservation);
                }
            }
            await _context.SaveChangesAsync();

            var books = from b in _context.Book select b;

            if (!string.IsNullOrEmpty(title))
            {
                books = books.Where(b => b.Title!.ToUpper().Contains(title.ToUpper()));
            }
            if (!string.IsNullOrEmpty(author))
            {
                books = books.Where(b => b.Author!.ToUpper().Contains(author.ToUpper()));
            }
            if (!string.IsNullOrEmpty(bookGenre))
            {
                books = books.Where(b => b.Genre == bookGenre);
            }

            if (!User.IsInRole("Librarian"))
            {
                books = books.Where(b => b.Status != "Permanently unavailable");
            }

            return await books.ToListAsync();
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Book.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("Librarian") && book.Status == "Permanently unavailable")
            {
                return NotFound();
            }

            return book;
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> PutBook(int id, PutBookDTO bookDTO)
        {
            if (id != bookDTO.Id)
            {
                return BadRequest();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            book.Title = bookDTO.Title;
            book.Author = bookDTO.Author;
            book.Genre = bookDTO.Genre;
            book.Publisher = bookDTO.Publisher;
            book.PublicationDate = bookDTO.PublicationDate;
            book.RowVersion = bookDTO.RowVersion;

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
                    //throw;
                    return Conflict();
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize (Roles = "Librarian")]
        public async Task<ActionResult<Book>> PostBook(PostBookDTO bookDTO)
        {
            var book = new Book
            {
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                Genre = bookDTO.Genre,
                Publisher = bookDTO.Publisher,
                PublicationDate = bookDTO.PublicationDate,
                Status = "Available"
            };

            _context.Book.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Book.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
