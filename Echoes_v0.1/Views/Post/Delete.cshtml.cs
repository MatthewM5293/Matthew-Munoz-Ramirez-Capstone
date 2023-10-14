using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Echoes_v0._1.Data;
using Echoes_v0._1.Models;

namespace Echoes_v0._1.Views.Post
{
    public class DeleteModel : PageModel
    {
        private readonly Echoes_v0._1.Data.ApplicationDbContext _context;

        public DeleteModel(Echoes_v0._1.Data.ApplicationDbContext context)
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

            var postmodel = await _context.PostModel.FirstOrDefaultAsync(m => m.PostId == id);

            if (postmodel == null)
            {
                return NotFound();
            }
            else 
            {
                PostModel = postmodel;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null || _context.PostModel == null)
            {
                return NotFound();
            }
            var postmodel = await _context.PostModel.FindAsync(id);

            if (postmodel != null)
            {
                PostModel = postmodel;
                _context.PostModel.Remove(PostModel);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
