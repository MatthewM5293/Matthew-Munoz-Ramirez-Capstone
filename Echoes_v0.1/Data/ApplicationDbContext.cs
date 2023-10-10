using Echoes_v0._1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Echoes_v0._1.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    //public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public DbSet<Echoes_v0._1.Models.ApplicationUser> ApplicationUsers { get; set; } = default!;

    //public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public DbSet<Echoes_v0._1.Models.PostModel> PostModel { get; set; } = default!;
}