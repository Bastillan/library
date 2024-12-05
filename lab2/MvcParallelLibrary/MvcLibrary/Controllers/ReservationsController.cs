using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcLibrary.Data;
using MvcLibrary.Models;

namespace MvcLibrary.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
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
            
            IEnumerable<ReservationBookViewModel> reservations = new List<ReservationBookViewModel>();

            foreach(var reservation in _context.Reservation)
            {
                if (!StillReserved(reservation))
                {
                    continue;
                }
                
                var book = await _context.Book.FindAsync(reservation.BookId);
                var new_reservation = new ReservationBookViewModel
                {
                    Id = reservation.Id,
                    UserName = reservation.UserName,
                    BookId = reservation.BookId,
                    Title = book!.Title,
                    Author = book!.Author,
                    ReservationDate = reservation.ReservationDate,
                    ValidDate = reservation.ValidDate
                };
                reservations = reservations.Concat(new[] { new_reservation });
            }

            if (!string.IsNullOrEmpty(title))
            {
                reservations = reservations.Where(b => b.Title!.ToUpper().Contains(title.ToUpper()));
            }

            if (!string.IsNullOrEmpty(author))
            {
                reservations = reservations.Where(b => b.Author!.ToUpper().Contains(author.ToUpper()));
            }

            if (!string.IsNullOrEmpty(userName))
            {
                reservations = reservations.Where(b => b.UserName!.ToUpper().Contains(userName.ToUpper()));
            }

            return View(reservations);
        }

        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> IndexReader(string title, string author)
        {
            if (_context.Reservation == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reservation' is null.");
            }

            IEnumerable<ReservationBookViewModel> reservations = new List<ReservationBookViewModel>();

            foreach (var reservation in _context.Reservation)
            {
                if (!StillReserved(reservation))
                {
                    continue;
                }

                var book = await _context.Book.FindAsync(reservation.BookId);
                var new_reservation = new ReservationBookViewModel
                {
                    Id = reservation.Id,
                    UserName = reservation.UserName,
                    BookId = reservation.BookId,
                    Title = book!.Title,
                    Author = book!.Author,
                    ReservationDate = reservation.ReservationDate,
                    ValidDate = reservation.ValidDate
                };
                reservations = reservations.Concat(new[] { new_reservation });
            }
            reservations = reservations.Where(b => b.UserName == User.Identity!.Name);

            if (!string.IsNullOrEmpty(title))
            {
                reservations = reservations.Where(b => b.Title!.ToUpper().Contains(title.ToUpper()));
            }

            if (!string.IsNullOrEmpty(author))
            {
                reservations = reservations.Where(b => b.Author!.ToUpper().Contains(author.ToUpper()));
            }

            return View(reservations);
        }

        // GET: Reservations/Reserve
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> Reserve(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FirstOrDefaultAsync(b => b.Id == id);
            
            if (book == null)
            {
                return NotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                if (book.Status != "Available")
                {
                    ViewData["ErrorMessage"] = "The book you attempted to reserve "
                    + "is no longer available."
                    + "Click the Back to List hyperlink.";
                }
                else
                {
                    ViewData["ErrorMessage"] = "The details of the book you attempted to reserve "
                    + "was modified by another user. "
                    + "Your reserve operation was canceled. "
                    + "If you still want to reserve this "
                    + "book, click the Reserve button again. Otherwise "
                    + "click the Back to List hyperlink.";
                }
            }

            return View(book);
        }

        // POST: Reservations/Reserve
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Reader")]
        public async Task<IActionResult> Reserve(int? id, byte[] rowVersion)
        {
            var book = await _context.Book.FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                return NotFound();
            }


            if (book.Status == "Available")
            {
                try
                {
                    book.Status = "Reserved";
                    _context.Update(book);
                    Reservation reservation = new Reservation()
                    {
                        UserName = User.Identity!.Name,
                        BookId = book.Id,
                        ReservationDate = DateTime.Now,
                        ValidDate = DateTime.Now.AddDays(2).Date
                    };
                    _context.Reservation.Add(reservation);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return RedirectToAction(nameof(Reserve), new {concurrencyError = true, id = book.Id });
                }
            }
            ViewData["ErrorMessage"] = "You can not reserve this book. "
                    + "Click the Back to List hyperlink.";
            return View(book);
        }

        // GET: Reservations/Unreserve/5
        //[Authorize(Roles = "Librarian")]
        [Authorize(Roles = "Reader, Librarian")]
        public async Task<IActionResult> Unreserve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Unreserve/5
        [HttpPost, ActionName("Unreserve")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Reader, Librarian")]
        public async Task<IActionResult> UnreserveConfirmed(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservation.Remove(reservation);
                var book = await _context.Book.FirstOrDefaultAsync(b => b.Id == reservation.BookId);

                if (book == null)
                {
                    return NotFound();
                }

                if (book.Status == "Reserved")
                {
                    book.Status = "Available";

                    _context.Update(book);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.Id == id);
        }

        private bool StillReserved(Reservation reservation)
        {
            if (DateTime.Now > reservation.ValidDate)
            {
                var book = _context.Book.FirstOrDefault(b => b.Id == reservation.BookId);
                book!.Status = "Available";
                _context.Book.Update(book);
                _context.Reservation.Remove(reservation);
                return false;
            }
            return true;
        }
    }
}
