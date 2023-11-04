using Azure.Core.GeoJson;
using Echoes_v0._1.Data;
using Echoes_v0._1.Interfaces;
using Echoes_v0._1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace Echoes_v0._1.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDataAccessLayer _dal;
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;

    //TempData["AlertMessage"] = "Message";
    /*
     * @if (TempData["AlertMessage"] != null)
        {
            <div class="alert alert-dismissible alert-success sticky-top">
                @TempData["AlertMessage"]
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        }
     */

    public HomeController(ILogger<HomeController> logger, IDataAccessLayer dal, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _logger = logger;
        _dal = dal;
        _context = context;
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        //user specific
        HttpContext.Session.SetString("UserId", User.FindFirstValue(ClaimTypes.NameIdentifier)); //stores UserID
        ViewBag.UserId = HttpContext.Session.GetString("UserId"); //Gets UserID

        var id = HttpContext.Session.GetString("UserId");
        if (_dal.GetUser(id) != null)
        {
            HttpContext.Session.SetString("UserName", _dal.GetUser(HttpContext.Session.GetString("UserId")).Uname);
            ViewBag.UserName = HttpContext.Session.GetString("UserName");
        }

        //for when TABS are set up of "posts near you"
        //GeoPosition geoPosition = new GeoPosition();

        //set up
        HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(_dal.GetUser(id)));

        HomeModel homeModel = new HomeModel();

        var posts = _context.PostModel.OrderByDescending(p => p.PostDate).ToList();
        var follows = _context.UserFollowModels.Where(fm => fm.CurrentUserId.Equals(new Guid(id))).ToList();

        homeModel.Posts = posts;

        foreach (PostModel post in posts)
        {
            PopulatePost(post);
        }
        // Filter the posts for the users the current user follows
        homeModel.FollowedPosts = posts.Where(p => follows.Select(fm => fm.FollowingUserId).Contains(p.UserId)).ToList();

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
        ViewBag.UserId = HttpContext.Session.GetString("UserId");

        // Deserialize the JSON string to the User model
        string userJson = HttpContext.Session.GetString("UserData");
        ApplicationUser foundUser = JsonConvert.DeserializeObject<ApplicationUser>(userJson);

        HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(_dal.GetUser(HttpContext.Session.GetString("UserId"))));
        return View("Profile/Profile", foundUser);

    }

    public IActionResult ProfilePost(string? id)
    {
        HttpContext.Session.SetString("UserId", User.FindFirstValue(ClaimTypes.NameIdentifier));
        ViewBag.UserId = HttpContext.Session.GetString("UserId"); ;

        if (id == null) return NotFound();

        ApplicationUser userFound = _dal.GetUser(id);
        if (userFound == null) return NotFound();

        //populate Followers
        var allFollowers = _context.UserFollowModels.ToList();
        userFound.Followers = allFollowers.Where(fm => fm.FollowingUserId.ToString().Equals(id)).ToList();
        userFound.Following = allFollowers.Where(fm => fm.CurrentUserId.ToString().Equals(id)).ToList();

        return View("Profile/Profile", userFound);
    }

    [HttpGet]
    public IActionResult EditProfile(string? id)
    {
        id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == null)
            return NotFound();

        HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(_dal.GetUser(id)));

        ViewBag.UserId = HttpContext.Session.GetString("UserId");

        // Deserialize the JSON string to the User model
        string userJson = HttpContext.Session.GetString("UserData");
        ApplicationUser foundUser = JsonConvert.DeserializeObject<ApplicationUser>(userJson);

        return View("Profile/EditProfile", foundUser);
    }

    [HttpPost]
    public IActionResult EditProfile(ApplicationUser ap)
    {
        // Deserialize the JSON string to the User model
        string userJson = HttpContext.Session.GetString("UserData");
        ApplicationUser foundUser = JsonConvert.DeserializeObject<ApplicationUser>(userJson);

        if (ap.Uname != foundUser.Uname)
        {
            bool validName = _dal.IsValidUserName(ap.Uname);
            if (!validName)
            {
                ModelState.AddModelError("UserName", errorMessage: "Username is being used by another user!");
                //Notification
            }
        }

        if (!ModelState.IsValid)
        {
            return View("Profile/EditProfile", ap);
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

                //update pfp
                ap.ProfilePicture = stringFileName;
                foundUser.ProfilePicture = ap.ProfilePicture;
            }

            //update posts and comments
            if (foundUser.Uname != ap.Uname || stringFileName != null)
            {
                UpdateUserFootprint(new Guid(foundUser.Id), ap.Uname, foundUser.ProfilePicture);
            }

            foundUser.Uname = ap.Uname;
            foundUser.Name = ap.Name;
            foundUser.DateOfBirth = ap.DateOfBirth;
            foundUser.Bio = ap.Bio;
            _dal.EditUser(foundUser);

            //set userData Json
            HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(foundUser));
        }
        TempData["AlertMessage"] = "Profile Updated Successfully";
        return Profile();
    }

    public async Task<IActionResult> FollowUserAsync(Guid? userId)
    {
        //get users
        var userFollowing = _context.ApplicationUsers.FirstOrDefault(ap => ap.Id.Equals(userId.ToString()));
        var currentUser = _context.ApplicationUsers.FirstOrDefault(ap => ap.Id.Equals(HttpContext.Session.GetString("UserId")));

        if (userFollowing == null)
        {
            //notfication
            TempData["AlertMessage"] = "Account not found";
            return Index();
        }
        else
        {
            //create follow model
            UserFollowModel followModel = new UserFollowModel();

            followModel.Id = Guid.NewGuid();
            followModel.CurrentUserId = new Guid(HttpContext.Session.GetString("UserId"));
            followModel.FollowingUserId = new Guid(userFollowing.Id);

            //gets all User's following
            userFollowing.Followers = _context.UserFollowModels.Where(fm => fm.FollowingUserId.Equals(userId)).ToList();

            //add userID to list
            userFollowing.Followers.Add(followModel);
            userFollowing.FollowersCount = userFollowing.Followers.Count();

            //update current User's following count
            currentUser.FollowingCount++;

            //update db
            _context.UserFollowModels.Add(followModel);
            _context.ApplicationUsers.Update(currentUser);
            _context.ApplicationUsers.Update(userFollowing);
            await _context.SaveChangesAsync();
        }

        //return user to page
        return ProfilePost(userFollowing.Id.ToString());

    }

    public async Task<IActionResult> UnFollowUserAsync(Guid? userId)
    {
        //get user being unfollowed
        var userFollowing = _context.ApplicationUsers.FirstOrDefault(ap => ap.Id.Equals(userId.ToString()));
        var currentUser = _context.ApplicationUsers.FirstOrDefault(ap => ap.Id.Equals(HttpContext.Session.GetString("UserId")));

        if (userFollowing == null)
        {
            //redirect to profile viewing
            TempData["AlertMessage"] = "Profile cannot be unfollowed, Account may be deleted";
            return Index();
        }
        else
        {
            //gets all user's following
            userFollowing.Followers = _context.UserFollowModels.Where(fm => fm.FollowingUserId.Equals(userId)).ToList();
            var followModel = userFollowing.Followers.FirstOrDefault(fm => fm.CurrentUserId.Equals(new Guid(HttpContext.Session.GetString("UserId"))));

            currentUser.FollowingCount--;

            //update db
            _context.UserFollowModels.Remove(followModel);
            _context.ApplicationUsers.Update(currentUser);
            _context.ApplicationUsers.Update(userFollowing);
            userFollowing.FollowersCount = userFollowing.Followers.Count();
            await _context.SaveChangesAsync();
        }

        TempData["AlertMessage"] = "Profile unfollowed";
        return ProfilePost(userFollowing.Id.ToString());
    }


    #endregion

    #region Post Functions

    [HttpPost]
    public async Task<IActionResult> CreatePostAsync(PostModel post)
    {
        post.PostId = Guid.NewGuid();
        //post.UserId = new Guid(HttpContext.Session.GetString("UserId"));
        //post.UserName = String.Empty;
        if (!ModelState.IsValid || _context.PostModel == null || post == null || (String.IsNullOrEmpty(post.Caption) && String.IsNullOrEmpty(post.ImageUrl)))
        {
            TempData["AlertMessage"] = "Failed to create Post";
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ApplicationUser foundUser = JsonConvert.DeserializeObject<ApplicationUser>(HttpContext.Session.GetString("UserData"));
            //to set Username and PFP
            post.UserName = HttpContext.Session.GetString("UserName");
            post.ProfilePicture = foundUser.ProfilePicture;
            post.Caption = post.Caption.Trim();
            post.Name = foundUser.Name;

            //set location
            GeoPosition geoPosition = new GeoPosition();

            post.Latitude = geoPosition.Latitude;
            post.Longitude = geoPosition.Longitude;

            //Images
            string stringFileName = UploadFile(post);
            if (stringFileName != null)
            {
                post.ImageUrl = stringFileName;
            }

            _context.PostModel.Add(post);
            await _context.SaveChangesAsync();


            TempData["AlertMessage"] = "Post successfully created";
            return RedirectToAction("Index", "Home", fragment: post.PostId.ToString());
        }
    }

    //display post
    public IActionResult Post(Guid postId)
    {
        if (postId != null)
        {
            //static post string
            HttpContext.Session.SetString("PostId", postId.ToString());
            ViewBag.PostID = HttpContext.Session.GetString("PostId"); //viewbag data
        }
        ViewBag.UserId = HttpContext.Session.GetString("UserId");
        PostViewModel postViewModel = new PostViewModel
        {
            Post = _context.PostModel.FirstOrDefault(p => p.PostId.Equals(postId)),
            Comment = new CommentModel()
        };
        if (postViewModel.Post != null)
        {
            PopulatePost(postViewModel.Post);
            return View("Post/_PostView", postViewModel);
        }
        else
        {
            TempData["AlertMessage"] = "Post not found";
            return RedirectToAction("Index", "Home");
        }

    }

    [HttpGet]
    public IActionResult EditPost(Guid? id)
    {
        PostModel foundPost = _context.PostModel.FirstOrDefault(p => p.PostId.Equals(id));

        if (foundPost == null) { 
            TempData["AlertMessage"] = "Post not found";
            return RedirectToAction("Index", "Home");
        }

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

                TempData["AlertMessage"] = "Nothing to be updated";
                return RedirectToAction("Index", "Home", fragment: postModel.PostId.ToString());
            }
            foundPost.Caption = postModel.Caption;
            foundPost.EditDate = postModel.EditDate;
            _context.PostModel.Update(foundPost);
            await _context.SaveChangesAsync();

            TempData["AlertMessage"] = "Post successfully updated";
            //go back to where they were
            return RedirectToAction("Index", "Home", fragment: postModel.PostId.ToString());
        }
        TempData["AlertMessage"] = "Post failed to update";
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

            _ = _context.PostModel.Remove(post);
            await _context.SaveChangesAsync();

            //delete Image
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
        //Notification
        return RedirectToAction("Index", "Home");
    }

    //likes
    public async Task<IActionResult> LikePostAsync(Guid? postId)
    {
        //get post
        var post = _context.PostModel.FirstOrDefault(p => p.PostId.Equals(postId));

        if (post == null)
        {
            //notification
            return RedirectToAction("Index", "Home");
        }
        else
        {
            //create Like model
            var LikeModel = new LikeModel();

            LikeModel.Id = Guid.NewGuid();
            LikeModel.UserId = new Guid(HttpContext.Session.GetString("UserId"));
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

        //notification
        //load page user was last on
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
            var like = post.LikedBy.FirstOrDefault(l => l.UserId.Equals(new Guid(HttpContext.Session.GetString("UserId"))));

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
            HttpContext.Session.SetString("PostId", PostID);
            ViewBag.PostID = HttpContext.Session.GetString("PostId");

            return View("Comment/CreateComment");
        }
        else
        {
            //notification "post does not exist"
            return RedirectToAction("Index", "Home"); //reload home if failed
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddCommentAsync(string PostID, CommentModel comment)
    {
        if (PostID != null)
        {
            HttpContext.Session.SetString("PostId", PostID);
            ViewBag.PostID = HttpContext.Session.GetString("PostId");
        }
        comment.CommentId = Guid.NewGuid();
        comment.PostId = new Guid(HttpContext.Session.GetString("PostId"));
        comment.UserId = new Guid(HttpContext.Session.GetString("UserId"));

        //to set Username
        comment.Username = HttpContext.Session.GetString("UserName");

        if (comment.Message.Length < 1 || _context == null)
        {
            return Post(comment.PostId);
        }

        _context.CommentModel.Add(comment);
        await _context.SaveChangesAsync();
        Guid TempPostId = comment.PostId;
        comment = new CommentModel();

        return Post(TempPostId);
    }

    [HttpGet]
    public IActionResult EditComment(Guid? id)
    {
        if (id == null) return NotFound();

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

        return Post(comment.PostId);
    }

    [HttpPost]
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
            _context.CommentModel.Remove(comment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index", "Home", fragment: comment.PostId.ToString());
    }
    public async Task<IActionResult> DeleteCommentAsync(CommentModel comment, Guid id)
    {
        CommentModel foundComment = _context.CommentModel.FirstOrDefault(c => c.CommentId.Equals(id));
        if (foundComment == null)
        {
            //validator
            ModelState.AddModelError("CommentId", "Cannot find comment to delete");
            return Post(new Guid(HttpContext.Session.GetString("PostId")));
        }
        else
        {
            _context.CommentModel.Remove(foundComment);
            await _context.SaveChangesAsync();
        }

        return Post(foundComment.PostId);
    }

    #endregion

    #region Search

    [HttpGet]
    public IActionResult Search()
    {
        ViewBag.UserId = HttpContext.Session.GetString("UserId");
        TempData["key"] = "";

        SearchViewModel searchModel = new SearchViewModel();
        searchModel.Users = _context.ApplicationUsers.OrderBy(a => a.Uname).ToList();
        searchModel.Posts = _context.PostModel.OrderByDescending(p => p.PostDate).ToList();
        foreach (PostModel post in searchModel.Posts)
        {
            PopulatePost(post);
        }
        return View("Search/UserSearch", searchModel);
    }

    [HttpPost]
    public IActionResult Search(string key)
    {
        ViewBag.UserId = HttpContext.Session.GetString("UserId");

        SearchViewModel searchModel = new SearchViewModel();
        searchModel.Users = _context.ApplicationUsers.ToList();
        searchModel.Posts = _context.PostModel.ToList();
        foreach (PostModel post in searchModel.Posts)
        {
            PopulatePost(post);
        }
        if (String.IsNullOrEmpty(key))
        {
            return View("Search/UserSearch", searchModel);
        }

        TempData["key"] = key;

        //filters
        searchModel.Users = searchModel.Users.Where(c => c.Uname.ToLower().Contains(key.ToLower())).ToList();
        searchModel.Posts = searchModel.Posts.Where(p => p.Caption.ToLower().Contains(key.ToLower())).ToList();


        //returns searched
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
        foreach (var post in posts)
        {
            post.TimeAgo = GetTimeSince(post.PostDate);
        }
        var allComments = _context.CommentModel.ToList();
        var allLikes = _context.LikeModel.ToList();
    }

    private void PopulatePost(PostModel postModel)
    {
        postModel.TimeAgo = GetTimeSince(postModel.PostDate);

        postModel.Comments = _context.CommentModel.Where(c => c.PostId.Equals(postModel.PostId)).OrderByDescending(c => c.PostDate).ToList();

        foreach (var commentModel in postModel.Comments)
        {
            commentModel.TimeAgo = GetTimeSince(commentModel.PostDate);
        }
        //comment count
        if (postModel.Comments != null)
        {
            postModel.CommentCount = postModel.Comments.Count;
        }

        postModel.LikedBy = _context.LikeModel.Where(l => l.PostId.Equals(postModel.PostId)).ToList();
    }

    //Update Posts and Comments pfp/Username when Profile is edited
    private void UpdateUserFootprint(Guid userId, string userName, string profilePicture)
    {
        var posts = _context.PostModel.Where(p => p.UserId.Equals(userId));
        var comments = _context.CommentModel.Where(c => c.UserId.Equals(userId));

        foreach (var post in posts)
        {
            post.UserName = userName;
            post.ProfilePicture = profilePicture;
            _context.PostModel.Update(post);
        }

        foreach (var comment in comments)
        {
            comment.Username = userName;
            _context.CommentModel.Update(comment);
        }

        TempData["AlertMessage"] = "Profile Successfully updated";
    }

    //From: https://www.thatsoftwaredude.com/content/1019/how-to-calculate-time-ago-in-c
    //Convert's Posts date into something more readable for the User
    public static string GetTimeSince(DateTime objDateTime)
    {
        // here we are going to subtract the passed in DateTime from the current time converted to UTC
        TimeSpan ts = DateTime.Now.Subtract(objDateTime);
        int intDays = ts.Days;
        int intHours = ts.Hours;
        int intMinutes = ts.Minutes;
        int intSeconds = ts.Seconds;

        if (intDays > 0)
            return string.Format("{0} days", intDays);

        if (intHours > 0)
            return string.Format("{0} hours", intHours);

        if (intMinutes > 0)
            return string.Format("{0} minutes", intMinutes);

        if (intSeconds > 0)
            return string.Format("{0} seconds", intSeconds);

        // let's handle future times..just in case
        if (intDays < 0)
            return string.Format("in {0} days", Math.Abs(intDays));

        if (intHours < 0)
            return string.Format("in {0} hours", Math.Abs(intHours));

        if (intMinutes < 0)
            return string.Format("in {0} minutes", Math.Abs(intMinutes));

        if (intSeconds < 0)
            return string.Format("in {0} seconds", Math.Abs(intSeconds));

        return "moments";
    }


    #endregion

}