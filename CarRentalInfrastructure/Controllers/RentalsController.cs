using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarRentalDomain.Model;
using CarRentalInfrasructure;
using CarRentalInfrastructure.Services;

namespace CarRentalInfrastructure.Controllers
{
    public class RentalsController : Controller
    {
        private readonly CarRentalDbContext _context;
        private readonly TelegramBotService _telegramBotService;

        public RentalsController(CarRentalDbContext context, TelegramBotService telegramBotService)
        {
            _context = context;
            _telegramBotService = telegramBotService;
        }


        // GET: Rentals
        public async Task<IActionResult> Index()
        {
            var carRentalDbContext = _context.Rentals.Include(r => r.Car).Include(r => r.Customer);
            return View(await carRentalDbContext.ToListAsync());
        }

        // GET: Rentals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // GET: Rentals/Create
        public IActionResult Create()
        {
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "LicensePlate");
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName");
            return View();
        }

        // POST: Rentals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CarId,RentalDate,ReturnDate,Notes")] Rental rental) // ⚠️ Id видалено
        {
            if (ModelState.IsValid)
            {
                _context.Add(rental);
                await _context.SaveChangesAsync();
              
                try
                {
                    var car = await _context.Cars.FindAsync(rental.CarId);
                    var customer = await _context.Customers.FindAsync(rental.CustomerId);

                    var message = $"🆕 Нова оренда!\n" +
                                  $"Авто: {car?.Make} {car?.Model} ({car?.LicensePlate})\n" +
                                  $"Клієнт: {customer?.FullName}\n" +
                                  $"Телефон: {customer?.PhoneNumber}\n" +
                                  $"Оренда: {rental.RentalDate:dd.MM.yyyy HH:mm}\n" +
                                  $"Повернення: {rental.ReturnDate?.ToString("dd.MM.yyyy HH:mm") ?? "не вказано"}";

                    await _telegramBotService.SendMessageAsync("938409337", message);
                }
                catch (Exception ex)
                {
                 
                    Console.WriteLine($"Помилка Telegram: {ex.Message}");
                }

                return RedirectToAction(nameof(Index));
            }

          
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "LicensePlate", rental.CarId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName", rental.CustomerId);
            return View(rental);
        }

        // GET: Rentals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "LicensePlate", rental.CarId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName", rental.CustomerId);
            return View(rental);
        }

        // POST: Rentals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CarId,RentalDate,ReturnDate,Notes,Id")] Rental rental)
        {
            if (id != rental.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rental);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RentalExists(rental.Id))
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
            ViewData["CarId"] = new SelectList(_context.Cars, "Id", "LicensePlate", rental.CarId);
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "FullName", rental.CustomerId);
            return View(rental);
        }

        // GET: Rentals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rental = await _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rental == null)
            {
                return NotFound();
            }

            return View(rental);
        }

        // POST: Rentals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental != null)
            {
                _context.Rentals.Remove(rental);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RentalExists(int id)
        {
            return _context.Rentals.Any(e => e.Id == id);
        }
    }
}
