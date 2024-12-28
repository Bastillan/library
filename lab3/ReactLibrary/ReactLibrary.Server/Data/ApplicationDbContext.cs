using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReactLibrary.Server.Models;

namespace ReactLibrary.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ReactLibrary.Server.Models.ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        { }
    }
}
