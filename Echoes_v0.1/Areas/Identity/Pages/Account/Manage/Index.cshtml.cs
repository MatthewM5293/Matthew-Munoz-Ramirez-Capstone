// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Echoes_v0._1.Data;
using Echoes_v0._1.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Echoes_v0._1.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        //private readonly UserManager<ApplicationUser> _appUserManager; //Doesn't work as intended
        private readonly SignInManager<IdentityUser> _signInManager;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            //edit name
            //edit Username
            //edit Bio
            //edit Profile Picture
            [Required]
            [StringLength(30, ErrorMessage = "Username must be between 2-16 characters", MinimumLength = 1)]
            [DataType(DataType.Text), RegularExpression(@"^[A-Za-z0-9_.]*$")]
            [Display(Name = "Username")]
            public string Uname { get; set; }


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

        private async Task LoadAsync(IdentityUser user)
        {
            //var userName = await _userManager.GetUserNameAsync(user); //email
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            var userName = user.Email;
            //var phoneNumber = user.PhoneNumber;

            //Need a way to get Application User instead of Identity User
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var temp = Globals.dal.GetUser(id); //gets App User (hopefuly)
            var uname = temp.Uname ?? string.Empty;
            var name = temp.Name ?? string.Empty;
            var bio = temp.Bio ?? string.Empty;
            var profiePicture = temp.ProfilePicture ?? string.Empty;
            var dob = temp.DateOfBirth.Value;

            //email
            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Uname = uname,
                Name = name,
                Bio = bio,
                DOB = dob,
                ProfilePicture = profiePicture
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
