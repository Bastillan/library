using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactLibrary.Server.Data;
using ReactLibrary.Server.Models;
using ReactLibrary.Server.Models.Api;

namespace ReactLibrary.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CheckoutsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Checkouts
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CheckoutDTO>>> GetCheckout(string? title, string? author, string? userName)
        {
            IEnumerable<CheckoutDTO> checkouts = new List<CheckoutDTO>();

            foreach (var checkout in _context.Checkout)
            {
                var book = await _context.Book.FindAsync(checkout.BookId);
                var checkoutDTO = new CheckoutDTO
                {
                    Id = checkout.Id,
                    UserName = checkout.UserName,
                    BookId = checkout.BookId,
                    Title = book!.Title,
                    Author = book!.Author,
                    StartTime = checkout.StartTime,
                    EndTime = checkout.EndTime
                };
                checkouts = checkouts.Concat(new[] { checkoutDTO });
            }

            if (!string.IsNullOrEmpty(title))
            {
                checkouts = checkouts.Where(c => c.Title!.ToUpper().Contains(title.ToUpper()));
            }

            if (!string.IsNullOrEmpty(author))
            {
                checkouts = checkouts.Where(c => c.Author!.ToUpper().Contains(author.ToUpper()));
            }

            if (!string.IsNullOrEmpty(userName) && User.IsInRole("Librarian"))
            {
                checkouts = checkouts.Where(c => c.UserName!.ToUpper().Contains(userName.ToUpper()));
            }

            if (User.IsInRole("Reader"))
            {
                checkouts = checkouts.Where(c => c.UserName == User.Identity!.Name);
            }

            return checkouts.ToList();
        }

        // GET: api/Checkouts/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CheckoutDTO>> GetCheckout(int id)
        {
            var checkout = await _context.Checkout.FindAsync(id);

            if (checkout == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(checkout.BookId);
            var checkoutDTO = new CheckoutDTO
            {
                Id = checkout.Id,
                UserName = checkout.UserName,
                BookId = checkout.BookId,
                Title = book!.Title,
                Author = book!.Author,
                StartTime = checkout.StartTime,
                EndTime = checkout.EndTime
            };

            if (User.IsInRole("Reader") && checkout.UserName != User.Identity!.Name)
            {
                return Forbid();
            }

            return checkoutDTO;
        }

        // POST: api/Checkouts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpGet]
        [Route("checkout/{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<ActionResult<Checkout>> MakeCheckout(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return BadRequest();
            }
            var book = await _context.Book.FindAsync(reservation.BookId);
            if (book == null || book.Status != "Reserved")
            {
                return BadRequest();
            }

            try
            {
                book.Status = "Checked out";
                _context.Book.Update(book);
                _context.Reservation.Remove(reservation);

                Checkout checkout = new Checkout
                {
                    UserName = reservation.UserName,
                    BookId = book.Id,
                    StartTime = DateTime.Now,
                    EndTime = null
                };
                _context.Checkout.Add(checkout);

                await _context.SaveChangesAsync();
                return CreatedAtAction("GetCheckout", new { id = checkout.Id }, checkout);
            }
            catch (DbUpdateConcurrencyException)
            {
                var reservation_after = _context.Reservation.Find(id);
                if (reservation_after == null)
                {
                    return BadRequest();
                }

                var book_after = _context.Book.Find(reservation_after.BookId);
                if (book_after == null || book_after.Status != "Reserved")
                {
                    return BadRequest();
                }
                return Conflict();
            }
        }

        // DELETE: api/Checkouts/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Return(int id)
        {
            var checkout = await _context.Checkout.FindAsync(id);
            if (checkout == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(checkout.BookId);
            if (book == null || book.Status != "Checked out" || checkout.EndTime != null)
            {
                return BadRequest();
            }

            try
            {
                book.Status = "Available";
                _context.Book.Update(book);

                checkout.EndTime = DateTime.Now;
                _context.Checkout.Update(checkout);

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckoutExists(id))
                {
                    return NotFound();
                }
                var checkout_after = _context.Checkout.Find(id);
                if (checkout_after!.EndTime != null)
                {
                    return BadRequest();
                }
                var book_after = _context.Book.Find(checkout_after.BookId);
                if (book_after == null || book_after.Status != "Checked out")
                {
                    return BadRequest();
                }
                return Conflict();
            }
        }

        private bool CheckoutExists(int id)
        {
            return _context.Checkout.Any(e => e.Id == id);
        }
    }
}
