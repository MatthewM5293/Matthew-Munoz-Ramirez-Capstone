using Microsoft.AspNetCore.Identity;

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

    public string ProfilePicture { get; set; } = "https://cdn-icons-png.flaticon.com/512/3177/3177440.png";
    public string Username { get; set; }
    
    public string Name { get; set; }
    
    public string Bio { get; set; } = "";
    
    public DateTime? DateOfBirth { get; set; }
    
    //Posts
    
    public ApplicationUser()
    {
    }
}