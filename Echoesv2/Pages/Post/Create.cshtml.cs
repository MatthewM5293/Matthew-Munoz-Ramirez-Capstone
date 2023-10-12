using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Echoesv2.Data;
using Echoesv2.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Echoesv2.Pages.Post
{
    public class CreateModel : PageModel
    {
        private readonly Echoesv2.Data.ApplicationDbContext _context;

        public CreateModel(Echoesv2.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public PostModel PostModel { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var temp = User.FindFirstValue(ClaimTypes.NameIdentifier);
            PostModel.UserId = new Guid(temp);

            if (!ModelState.IsValid || _context.PostModel == null || PostModel == null)
            {
                return Page();
            }

            _context.PostModel.Add(PostModel);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
