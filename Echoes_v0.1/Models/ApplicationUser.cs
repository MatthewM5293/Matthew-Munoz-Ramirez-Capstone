using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Security.Claims;

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

public class ApplicationUserManager : UserManager<ApplicationUser>
{
    public ApplicationUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor, 
        IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators, 
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer, 
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<ApplicationUser>> logger)
        : base
        (
            store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, 
            services, logger)
    {

    }

    public virtual string? GetApplicationUserName(ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }
        return principal.FindFirstValue(Options.ClaimsIdentity.UserNameClaimType);
    }

    public virtual string? GetUserProfilePicture(ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }
        return principal.FindFirstValue(Options.ClaimsIdentity.UserNameClaimType); //return Profile picture
    }

    public virtual string? GetUserName(ClaimsPrincipal principal)
    {
        if (principal == null)
        {
            throw new ArgumentNullException(nameof(principal));
        }
        return principal.FindFirstValue(Options.ClaimsIdentity.UserNameClaimType); //return Profile picture
    }


}