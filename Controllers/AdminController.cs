using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP2.Models;
using TP2.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace TP2.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _context;

        public AdminController(RoleManager<IdentityRole> roleManager,
                             UserManager<IdentityUser> userManager,
                             AppDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        // GET: AdminController/Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var dashboardData = new AdminDashboardViewModel
            {
                // Basic statistics
                TotalProducts = await _context.Products.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync(),
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),

                // Sales statistics
                TotalRevenue = await GetTotalRevenue(),
                MonthlyRevenue = await GetMonthlyRevenue(),
                AverageOrderValue = await GetAverageOrderValue(),

                // Product statistics
                LowStockProducts = await _context.Products.Where(p => p.QteStock < 10 && p.QteStock > 0).CountAsync(),
                OutOfStockProducts = await _context.Products.Where(p => p.QteStock == 0).CountAsync(),

                // Chart data
                SalesData = await GetSalesData(),
                CategoryDistribution = await GetCategoryDistribution(),
                TopProducts = await GetTopProducts(),
                MonthlyGrowth = await GetMonthlyGrowth(),

                // User roles statistics
                UserRoles = await GetUserRolesStatistics()
            };

            return View(dashboardData);
        }

        private async Task<decimal> GetTotalRevenue()
        {
            var orders = await _context.Orders.ToListAsync();
            return orders.Any() ? (decimal)orders.Sum(o => o.TotalAmount) : 0m;
        }

        private async Task<decimal> GetMonthlyRevenue()
        {
            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startOfMonth)
                .ToListAsync();

            return orders.Any() ? (decimal)orders.Sum(o => o.TotalAmount) : 0m;
        }

        private async Task<decimal> GetAverageOrderValue()
        {
            var orders = await _context.Orders.ToListAsync();
            if (!orders.Any())
                return 0m;

            return (decimal)orders.Average(o => o.TotalAmount);
        }

        private async Task<List<SalesData>> GetSalesData()
        {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-6);

            var orders = await _context.Orders
                .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
                .ToListAsync();

            var salesData = new List<SalesData>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dailyOrders = orders.Where(o => o.OrderDate.Date == date.Date);
                var dailySales = dailyOrders.Any() ? (decimal)dailyOrders.Sum(o => o.TotalAmount) : 0m;

                salesData.Add(new SalesData
                {
                    Date = date,
                    Amount = dailySales
                });
            }

            return salesData;
        }

        private async Task<List<CategoryData>> GetCategoryDistribution()
        {
            return await _context.Categories
                .Include(c => c.Products)
                .Select(c => new CategoryData
                {
                    CategoryName = c.CategoryName,
                    ProductCount = c.Products.Count,
                    TotalValue = c.Products.Sum(p => (decimal)p.Price * p.QteStock)
                })
                .ToListAsync();
        }

        private async Task<List<TopProduct>> GetTopProducts()
        {
            // Get products from order items to find actual top-selling products
            var orderItems = await _context.OrderItems.ToListAsync();

            if (orderItems.Any())
            {
                var topProducts = orderItems
                    .GroupBy(oi => oi.ProductName)
                    .Select(g => new TopProduct
                    {
                        Name = g.Key,
                        Sales = g.Sum(oi => oi.Quantity),
                        Revenue = (decimal)g.Sum(oi => oi.Quantity * oi.Price)
                    })
                    .OrderByDescending(p => p.Revenue)
                    .Take(5)
                    .ToList();

                return topProducts;
            }

            // If no orders exist yet, show top products by stock value
            var fallbackProducts = await _context.Products
                .OrderByDescending(p => (decimal)p.Price * p.QteStock)
                .Take(5)
                .Select(p => new TopProduct
                {
                    Name = p.Name,
                    Sales = p.QteStock,
                    Revenue = (decimal)p.Price * p.QteStock
                })
                .ToListAsync();

            return fallbackProducts;
        }

        private async Task<List<GrowthData>> GetMonthlyGrowth()
        {
            var growthData = new List<GrowthData>();

            for (int i = 5; i >= 0; i--)
            {
                var monthDate = DateTime.Now.AddMonths(-i);
                var startOfMonth = new DateTime(monthDate.Year, monthDate.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                var monthlyOrders = await _context.Orders
                    .Where(o => o.OrderDate >= startOfMonth && o.OrderDate <= endOfMonth)
                    .ToListAsync();

                var monthlyRevenue = monthlyOrders.Any() ? (decimal)monthlyOrders.Sum(o => o.TotalAmount) : 0m;

                // Calculate growth percentage
                decimal growth = 0;
                if (i < 5)
                {
                    var previousMonth = DateTime.Now.AddMonths(-(i + 1));
                    var prevStartOfMonth = new DateTime(previousMonth.Year, previousMonth.Month, 1);
                    var prevEndOfMonth = prevStartOfMonth.AddMonths(1).AddDays(-1);

                    var previousMonthOrders = await _context.Orders
                        .Where(o => o.OrderDate >= prevStartOfMonth && o.OrderDate <= prevEndOfMonth)
                        .ToListAsync();

                    var previousRevenue = previousMonthOrders.Any() ? (decimal)previousMonthOrders.Sum(o => o.TotalAmount) : 0m;

                    if (previousRevenue > 0)
                    {
                        growth = ((monthlyRevenue - previousRevenue) / previousRevenue) * 100;
                    }
                }

                growthData.Add(new GrowthData
                {
                    Month = startOfMonth.ToString("MMM yyyy"),
                    Revenue = monthlyRevenue,
                    Growth = Math.Round(growth, 2)
                });
            }

            return growthData;
        }

        private async Task<Dictionary<string, int>> GetUserRolesStatistics()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = new Dictionary<string, int>();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                userRoles.Add(role.Name, usersInRole.Count);
            }

            return userRoles;
        }

        // GET: AdminController/CreateRole
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        // POST: AdminController/CreateRole
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole { Name = model.RoleName };
                IdentityResult result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: AdminController/ListRoles
        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        // GET: AdminController/EditRole
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            foreach (var user in _userManager.Users.ToList())
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        }

        // POST: AdminController/EditRole
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        // POST: AdminController/DeleteRole
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListRoles");
            }
        }

        // GET: AdminController/EditUsersInRole
        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            foreach (var user in _userManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        // POST: AdminController/EditUsersInRole
        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);
                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });
                }
            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }
    }
}