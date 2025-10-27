using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CarRentalInfrastructure; 
using CarRentalDomain.Model;
using CarRentalInfrasructure;
using Microsoft.EntityFrameworkCore;
namespace CarRentalInfrastructure.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChartsController : ControllerBase
{
    
    private record CarsByCategoryResponseItem(string CategoryName, int Count);
    private record RentalsByMonthResponseItem(string Month, int Count);

    private readonly CarRentalDbContext _context;

    public ChartsController(CarRentalDbContext context)
    {
        _context = context;
    }

    // GET: /api/charts/carsByCategory
    [HttpGet("carsByCategory")]
    public async Task<IActionResult> GetCarsByCategoryAsync(CancellationToken ct = default)
    {
        var data = await _context.Cars
            .Where(c => c.Category != null)
            .GroupBy(c => c.Category.Name)
            .Select(g => new CarsByCategoryResponseItem(g.Key, g.Count()))
            .ToListAsync(ct);

        return Ok(data);
    }

    // GET: /api/charts/rentalsByMonth
    [HttpGet("rentalsByMonth")]
    public async Task<IActionResult> GetRentalsByMonthAsync(CancellationToken ct = default)
    {
        var sixMonthsAgo = DateTime.Now.AddMonths(-6);

       
        var rawData = await _context.Rentals
            .Where(r => r.RentalDate >= sixMonthsAgo)
            .Select(r => new { Year = r.RentalDate.Year, Month = r.RentalDate.Month })
            .ToListAsync(ct);

       
        var grouped = rawData
            .GroupBy(x => $"{x.Year}-{x.Month:D2}")
            .Select(g => new RentalsByMonthResponseItem(g.Key, g.Count()))
            .OrderBy(x => x.Month)
            .ToList();

        return Ok(grouped);
    }
}