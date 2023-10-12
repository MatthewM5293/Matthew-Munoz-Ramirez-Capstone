using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Echoes_v0._1.Data;
using Echoes_v0._1.Models;

namespace Echoes_v0._1.Views.Home.Post
{
    public class DetailsModel : PageModel
    {
        private readonly Echoes_v0._1.Data.ApplicationDbContext _context;

        public DetailsModel(Echoes_v0._1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
