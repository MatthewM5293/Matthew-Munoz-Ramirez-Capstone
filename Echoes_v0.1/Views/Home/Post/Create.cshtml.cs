﻿# 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Echoes_v0._1.Data;
using Echoes_v0._1.Models;

namespace Echoes_v0._1.Views.Home.Post
{
    public class CreateModel : PageModel
    {
        private readonly Echoes_v0._1.Data.ApplicationDbContext _context;

        public CreateModel(Echoes_v0._1.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public CreateModel()
        {
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public PostModel PostModel { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public  IActionResult OnPost()
        {   
            PostModel.PostDate = DateTime.Now;
            if (!ModelState.IsValid || _context.PostModel == null || PostModel == null)
            {
                return Page();
            }

            _context.PostModel.Add(PostModel);
            _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
