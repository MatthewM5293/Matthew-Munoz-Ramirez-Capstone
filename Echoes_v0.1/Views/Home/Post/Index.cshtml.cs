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
    public class IndexModel : PageModel
    {
        private readonly Echoes_v0._1.Data.ApplicationDbContext _context;

        public IndexModel(Echoes_v0._1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<PostModel> PostModel { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.PostModel != null)
            {
                PostModel = await _context.PostModel.ToListAsync();
            }
        }
    }
}
