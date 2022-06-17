using Exam2.DAL;
using Exam2.Models;
using Exam2.Utilies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Exam2.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private AppDbContext _context;
        private IWebHostEnvironment _env;

        public ProductController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Products.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            if (product.Photo.CheckSize(800))
            {
                ModelState.AddModelError("Photo", "File size must be less than 800kb");
                return View();
            }
            if (!product.Photo.CheckType("image/"))
            {
                ModelState.AddModelError("Photo", "File must be image");
                return View();
            }
            product.Img = await product.Photo.SaveFileAsync(Path.Combine(_env.WebRootPath, "assets", "img"));
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            Product product = _context.Products.Find(id);
            if (product == null) return NotFound();
            if (System.IO.File.Exists(Path.Combine(_env.WebRootPath, "assets", "img", product.Img)))
            {
                System.IO.File.Delete(Path.Combine(_env.WebRootPath, "assets", "img", product.Img));
            }
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            Product dinner = _context.Products.FirstOrDefault(c => c.Id == id);
            if (dinner == null) return NotFound();
            return View(dinner);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (ModelState.IsValid)
            {
                var p = await _context.Products.FindAsync(product.Id);
                p.Name = product.Name;
                p.Desc = product.Desc;
                p.CategoryId = product.CategoryId;
                if (product.Photo != null)
                {
                    if (product.Img != null)
                    {
                        string filePath = Path.Combine(_env.WebRootPath, "assets", "img", product.Img);
                        System.IO.File.Delete(filePath);
                    }
                    p.Img = ProcessUploadedFile(product);
                }
                _context.Update(p);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private string ProcessUploadedFile(Product product)
        {
            string uniqueFileName = null;

            if (product.Photo != null)
            {
                string uploadsFolder = Path.Combine(_env.WebRootPath, "assets", "img");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + product.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    product.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
