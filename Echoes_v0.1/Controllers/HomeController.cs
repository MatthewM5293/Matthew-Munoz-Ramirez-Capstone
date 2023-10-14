using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Echoes_v0._1.Models;
using Echoes_v0._1.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Echoes_v0._1.Data;

namespace Echoes_v0._1.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static string? UserId;
    private static int? PostId;
    private static string? UserName;

    private readonly IDataAccessLayer dal;
    private readonly ApplicationDbContext _context;

    [BindProperty]
    public PostModel PostModel { get; set; } = default!;

    public HomeController(ILogger<HomeController> logger, IDataAccessLayer indal, ApplicationDbContext context)
    {
        _logger = logger;
        dal = indal;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #region Profile Controls
    [HttpGet]
    public IActionResult Profile()
    {
        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.UserId = UserId;

        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == null) return NotFound();
        
        ApplicationUser foundUser = dal.GetUser(id);
        if (foundUser == null) return NotFound();

        return View("Profile/Profile", foundUser);
    }

    //[HttpPost]
    public IActionResult Profile(string? id)
    {
        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.UserId = UserId;

        if (id == null) return NotFound();

        ApplicationUser foundUser = dal.GetUser(id);
        if (foundUser == null) return NotFound();

        return View("Profile/ProfilePost", foundUser);
    }

    [HttpGet]
    public IActionResult EditProfile(string? id)
    {
        id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == null)
            return NotFound();

        ApplicationUser foundUser = dal.GetUser(id);

        if (foundUser == null) return NotFound();

        return View("Profile/EditProfile", foundUser);
    }

    [HttpPost]
    public IActionResult EditProfile(ApplicationUser ap)
    {
        //will have to save Pfp, Username, Fn, Ln, and About seperately

        string temp = User.FindFirstValue(ClaimTypes.NameIdentifier);

        ApplicationUser foundUser = dal.GetUser(temp);
        if (foundUser == null) return NotFound();

        if (!ap.ProfilePicture.Contains(".png") || !ap.ProfilePicture.Contains(".jpg")) ModelState.AddModelError("Invalid Image Format", "Invalid Format");
        if (ap.Uname != foundUser.Uname && !dal.IsValidUserName(ap.Uname)) ModelState.AddModelError("Username Taken", "Username is being used by another user!");

        if (!ModelState.IsValid)
        {
            return View("Profile/EditProfile", foundUser);
        }
        else
        {
            foundUser.ProfilePicture = ap.ProfilePicture;
            foundUser.UserName = ap.UserName;
            foundUser.Name = ap.Name;
            foundUser.DateOfBirth = ap.DateOfBirth;
            foundUser.Bio = ap.Bio;
            dal.EditUser(foundUser);
        }

        return RedirectToAction("Profile", "Home");
    }
    #endregion

    #region Post Functions
    
    public IActionResult CreatePost()
    {
        return View("Post/CreatePost");
    }

    [HttpPost]
    public async Task<IActionResult> CreatePostAsync(PostModel post)
    {
        //
        var temp = User.FindFirstValue(ClaimTypes.NameIdentifier);
        PostModel.UserId = new Guid(temp);

        //to set Username and PFP
        //post.Username = foundUser.UName;
        //post.ProfilePicture = foundUser.ProfilePictureUrl;

        if (!ModelState.IsValid || _context.PostModel == null || post == null)
        {
            return View();
        }

        _context.PostModel.Add(post);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home", fragment: post.PostId.ToString());
    }

    //display post
    public IActionResult Post()
    {
        return View("Post/Details");
    }

    public IActionResult EditPost()
    {
        return View("Post/Edit");
    }
    
    public IActionResult DeletePost()
    {
        return View("Post/Delete");
    }
    #endregion



}