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

    public DbSet<ApplicationUser> ApplicationUsers { get; set; } = default!;

    public DbSet<PostModel> PostModel { get; set; } = default!;
    public DbSet<CommentModel> CommentModel { get; set; } = default!;
}