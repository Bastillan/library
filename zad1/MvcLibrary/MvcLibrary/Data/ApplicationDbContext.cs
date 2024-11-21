using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcLibrary.Models;

namespace MvcLibrary.Data
{
    public class ApplicationDbContext : IdentityDbContext<MvcLibrary.Models.ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<MvcLibrary.Models.Book> Book { get; set; } = default!;
    }
}
