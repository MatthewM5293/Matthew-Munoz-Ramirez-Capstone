using Microsoft.AspNetCore.Identity;
using System.Drawing;

namespace Echoesv2.Models;

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
    public string ProfilePicture { get; set; } = "https://cdn-icons-png.flaticon.com/512/3177/3177440.png";
    //public Image ProfilePicture { get; set; } = Image.FromFile("Resources\\3177440.png");
    public string Uname { get; set; } //Username for Application
    
    public string Name { get; set; }
    
    public string Bio { get; set; } = "";
    
    public DateTime? DateOfBirth { get; set; }
    
    //Posts

    //Tag Object
    
    public ApplicationUser()
    {
    }
}