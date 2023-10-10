#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Echoes_v0._1.Models;
using System;
using System.Text.Encodings.Web;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Echoes_v0._1.Views.Home
{
    public class EditProfileModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public EditProfileModel(
            SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public EditProfileModel(SignInManager<IdentityUser> signInManager, string statusMessage, InputModel input) : this(signInManager)
        {
            StatusMessage = statusMessage;
            Input = input;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(30, ErrorMessage = "Username must be between 2-16 characters", MinimumLength = 1)]
            [DataType(DataType.Text), RegularExpression(@"^[A-Za-z0-9_.]*$")]
            [Display(Name = "Username")]
            public string Username { get; set; }


            [Required]
            [DataType(DataType.Text)]
            [Display(Name = "Name")]
            [RegularExpression(@"^[A-Za-z0-9@~`!@#$%^&*()_=+\\\\';:\""\\/?>.<, -]*$")]
            public string Name { get; set; }

            [Display(Name = "Bio")]
            public string Bio { get; set; }

            [Required]
            [DataType(DataType.ImageUrl)]
            [Display(Name = "Profile Picture")]
            public string ProfilePicture { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Birthday")]
            public DateTime DOB { get; set; }
        }

        //When page loads, show info from DB
        private async Task LoadAsync(ApplicationUser user)
        {
            //load db info into forms
            Input = new InputModel
            {
                //load info into the fields
                Username = user.UserName,
                Name = user.UserName,
                Bio = user.Bio,
                ProfilePicture = user.ProfilePicture,
                DOB = user.DateOfBirth.Value
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = Globals.dal.GetUser(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{User.FindFirstValue(ClaimTypes.NameIdentifier)}'.");
            }

            await LoadAsync(user);

            return Page(); //Loads page
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = Globals.dal.GetUser(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                NotFound($"Unable to load user with ID '{User.FindFirstValue(ClaimTypes.NameIdentifier)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var bio = user.Bio;
            var profilePicture = user.ProfilePicture;
            var name = user.Name;
            var username = user.UserName;
            var dob = user.DateOfBirth.Value;
            if (Input.Bio != bio)
            {
                //var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                //if (!setPhoneResult.Succeeded)
                //{
                //    StatusMessage = "Unexpected error when trying to set phone number.";
                //    return RedirectToPage();
                //}
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

    }

}
