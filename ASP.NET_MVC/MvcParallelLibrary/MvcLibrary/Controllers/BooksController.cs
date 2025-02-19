using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using MvcLibrary.Data;
using MvcLibrary.Models;
using Microsoft.AspNetCore.Identity;
using MvcLibrary.Data.Migrations;
using static System.Reflection.Metadata.BlobBuilder;
using X.PagedList.Extensions;

namespace MvcLibrary.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string bookGenre, string title, string author, int? page)
        {
            if (_context.Book == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Book' is null.");
            }

            IQueryable<string> genreQuery = from b in _context.Book
                                            orderby b.Genre
                                            select b.Genre;

            var books = from b in _context.Book
                        select b;

            var reservations = from r in _context.Reservation
                               select r;

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
                books = books.Where(s => s.Status != "Permanently unavailable");
            }

            var pageNumber = page ?? 1;
            var booksList = await books.ToListAsync();
            var onePageOffBooks = booksList.ToPagedList(pageNumber, 15);

            var bookGenreVM = new BookGenreViewModel
            {
                Genres = new SelectList(await genreQuery.Distinct().ToListAsync()),
                Books = onePageOffBooks
            };

            if (User.IsInRole("Librarian"))
            {
                return View("IndexLibrarian", bookGenreVM);
            }
            if (User.IsInRole("Reader"))
            {
                return View("IndexReader", bookGenreVM);
            }

            return View(bookGenreVM);
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            if (User.IsInRole("Librarian"))
            {
                return View("DetailsLibrarian", book);
            }

            return View(book);
        }

        // GET: Books/Create
        [Authorize(Roles = "Librarian")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Genre,Publisher,PublicationDate,Status")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Genre,Publisher,PublicationDate,Status,RowVersion")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //_context.Entry(book).Property("RowVersion").OriginalValue = book.RowVersion;
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Book)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();

                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The book was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Book)databaseEntry.ToObject();

                        if (databaseValues.Title != clientValues.Title)
                        {
                            ModelState.AddModelError("Title", $"Current value: {databaseValues.Title}");
                        }
                        if (databaseValues.Author != clientValues.Author)
                        {
                            ModelState.AddModelError("Author", $"Current value: {databaseValues.Author}");
                        }
                        if (databaseValues.Genre != clientValues.Genre)
                        {
                            ModelState.AddModelError("Genre", $"Current value: {databaseValues.Genre}");
                        }
                        if (databaseValues.Publisher != clientValues.Publisher)
                        {
                            ModelState.AddModelError("Publisher", $"Current value: {databaseValues.Publisher}");
                        }
                        if (databaseValues.PublicationDate != clientValues.PublicationDate)
                        {
                            ModelState.AddModelError("PublicationDate", $"Current value: {databaseValues.PublicationDate}");
                        }
                        if (databaseValues.Status != clientValues.Status)
                        {
                            ModelState.AddModelError("Status", $"Previous value: {clientValues.Status}");
                            book.Status = databaseValues.Status;
                            ModelState.Remove("Status");
                        }

                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                                + "was modified by another user after you got the original value. The "
                                + "edit operation was canceled and the current values in the database "
                                + "have been displayed. If you still want to edit this record, click "
                                + "the Save button again. Otherwise click the Back to List hyperlink.");
                        book.RowVersion = databaseValues.RowVersion;
                        ModelState.Remove("RowVersion");
                    }

                    return View(book);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction(nameof(Index));
                }
                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ConcurrencyErrorMessage"] = "The book you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "book, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Delete(int? id, byte[] rowVersion)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return RedirectToAction(nameof(Index));
            }

            _context.Entry(book).Property("RowVersion").OriginalValue = rowVersion;

            try
            {
                if (_context.Checkout.Any(c => c.BookId == book!.Id))
                {
                    book.Status = "Permanently unavailable";
                    _context.Book.Update(book);
                }
                else
                {
                    _context.Book.Remove(book);
                }

                var reservation = await _context.Reservation.FirstOrDefaultAsync(b => b.BookId == book.Id);
                if (reservation != null)
                {
                    _context.Reservation.Remove(reservation);
                }
                var checkout = await _context.Checkout.Where(b => b.EndTime == null).FirstOrDefaultAsync(b => b.BookId == book.Id);
                if (checkout  != null)
                {
                    checkout.EndTime = DateTime.Now;
                    _context.Checkout.Update(checkout);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction(nameof(Delete), new { concurrencyError = true, id = book.Id });
            }
        }

        private bool BookExists(int id)
        {
            return _context.Book.Any(e => e.Id == id);
        }
    }
}
