using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP2.Models;
using System.Linq;
using System.Threading.Tasks;

public class WishlistController : Controller
{
    private readonly AppDbContext _context;

    public WishlistController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<JsonResult> ToggleWishlist([FromBody] WishlistRequest request)
    {
        try
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == request.ProductId);

            if (product == null)
            {
                return Json(new { success = false, message = "Produit non trouvé" });
            }

            // Toggle wishlist status
            product.IsInWishlist = !product.IsInWishlist;
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                isInWishlist = product.IsInWishlist,
                message = product.IsInWishlist ? "Ajouté à la liste de souhaits" : "Retiré de la liste de souhaits"
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public async Task<IActionResult> Index()
    {
        var wishlistProducts = await _context.Products
            .Where(p => p.IsInWishlist)
            .Include(p => p.Category)
            .ToListAsync();

        return View(wishlistProducts);
    }
}

public class WishlistRequest
{
    public int ProductId { get; set; }
}