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

            if (!string.IsNullOrEmpty(title))
            {
                checkouts = checkouts.Where(b => b.Title!.ToUpper().Contains(title.ToUpper()));
            }

            if (!string.IsNullOrEmpty(author))
            {
                checkouts = checkouts.Where(b => b.Author!.ToUpper().Contains(author.ToUpper()));
            }
            checkouts = checkouts.Where(b => b.UserName == User.Identity!.Name);

            return View(checkouts);
        }


        // GET: Checkouts/Create
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Checkout(int? id)
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

            return View(reservation);
        }

        // POST: Checkouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Checkout(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            var book = await _context.Book.FindAsync(reservation!.BookId);
            book!.Status = "Checked out";
            Checkout checkout = new Checkout()
            {
                UserName = reservation.UserName,
                BookId = book.Id,
                StartTime = DateTime.Now,
                EndTime = null
            };
            await _context.Checkout.AddAsync(checkout);
            _context.Book.Update(book);
            _context.Reservation.Remove(reservation!);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(IndexLibrarian));
        }

        // GET: Checkouts/Delete/5
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> EndCheckout(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var checkout = await _context.Checkout
                .FirstOrDefaultAsync(m => m.Id == id);
            if (checkout == null)
            {
                return NotFound();
            }

            return View(checkout);
        }

        // POST: Checkouts/Delete/5
        [HttpPost, ActionName("EndCheckout")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> EndCheckoutConfirmed(int id)
        {
            var checkout = await _context.Checkout.FindAsync(id);
            var book = await _context.Book.FindAsync(checkout!.BookId);

            if (checkout != null && checkout.EndTime == null)
            {
                checkout.EndTime = DateTime.Now;
                _context.Checkout.Update(checkout);
            }
            if(book != null)
            {
                book.Status = "Available";
                _context.Book.Update(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CheckoutExists(int id)
        {
            return _context.Checkout.Any(e => e.Id == id);
        }
    }
}
