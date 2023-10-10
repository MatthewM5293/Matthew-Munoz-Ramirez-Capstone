using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Echoesv2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Echoesv2.Models.ApplicationUser> ApplicationUsers { get; set; } = default!;

        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<Echoesv2.Models.PostModel> PostModel { get; set; } = default!;
    }
}