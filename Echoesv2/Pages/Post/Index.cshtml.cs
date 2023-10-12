using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Echoesv2.Data;
using Echoesv2.Models;

namespace Echoesv2.Pages.Post
{
    public class IndexModel : PageModel
    {
        private readonly Echoesv2.Data.ApplicationDbContext _context;

        public IndexModel(Echoesv2.Data.ApplicationDbContext context)
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
