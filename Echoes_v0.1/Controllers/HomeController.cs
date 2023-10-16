using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Echoes_v0._1.Models;
using Echoes_v0._1.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Echoes_v0._1.Data;
using Microsoft.Extensions.Hosting;

namespace Echoes_v0._1.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private static string? UserId;
    private static string? PostId;
    private static string? UserName;

    private readonly IDataAccessLayer dal;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, IDataAccessLayer indal, ApplicationDbContext context)
    {
        _logger = logger;
        dal = indal;
        _context = context;
    }

    public IActionResult Index()
    {
        if (User != null)
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.UserId = UserId;

            if (dal.GetUser(UserId) != null)
            {
                UserName = dal.GetUser(UserId).Uname;
                ViewBag.UserName = UserName; //Viewbag data
            }
        }

        //var posts = dal.GetPosts();
        var posts = _context.PostModel.ToList();
        var allComments = _context.CommentModel.ToList();

        //shows comments with corresonding posts, can do the same with profiles
        foreach (PostModel model in posts)
        {
            //model.Comments = _context.CommentModel.ToList();
            model.Comments = allComments.Where(c => c.PostId == model.PostId).ToList();
        }

        return View(posts);
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
        //var temp = User.FindFirstValue(ClaimTypes.NameIdentifier);
        post.PostId = Guid.NewGuid();
        post.UserId = new Guid(UserId);

        //to set Username and PFP
        //post.Username = foundUser.UName;
        //post.ProfilePicture = foundUser.ProfilePictureUrl;

        if (!ModelState.IsValid || _context.PostModel == null || post == null)
        {
            TempData["StatusMessage"] = "Post failed to update";
            return View("Post/CreatePost");
        }

        _context.PostModel.Add(post);
        await _context.SaveChangesAsync();


        TempData["StatusMessage"] = "Post successfully created";
        return RedirectToAction("Index", "Home", fragment: post.PostId.ToString());
    }

    //display post
    public IActionResult Post()
    {
        return View("Post/Details");
    }

    public IActionResult EditPost(Guid? id)
    {
        if (id == null)
            return NotFound();

        PostModel foundPost = _context.PostModel.FirstOrDefault(p => p.PostId.Equals(id));

        if (foundPost == null) return NotFound();

        return View("Post/EditPost", foundPost);
    }

    [HttpPost]
    public async Task<IActionResult> EditPost(PostModel postModel)
    {
        if (ModelState.IsValid)
        {
            //check if post is different than original

            _context.PostModel.Update(postModel);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Post successfully updated";
            return RedirectToAction("Index", "Home", fragment: postModel.PostId.ToString());
        }
        return View();
    }
    
    public async Task<IActionResult> DeletePostAsync(Guid? id)
    {
        var post = _context.PostModel.FirstOrDefault(p => p.PostId.Equals(id));
        if (post == null)
        {
            //validator
            ModelState.AddModelError("PostId", "Cannot find post to delete");
        }
        if (ModelState.IsValid)
        {
            //temp delete
            //deletes comments of post
            var comments = _context.CommentModel.Where(c => c.PostId.Equals(id)).ToList();
            foreach (var comment in comments)
            {
                //DeleteComment(comment.Id);
                
                //back up
                //comment.PostId = Guid.Empty;
                _context.CommentModel.Remove(comment);
            }

            //dal.RemovePost(id);
            _context.PostModel.Remove(post);
            await _context.SaveChangesAsync();

        }
        else
        {
            return View();
        }
        return RedirectToAction("Index", "Home");
    }
    #endregion


    #region Comment Functions

    [HttpGet]
    public IActionResult AddComment(string? PostID) 
    {
        if (PostID != null)
        {
            ViewBag.PostID = PostID; //viewbag data
            PostId = PostID; // static string
            return View("Comment/CreateComment");
        }
        else
        {
            //notification "post does not exist"
            return RedirectToAction("Index", "Home"); //reload home if failed
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddCommentAsync(CommentModel comment)
        
    {
        //var temp = User.FindFirstValue(ClaimTypes.NameIdentifier);
        comment.CommentId = Guid.NewGuid();
        comment.PostId = new Guid(PostId); 
        comment.UserId = new Guid(UserId);

        //to set Username and PFP
        comment.Username = UserName;
        //comment.ProfilePicture = foundUser.ProfilePictureUrl;

        if (comment.Message.Length < 1 || _context == null || comment == null)
        {
            return View("Comment/CreateComment");
        }

        _context.CommentModel.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home", fragment: comment.PostId.ToString());

        //return View("Comment/CreateComment");
    }

    [HttpGet]
    public IActionResult EditComment(Guid? id) 
    {
        if (id == null) return NotFound();

        //CommentModel foundComment = dal.GetComment(id);
        CommentModel foundComment = _context.CommentModel.FirstOrDefault(c => c.CommentId.Equals(id));

        if (foundComment == null) return NotFound();

        return View("Comment/EditComment", foundComment);
    }

    [HttpPost]
    public async Task<IActionResult> EditCommentAsync(CommentModel comment) 
    {
        if (comment.Message.Length < 1 || _context == null || comment == null)
        {
            return View("Comment/EditComment", comment);
        }

        _context.CommentModel.Update(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home", fragment: comment.PostId.ToString());
    }
    
    //[HttpPost]
    public async Task<IActionResult> DeleteCommentAsync(CommentModel comment) 
    {
        //CommentModel foundComment;
        if (comment == null)
        {
            //validator
            ModelState.AddModelError("CommentId", "Cannot find comment to delete");
        }
        else 
        {
            //comment.PostId = Guid.Empty;
            _context.CommentModel.Remove(comment); //dal method?
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home", fragment: comment.PostId.ToString());
    }






    #endregion

}