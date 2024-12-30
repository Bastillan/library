using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReactLibrary.Server.Models;
using ReactLibrary.Server.Models.Api;

namespace ReactLibrary.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        { }

        public DbSet<ReactLibrary.Server.Models.Book> Book { get; set; } = default!;
        public DbSet<ReactLibrary.Server.Models.Reservation> Reservation { get; set; } = default!;
        public DbSet<ReactLibrary.Server.Models.Checkout> Checkout { get; set; } = default!;
        public DbSet<ReactLibrary.Server.Models.Api.CheckoutDTO> CheckoutDTO { get; set; } = default!;
    }
}
