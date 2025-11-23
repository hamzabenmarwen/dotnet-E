using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP2.Models;
using TP2.Models.ViewModels;

namespace TP2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var homeVM = new HomeViewModel
            {
                Categories = _context.Categories
                                     .OrderBy(c => c.CategoryName)
                                     .Take(6)
                                     .ToList(),

                Products = _context.Products
                                   .Include(p => p.Category)
                                   .OrderByDescending(p => p.ProductId)
                                   .Take(6)
                                   .ToList()
            };

            return View(homeVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
