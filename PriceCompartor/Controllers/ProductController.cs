﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PriceCompartor.Data;
using PriceCompartor.Models;

namespace PriceCompartor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Platforms);
            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LoadCategoriesAndPlatforms();
            return View();
        }

        [NonAction]
        private void LoadCategoriesAndPlatforms()
        {
            var categories = _context.Categories.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            var platforms = _context.Platforms.ToList();
            ViewBag.Platforms = new SelectList(platforms, "Id", "Name");
        }

        [NonAction]
        private void UnloadCategoriesAndPlatforms()
        {
            ModelState.Remove("Categories");
            ModelState.Remove("Platforms");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product model)
        {
            if (ModelState.IsValid)
            {
                UnloadCategoriesAndPlatforms();
                _context.Products.Add(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            LoadCategoriesAndPlatforms();
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LoadCategoriesAndPlatforms();
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                UnloadCategoriesAndPlatforms();
                _context.Products.Update(model);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            LoadCategoriesAndPlatforms();
            return View();
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            LoadCategoriesAndPlatforms();
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Product model)
        {
            ModelState.Remove("Categories");
            _context.Products.Remove(model);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Purge()
        {
            return View();
        }

        [HttpPost, ActionName("Purge")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurgeConfirmed()
        {
            var allProducts = await _context.Products.ToListAsync();
            _context.Products.RemoveRange(allProducts);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
