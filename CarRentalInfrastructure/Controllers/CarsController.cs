using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRentalDomain.Model;
using CarRentalInfrastructure;
using CarRentalInfrasructure;

namespace CarRentalInfrastructure.Controllers
{
    public class CarsController : Controller
    {
        private readonly CarRentalDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CarsController(CarRentalDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

       
        // GET: Cars
        // GET: Cars?categoryId=1&categoryName=Економ
        public async Task<IActionResult> Index(int? categoryId, string? categoryName)
        {
            if (categoryId.HasValue)
            {
               
                ViewBag.CategoryId = categoryId;
                ViewBag.CategoryName = categoryName;

                var cars = await _context.Cars
                    .Where(c => c.CategoryId == categoryId)
                    .Include(c => c.Category)
                    .ToListAsync();

                return View(cars);
            }
            else
            {
               
                var allCars = await _context.Cars.Include(c => c.Category).ToListAsync();
                return View(allCars);
            }
        }
        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Make,Model,Year,LicensePlate,CategoryId")] Car car, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
               
                if (Photo != null && Photo.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "cars");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Photo.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Photo.CopyToAsync(stream);
                    }

                    car.PhotoPath = "/images/cars/" + fileName;
                }

                _context.Add(car);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

           
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", car.CategoryId);
            return View(car);
        }

        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", car.CategoryId);
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Make,Model,Year,LicensePlate,PhotoPath,CategoryId")] Car car, IFormFile? Photo)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    if (Photo != null && Photo.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "cars");
                        Directory.CreateDirectory(uploadsFolder);

                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(Photo.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Photo.CopyToAsync(stream);
                        }

                        car.PhotoPath = "/images/cars/" + fileName;
                    }

                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", car.CategoryId);
            return View(car);
        }

        // GET: Cars/Landing/5
        public async Task<IActionResult> Landing(int? id)
        {
            if (id == null) return NotFound();

            var car = await _context.Cars
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null) return NotFound();

            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car != null)
            {
                _context.Cars.Remove(car);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }
    }
}