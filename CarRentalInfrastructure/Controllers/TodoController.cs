using Microsoft.AspNetCore.Mvc;

namespace CarRental.Controllers
{
    public class TodoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
