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
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Reservations
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservation(string? title, string? author, string? userName)
        {
            IEnumerable<ReservationDTO> reservations = new List<ReservationDTO>();

            foreach (var reservation in _context.Reservation)
            {
                if (!StillReserved(reservation))
                {
                    continue;
                }

                var book = await _context.Book.FindAsync(reservation.BookId);
                var reservationDTO = new ReservationDTO
                {
                    Id = reservation.Id,
                    UserName = reservation.UserName,
                    BookId = reservation.BookId,
                    Title = book!.Title,
                    Author = book!.Author,
                    ReservationDate = reservation.ReservationDate,
                    ValidDate = reservation.ValidDate
                };
                reservations = reservations.Concat(new[] { reservationDTO });
            }

            if (!string.IsNullOrEmpty(title))
            {
                reservations = reservations.Where(r => r.Title!.ToUpper().Contains(title.ToUpper()));
            }

            if (!string.IsNullOrEmpty(author))
            {
                reservations = reservations.Where(r => r.Author!.ToUpper().Contains(author.ToUpper()));
            }

            if (!string.IsNullOrEmpty(userName) && User.IsInRole("Librarian"))
            {
                reservations = reservations.Where(r => r.UserName!.ToUpper().Contains(userName.ToUpper()));
            }

            if (User.IsInRole("Reader"))
            {
                reservations = reservations.Where(r => r.UserName == User.Identity!.Name);
            }

            return reservations.ToList();
        }

        // GET: api/Reservations/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(reservation.BookId);
            var reservationDTO = new ReservationDTO
            {
                Id = reservation.Id,
                UserName = reservation.UserName,
                BookId = reservation.BookId,
                Title = book!.Title,
                Author = book!.Author,
                ReservationDate = reservation.ReservationDate,
                ValidDate = reservation.ValidDate
            };

            if (User.IsInRole("Reader") && reservation.UserName != User.Identity!.Name)
            {
                return Forbid();
            }

            return reservationDTO;
        }

        // POST: api/Reservations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpGet]
        [Route("reserve/{id}")]
        [Authorize(Roles = "Reader")]
        public async Task<ActionResult<Reservation>> MakeReservation(int id)
        {
            var book = await _context.Book.FindAsync(id);

            if (book == null || book.Status != "Available")
            {
                return BadRequest();
            }

            try
            {
                book.Status = "Reserved";
                _context.Update(book);

                Reservation reservation = new Reservation
                {
                    UserName = User.Identity!.Name,
                    BookId = book.Id,
                    ReservationDate = DateTime.Now,
                    ValidDate = DateTime.Now.AddDays(2).Date
                };
                _context.Reservation.Add(reservation);

                await _context.SaveChangesAsync();
                return CreatedAtAction("GetReservation", new { id = reservation.Id }, reservation);
            }
            catch (DbUpdateConcurrencyException)
            {
                var book_after = _context.Book.Find(id);
                if (book_after == null || book_after.Status != "Available")
                {
                    return BadRequest();
                }
                return Conflict();
            }
        }

        // DELETE: api/Reservations/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservation.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            var book = await _context.Book.FindAsync(reservation.BookId);
            if (book == null || book.Status != "Reserved")
            {
                return Conflict();
            }

            try
            {
                book.Status = "Available";
                _context.Update(book);
                _context.Reservation.Remove(reservation);

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound();
                }
                var book_after = _context.Book.Find(reservation.BookId);
                if (book_after == null || book_after.Status != "Reserved")
                {
                    return BadRequest();
                }
                return Conflict();
            }
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservation.Any(e => e.Id == id);
        }

        private bool StillReserved(Reservation reservation)
        {
            if (DateTime.Now > reservation.ValidDate)
            {
                var book = _context.Book.Find(reservation.BookId);
                book!.Status = "Available";
                _context.Book.Update(book);
                _context.Reservation.Remove(reservation);
                return false;
            }
            return true;
        }
    }
}
