using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Echoes_v0._1.Data;
using Echoes_v0._1.Models;

namespace Echoes_v0._1.Views.Post
{
    public class EditModel : PageModel
    {
        private readonly Echoes_v0._1.Data.ApplicationDbContext _context;

        public EditModel(Echoes_v0._1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public PostModel PostModel { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null || _context.PostModel == null)
            {
                return NotFound();
            }

            var postmodel =  await _context.PostModel.FirstOrDefaultAsync(m => m.PostId == id);
            if (postmodel == null)
            {
                return NotFound();
            }
            PostModel = postmodel;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(PostModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostModelExists(PostModel.PostId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool PostModelExists(Guid id)
        {
          return (_context.PostModel?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}
