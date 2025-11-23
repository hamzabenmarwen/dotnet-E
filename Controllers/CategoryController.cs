using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP2.Models;
using TP2.Models.Repositories;

namespace TP2.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class CategoryController : Controller
    {
        private readonly ICategorieRepository CategRepository;

        public CategoryController(ICategorieRepository categRepository)
        {
            CategRepository = categRepository;
        }

        // GET: Category
        [AllowAnonymous]
        public IActionResult Index()
        {
            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            return View(categories);
        }

        // GET: Category/Details/5
        public IActionResult Details(int id)
        {
            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            var category = CategRepository.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            // Remove CategoryId from ModelState if it's auto-generated
            ModelState.Remove("CategoryId");

            if (ModelState.IsValid)
            {
                CategRepository.Add(category);
                return RedirectToAction(nameof(Index));
            }

            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            return View(category);
        }

        // GET: Category/Edit/5
        public IActionResult Edit(int id)
        {
            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            var category = CategRepository.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                CategRepository.Update(category);
                return RedirectToAction(nameof(Index));
            }

            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            return View(category);
        }

        // GET: Category/Delete/5
        public IActionResult Delete(int id)
        {
            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            var category = CategRepository.GetById(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            CategRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }
    }
}