using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Echoes_v0._1.Models;

public class ApplicationUser : IdentityUser
{
    /*
     * UserID (may not be needed)
     * Profile Picture 
     * Username
     * First Name
     * Last Name
     * Bio
     *
     * Posts (filtered using UserID)
     */

    //Will be image later


    [Required]
    [DataType(DataType.ImageUrl)]
    [Display(Name = "Profile Picture")]
    public string ProfilePicture { get; set; } = "https://cdn-icons-png.flaticon.com/512/3177/3177440.png";
    //public Image ProfilePicture { get; set; } = Image.FromFile("Resources\\3177440.png");

    [Required]
    [StringLength(30, ErrorMessage = "Username must be between 2-16 characters", MinimumLength = 1)]
    [DataType(DataType.Text), RegularExpression(@"^[A-Za-z0-9_.]*$")]
    [Display(Name = "Username")]
    public string Uname { get; set; } //Username for Application (Will be Unique)


    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Name")]
    [RegularExpression(@"^[A-Za-z0-9@~`!@#$%^&*()_=+\\\\';:\""\\/?>.<, -]*$")]
    public string Name { get; set; }


    [Display(Name = "Bio")]
    public string? Bio { get; set; } = "";

    [DataType(DataType.Date)]
    [Display(Name = "Birthday")]
    public DateTime? DateOfBirth { get; set; }

    [NotMapped]
    [DataType(DataType.Upload)]
    public IFormFile? Image { get; set; }

    //Posts
    public List<PostModel>? Posts { get; set; }

    //Followers
    public List<UserFollowModel>? Followers { get; set; }

    [Display(Name = "Followers")]
    public int FollowersCount { get; set; } = 0;
    [Display(Name = "Following")]
    public int FollowingCount { get; set; } = 0;
    [Display(Name = "Posts")]
    public int PostCount { get; set; } = 0;

    //Who the User is Following
    public List<UserFollowModel>? Following { get; set; }

    //Join Date
    [Display(Name = "Joined on: ")]
    public DateTime JoinDate { get; set; } = DateTime.Now;

    public ApplicationUser()
    {
    }
}