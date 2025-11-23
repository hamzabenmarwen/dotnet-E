using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using TP2.Models;
using TP2.Models.Repositories;
using TP2.ViewModels;

namespace TP2.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProductController : Controller
    {
        private readonly IProductRepository ProductRepository;
        private readonly ICategorieRepository CategRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ProductController(IProductRepository prodRepository,
                                 ICategorieRepository categRepository,
                                 IWebHostEnvironment hostingEnvironment)
        {
            ProductRepository = prodRepository;
            CategRepository = categRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Product
        [AllowAnonymous]
        public IActionResult Index(int? categoryId, int page = 1)
        {
            int pageSize = 4; // Nombre de produits par page

            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            // Récupérer les produits en fonction de categoryId, s'il est spécifié
            IQueryable<Product> productsQuery = ProductRepository.GetAllProducts();
            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }

            // Pagination
            var totalProducts = productsQuery.Count();
            var products = productsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalProducts / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.CategoryId = categoryId; // Passer categoryId à la vue

            return View(products);
        }

        // GET: Product/Details/5
        public IActionResult Details(int id)
        {
            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            var product = ProductRepository.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(),
                                                "CategoryId", "CategoryName");
            return View();
        }

        // POST: Product/Create (with image upload)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model.ImagePath);

                Product newProduct = new Product
                {
                    Name = model.Name,
                    Price = model.Price,
                    QteStock = model.QteStock,
                    CategoryId = model.CategoryId,
                    Image = uniqueFileName
                };

                ProductRepository.Add(newProduct);
                return RedirectToAction("Details", new { id = newProduct.ProductId });
            }

            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(),
                                                "CategoryId", "CategoryName");
            return View(model);
        }

        // GET: Product/Edit/5
        public IActionResult Edit(int id)
        {
            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            var product = ProductRepository.GetById(id);
            if (product == null) return NotFound();

            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(),
                                                "CategoryId", "CategoryName");

            var editModel = new EditViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Price = product.Price,
                QteStock = product.QteStock,
                CategoryId = product.CategoryId,
                ExistingImagePath = product.Image
            };

            return View(editModel);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditViewModel model)
        {
            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            ViewBag.CategoryId = new SelectList(CategRepository.GetAll(),
                                                "CategoryId", "CategoryName");

            if (ModelState.IsValid)
            {
                var product = ProductRepository.GetById(model.ProductId);
                if (product == null) return NotFound();

                product.Name = model.Name;
                product.Price = model.Price;
                product.QteStock = model.QteStock;
                product.CategoryId = model.CategoryId;

                if (model.ImagePath != null)
                {
                    // delete old file if it exists
                    if (!string.IsNullOrEmpty(model.ExistingImagePath))
                    {
                        string oldFile = Path.Combine(hostingEnvironment.WebRootPath,
                                                      "images", model.ExistingImagePath);
                        if (System.IO.File.Exists(oldFile))
                            System.IO.File.Delete(oldFile);
                    }

                    product.Image = ProcessUploadedFile(model.ImagePath);
                }

                ProductRepository.Update(product);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Product/Delete/5
        public IActionResult Delete(int id)
        {
            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            var product = ProductRepository.GetById(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = ProductRepository.GetById(id);
            if (product != null && !string.IsNullOrEmpty(product.Image))
            {
                string file = Path.Combine(hostingEnvironment.WebRootPath,
                                           "images", product.Image);
                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);
            }

            ProductRepository.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        // 🔹 helper to save uploaded files
        [NonAction]
        private string ProcessUploadedFile(IFormFile file)
        {
            if (file == null) return null;

            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
            Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            return uniqueFileName;
        }

        // Search action
        public ActionResult Search(string val)
        {
            // Load categories for sidebar
            var categories = CategRepository.GetAll();
            ViewData["Categories"] = categories;

            var result = ProductRepository.FindByName(val);
            return View("Index", result);
        }
    }
}