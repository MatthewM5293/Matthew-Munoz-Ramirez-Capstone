using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Echoes_v0._1.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Echoes_v0._1.Interfaces;
using Echoes_v0._1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
    //options.UseSqlite(connectionString)); //sql server
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

//User roles
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
  //  .AddDefaultTokenProviders()
    //.AddEntityFrameworkStores<ApplicationDbContext>();
    //.AddUserStore<ApplicationUserStore>()
    //.AddRoleStore<ApplicationRoleStore>()
    //.AddUserManager<ApplicationUserManager>()
    //.AddRoleManager<ApplicationRoleManager>()


builder.Services.AddTransient<IDataAccessLayer, EchoesDBDAL>(); //DAL for Models
//builder.Services.AddSingleton<IEmailSender, EmailSender>(); //for Emails

builder.Services.Configure<IdentityOptions>(options =>
{
    //user settings
    //options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+"; //Characters allowed
    options.User.RequireUniqueEmail = true; //1 account per email

    //password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
}
);

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); //?
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();