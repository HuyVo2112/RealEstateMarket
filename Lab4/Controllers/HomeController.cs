using Microsoft.AspNetCore.Mvc;
using Assignment2.Data;

namespace Assignment2.Controllers
{
    public class HomeController : Controller
    {

        private readonly MarketDbContext _context;

        public HomeController(MarketDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(); 
        }

    }
}
