using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcLibrary.Data;
using MvcLibrary.Data.Migrations;
using MvcLibrary.Models;

namespace MvcLibrary.Controllers
{
    public class CheckoutsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Checkouts
        [Authorize]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Librarian"))
            {
                return RedirectToAction(nameof(IndexLibrarian));
            }
            if (User.IsInRole("Reader"))
            {
                return RedirectToAction(nameof(IndexReader));
            }
            return View(await _context.Reservation.ToListAsync());
        }

        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> IndexLibrarian(string title, string author, string userName)
        {
            if (_context.Reservation == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservation' is null.");
            }

            IEnumerable<CheckoutBookViewModel> checkouts = new List<CheckoutBookViewModel>();

            foreach (var checkout in _context.Checkout)
            {
                var book = await _context.Book.FindAsync(checkout.BookId);
                var new_checkout = new CheckoutBookViewModel
                {
                    Id = checkout.Id,
                    UserName = checkout.UserName,
                    BookId = checkout.BookId,
                    Title = book!.Title,
                    Author = book!.Author,
                    StartTime = checkout.StartTime,
                    EndTime = checkout.EndTime
                };
                checkouts = checkouts.Concat(new[] { new_checkout });
            }

            if (!string.IsNullOrEmpty(title))
            {
                checkouts = checkouts.Where(b => b.Title!.ToUpper().Contains(title.ToUpper()));
            }

            if (!string.IsNullOrEmpty(author))
            {
                checkouts = checkouts.Where(b => b.Author!.ToUpper().Contains(author.ToUpper()));
            }

            if (!string.IsNullOrEmpty(userName))
            {
                checkouts = checkouts.Where(b => b.UserName!.ToUpper().Contains(userName.ToUpper()));
            }

            return View(checkouts);
        }

        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> IndexReader(string title, string author)
        {
            if (_context.Reservation == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservation' is null.");
            }

            IEnumerable<CheckoutBookViewModel> checkouts = new List<CheckoutBookViewModel>();

            foreach (var checkout in _context.Checkout)
            {
                var book = await _context.Book.FindAsync(checkout.BookId);
                var new_checkout = new CheckoutBookViewModel
                {
                    Id = checkout.Id,
                    UserName = checkout.UserName,
                    BookId = checkout.BookId,
                    Title = book!.Title,
                    Author = book!.Author,
                    StartTime = checkout.StartTime,
                    EndTime = checkout.EndTime
                };
                checkouts = checkouts.Concat(new[] { new_checkout });
            }

            checkouts = checkouts.Where(b => b.UserName == User.Identity!.Name);

            if (!string.IsNullOrEmpty(title))
            {
                checkouts = checkouts.Where(b => b.Title!.ToUpper().Contains(title.ToUpper()));
            }

            if (!string.IsNullOrEmpty(author))
            {
                checkouts = checkouts.Where(b => b.Author!.ToUpper().Contains(author.ToUpper()));
            }

            return View(checkouts);
        }


        // GET: Checkouts/Checkout
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Checkout(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            var book = await _context.Book.FindAsync(reservation.BookId);
            if (book == null)
            {
                return NotFound();
            }

            var reservation_book = new ReservationBookViewModel
            {
                Id = reservation.Id,
                UserName = reservation.UserName,
                BookId = reservation.BookId,
                Title = book.Title,
                Author = book.Author,
                ReservationDate = reservation.ReservationDate,
                ValidDate = reservation.ValidDate
            };

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "This book was recently modified by another user. "
                        + "Check out operation was canceled. "
                        + "If you still want to check out this book, "
                        + "click the Checkout button again. Otherwise "
                        + "click the Back to List hyperlink.";
            }

            return View(reservation_book);
        }

        // POST: Checkouts/Checkout
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Checkout(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            var book = await _context.Book.FindAsync(reservation.BookId);
            if (book == null)
            {
                return NotFound();
            }
            if (book.Status != "Reserved")
            {
                ViewData["ErrorMessage"] = "You can not check out this book. "
                    + "It is no longer reserved. "
                    + "Click the Back to List hyperlink.";

                var reservation_book = new ReservationBookViewModel
                {
                    Id = reservation.Id,
                    UserName = reservation.UserName,
                    BookId = reservation.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    ReservationDate = reservation.ReservationDate,
                    ValidDate = reservation.ValidDate
                };
                return View(reservation_book);
            }

            try
            {
                book.Status = "Checked out";
                _context.Book.Update(book);
                _context.Reservation.Remove(reservation);
                Checkout checkout = new Checkout()
                {
                    UserName = reservation.UserName,
                    BookId = book.Id,
                    StartTime = DateTime.Now,
                    EndTime = null
                };
                _context.Checkout.Add(checkout);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(IndexLibrarian));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction(nameof(Checkout), new {concurrecyError = true, id = reservation.Id});
            }
            
        }

        // GET: Checkouts/EndCheckout/5
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> EndCheckout(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkout = await _context.Checkout.FindAsync(id);
            if (checkout == null)
            {
                return NotFound();
            }
            var book = await _context.Book.FindAsync(checkout.BookId);
            if (book == null)
            {
                return NotFound();
            }

            var checkout_book = new CheckoutBookViewModel
            {
                Id = checkout.Id,
                UserName = checkout.UserName,
                BookId = checkout.BookId,
                Title = book.Title,
                Author = book.Author,
                StartTime = checkout.StartTime,
                EndTime = checkout.EndTime
            };

            if (concurrencyError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "This book was recently modified by another user. "
                        + "Checkout is still valid. "
                        + "If you want to end checkout of this book, "
                        + "click the Endheckout button again. Otherwise "
                        + "click the Back to List hyperlink.";
            }

            return View(checkout_book);
        }

        // POST: Checkouts/EndCheckout/5
        [HttpPost, ActionName("EndCheckout")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> EndCheckoutConfirmed(int id)
        {
            var checkout = await _context.Checkout.FindAsync(id);
            if (checkout == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(checkout.BookId);
            if (book == null )
            {
                return NotFound();
            }
            if (book.Status != "Checked out" || checkout.EndTime != null)
            {
                ViewData["ErrorMessage"] = "The status of this book was recently changed. "
                    + "It is no longer checked out. "
                    + "Click the Back to List hyperlink.";

                var checkout_book = new CheckoutBookViewModel
                {
                    Id = checkout.Id,
                    UserName = checkout.UserName,
                    BookId = checkout.BookId,
                    Title = book.Title,
                    Author = book.Author,
                    StartTime = checkout.StartTime,
                    EndTime = checkout.EndTime
                };

                return View(checkout_book);
            }

            try
            {
                book.Status = "Available";
                _context.Book.Update(book);
                checkout.EndTime = DateTime.Now;
                _context.Checkout.Update(checkout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction(nameof(EndCheckout), new { concurrencyError = true, id = checkout.Id });
            }
        }

        private bool CheckoutExists(int id)
        {
            return _context.Checkout.Any(e => e.Id == id);
        }
    }
}
