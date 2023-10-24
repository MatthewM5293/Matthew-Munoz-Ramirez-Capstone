﻿using Echoes_v0._1.Data;
using Echoes_v0._1.Interfaces;
using Echoes_v0._1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using System.Security.Claims;

namespace Echoes_v0._1.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDataAccessLayer dal;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    private static string? UserId;
    private static string? PostId;
    private static string? UserName;

    public HomeController(ILogger<HomeController> logger, IDataAccessLayer indal, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        dal = indal;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
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

        HomeModel homeModel = new HomeModel();

        var posts = _context.PostModel.ToList();
        var allComments = _context.CommentModel.ToList();
        var allLikes = _context.LikeModel.ToList();
        homeModel.Posts = posts;

        //populates Posts, comments, and Likes
        //shows comments with corresonding posts, can do the same with profiles

        foreach (PostModel model in posts)
        {
            //model.Comments = _context.CommentModel.ToList();
            model.Comments = allComments.Where(c => c.PostId == model.PostId).ToList();
            //likes
            model.LikedBy = allLikes.Where(l => l.PostId == model.PostId).ToList();
        }

        return View(homeModel);
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
    public IActionResult ProfilePost(string? id)
    {
        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.UserId = UserId;

        if (id == null) return NotFound();

        ApplicationUser foundUser = dal.GetUser(id);
        if (foundUser == null) return NotFound();

        //populate Followers
        var allFollowers = _context.UserFollowModels.ToList();
        foundUser.Followers = allFollowers.Where(fm => fm.FollowingUserId.ToString().Equals(id)).ToList();
        foundUser.Following = allFollowers.Where(fm => fm.CurrentUserId.ToString().Equals(id)).ToList();

        return View("Profile/Profile", foundUser);
    }

    [HttpGet]
    public IActionResult EditProfile(string? id)
    {
        id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == null)
            return NotFound();

        ApplicationUser foundUser = dal.GetUser(id);

        if (foundUser == null) return NotFound();

        ViewBag.UserId = UserId;
        return View("Profile/EditProfile", foundUser);
    }

    [HttpPost]
    public IActionResult EditProfile(ApplicationUser ap)
    {
        ApplicationUser foundUser = dal.GetUser(ap.Id);
        if (foundUser == null) return NotFound();

        if (!(ap.ProfilePicture.Contains(".png") || ap.ProfilePicture.Contains(".jpg")))
        {
            ModelState.AddModelError("Invalid Image Format", "Invalid Format");
        }
        if (ap.Uname != foundUser.Uname && !dal.IsValidUserName(ap.Uname)) ModelState.AddModelError("Username Taken", "Username is being used by another user!");

        if (!ModelState.IsValid)
        {
            return View("Profile/EditProfile", foundUser);
        }
        else
        {
            string stringFileName = UploadFile(ap);
            if (stringFileName != null)
            {
                //delete old pfp
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "ProfilePictures");
                filePath += "\\" + foundUser.ProfilePicture;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                //update
                ap.ProfilePicture = stringFileName;
                foundUser.ProfilePicture = ap.ProfilePicture;

            }
            
            foundUser.UserName = ap.UserName;
            foundUser.Name = ap.Name;
            foundUser.DateOfBirth = ap.DateOfBirth;
            foundUser.Bio = ap.Bio;
            dal.EditUser(foundUser);
        }

        return RedirectToAction("Profile", "Home");
    }

    //follow
    public async Task<IActionResult> FollowUserAsync(Guid? userId)
    {
        //get users
        var userFollowing = _context.ApplicationUsers.FirstOrDefault(ap => ap.Id.Equals(userId.ToString()));
        var currentUser = _context.ApplicationUsers.FirstOrDefault(ap => ap.Id.Equals(UserId));


        if (userFollowing == null)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            //create follow model
            UserFollowModel followModel = new UserFollowModel();

            followModel.Id = Guid.NewGuid();
            followModel.CurrentUserId = new Guid(UserId);
            followModel.FollowingUserId = new Guid(userFollowing.Id);

            //gets all User's following
            userFollowing.Followers = _context.UserFollowModels.Where(fm => fm.FollowingUserId.Equals(userId)).ToList();

            //add userID to list
            userFollowing.Followers.Add(followModel);
            userFollowing.FollowersCount = userFollowing.Followers.Count();

            //update current User's following count
            currentUser.FollowersCount++;

            //update db
            _context.UserFollowModels.Add(followModel);
            await _context.SaveChangesAsync();
        }

        //return user to page
        return ProfilePost(userFollowing.Id.ToString());

    }

    public async Task<IActionResult> UnFollowUserAsync(Guid? userId)
    {
        //get user being unfollowed
        var userFollowing = _context.ApplicationUsers.FirstOrDefault(ap => ap.Id.Equals(userId.ToString()));
        var currentUser = _context.ApplicationUsers.FirstOrDefault(ap => ap.Id.Equals(UserId));
        //var post = _context.PostModel.FirstOrDefault(p => p.PostId.Equals(postId));

        if (userFollowing == null)
        {
            //redirect to profile viewing

            TempData["StatusMessage"] = "Profile cannot be unfollowed, Account may be deleted";
            return RedirectToAction("Index", "Home");
        }
        else
        {
            //gets all user's following
            userFollowing.Followers = _context.UserFollowModels.Where(fm => fm.FollowingUserId.Equals(userId)).ToList();
            var followModel = userFollowing.Followers.FirstOrDefault(fm => fm.CurrentUserId.Equals(new Guid(UserId)));

            //post.LikedBy = _context.LikeModel.Where(l => l.PostId.Equals(postId)).ToList();
            //var like = post.LikedBy.FirstOrDefault(l => l.UserId.Equals(new Guid(UserId)));

            //update current User's following count
            currentUser.FollowersCount--;

            //update db
            _context.UserFollowModels.Remove(followModel);
            userFollowing.FollowersCount = userFollowing.Followers.Count();
            await _context.SaveChangesAsync();
        }

        return ProfilePost(userFollowing.Id.ToString());
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
        post.PostDate = DateTime.Now;

        //to set Username and PFP
        post.UserName = UserName;
        //post.ProfilePicture = foundUser.ProfilePictureUrl;

        if (!ModelState.IsValid || _context.PostModel == null || post == null)
        {
            TempData["StatusMessage"] = "Post failed to create Post";
            return RedirectToAction("Index", "Home");
        }

        //Images
        string stringFileName = UploadFile(post);
        if (stringFileName != null)
        {
            post.ImageUrl = stringFileName;
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

    [HttpGet]
    public IActionResult EditPost(Guid? id)
    {
        if (id == null)
            return NotFound();

        PostModel foundPost = _context.PostModel.FirstOrDefault(p => p.PostId.Equals(id));

        if (foundPost == null) return NotFound();
        //notification

        return View("Post/EditPost", foundPost);
    }

    [HttpPost]
    public async Task<IActionResult> EditPost(PostModel postModel)
    {
        if (ModelState.IsValid)
        {
            //check if post is different than original
            PostModel foundPost = _context.PostModel.FirstOrDefault(p => p.PostId.Equals(postModel.PostId));
            if (foundPost.Caption == postModel.Caption)
            {
                postModel.EditDate = null;

                TempData["StatusMessage"] = "Nothing to be updated";
                return RedirectToAction("Index", "Home", fragment: postModel.PostId.ToString());
            } 

            _context.PostModel.Update(postModel);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Post successfully updated";
            return RedirectToAction("Index", "Home", fragment: postModel.PostId.ToString());
        }
        return View("Post/EditPost", postModel);
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
            //deletes comments of post
            var comments = _context.CommentModel.Where(c => c.PostId.Equals(id)).ToList();
            foreach (var comment in comments)
            {
                _context.CommentModel.Remove(comment);
            }

            //deletes likes of post
            var likes = _context.LikeModel.Where(l => l.PostId.Equals(id)).ToList();
            foreach (var like in likes)
            {
                _context.LikeModel.Remove(like);
            }

            //dal.RemovePost(id);
            _ = _context.PostModel.Remove(post);
            await _context.SaveChangesAsync();

            //delete Images
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
            filePath += "\\" + post.ImageUrl;
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);    
            }

        }
        else
        {
            return View();
        }
        return RedirectToAction("Index", "Home");
    }

    //likes
    public async Task<IActionResult> LikePostAsync(Guid? postId)
    {
        //get post
        var post = _context.PostModel.FirstOrDefault(p => p.PostId.Equals(postId));

        if (post == null)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            //create Like model
            var LikeModel = new LikeModel();

            LikeModel.Id = Guid.NewGuid();
            LikeModel.UserId = new Guid(UserId);
            LikeModel.PostId = post.PostId;

            //gets all post's likes
            post.LikedBy = _context.LikeModel.Where(l => l.PostId.Equals(postId)).ToList();

            //add userID to list
            post.LikedBy.Add(LikeModel);
            post.LikeCount = post.LikedBy.Count();

            //update Post
            _context.LikeModel.Add(LikeModel);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home", fragment: post.PostId.ToString());
    }

    public async Task<IActionResult> UnlikePostAsync(Guid? postId)
    {
        //get post
        var post = _context.PostModel.FirstOrDefault(p => p.PostId.Equals(postId));

        if (post == null)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            //gets all post's likes
            post.LikedBy = _context.LikeModel.Where(l => l.PostId.Equals(postId)).ToList();
            var like = post.LikedBy.FirstOrDefault(l => l.UserId.Equals(new Guid(UserId)));

            //update Post
            _context.LikeModel.Remove(like);
            post.LikeCount = post.LikedBy.Count();
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home", fragment: post.PostId.ToString());
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

    #region Search

    [HttpGet]
    public IActionResult Search()
    {

        TempData["key"] = "";

        SearchViewModel searchModel = new SearchViewModel();
        searchModel.Users = _context.ApplicationUsers.ToList();
        searchModel.Posts = _context.PostModel.ToList();
        return View("Search/UserSearch", searchModel);
    }

    [HttpPost]
    public IActionResult Search(string key)
    {
        SearchViewModel searchModel = new SearchViewModel();
        searchModel.Users = _context.ApplicationUsers.ToList();
        searchModel.Posts = _context.PostModel.ToList();

        if (String.IsNullOrEmpty(key))
        {
            return View("Search/UserSearch", searchModel);
        }

        TempData["key"] = key;

        //returns searched
        //filters
        searchModel.Users = searchModel.Users.Where(c => c.Uname.ToLower().Contains(key.ToLower())).ToList();
        searchModel.Posts = searchModel.Posts.Where(p => p.Caption.ToLower().Contains(key.ToLower())).ToList();

        return View("Search/UserSearch", searchModel);
    }
    #endregion

    #region Helped Methods

    private string UploadFile(PostModel postModel) 
    {
        string filename = null;
        if (postModel.Image != null) 
        {
            // Create the directory if it does not exist.
            if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "Images")))
            {
                Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "Images"));
            }

            string uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "Images");
            filename = Guid.NewGuid().ToString() + "-" + postModel.Image.FileName;

            string filePath = Path.Combine(uploadDirectory, filename);
            
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                postModel.Image.CopyTo(fileStream);
            }
        }

        return filename;
    }
    
    private string UploadFile(ApplicationUser applicationUser) 
    {
        string filename = null;
        if (applicationUser.Image != null) 
        {
            // Create the directory if it does not exist.
            if (!Directory.Exists(Path.Combine(_webHostEnvironment.WebRootPath, "ProfilePictures")))
            {
                Directory.CreateDirectory(Path.Combine(_webHostEnvironment.WebRootPath, "ProfilePictures"));
            }

            if (Directory.Exists("/wwwroot/ProfilePictures"))
            {
                string uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "ProfilePictures");
                filename = Guid.NewGuid().ToString() + "-" + applicationUser.Image.FileName;
                string filePath = Path.Combine(uploadDirectory, filename);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    applicationUser.Image.CopyTo(fileStream);
                }
            }

        }

        return filename;
    }

    //populate likes
    private void PopulatePosts() 
    {
        var posts = _context.PostModel.ToList();
        var allComments = _context.CommentModel.ToList();
        var allLikes = _context.LikeModel.ToList();
    }

    #endregion

}