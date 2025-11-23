using Microsoft.EntityFrameworkCore;

namespace TP2.Models.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AppDbContext _context;

        public WishlistRepository(AppDbContext context)
        {
            _context = context;
        }

        public Wishlist GetWishlistByUserId(string userId)
        {
            return _context.Wishlists
                .Include(w => w.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefault(w => w.UserId == userId);
        }

        public void AddToWishlist(string userId, int productId)
        {
            var wishlist = GetWishlistByUserId(userId) ?? new Wishlist { UserId = userId };

            if (wishlist.Id == 0)
            {
                _context.Wishlists.Add(wishlist);
                _context.SaveChanges();
            }

            // Check if product is already in wishlist
            if (!wishlist.Items.Any(i => i.ProductId == productId))
            {
                var wishlistItem = new WishlistItem
                {
                    WishlistId = wishlist.Id,
                    ProductId = productId,
                    AddedDate = DateTime.Now
                };

                _context.WishlistItems.Add(wishlistItem);
                wishlist.UpdatedDate = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public void RemoveFromWishlist(string userId, int productId)
        {
            var wishlist = GetWishlistByUserId(userId);
            if (wishlist != null)
            {
                var itemToRemove = wishlist.Items.FirstOrDefault(i => i.ProductId == productId);
                if (itemToRemove != null)
                {
                    _context.WishlistItems.Remove(itemToRemove);
                    wishlist.UpdatedDate = DateTime.Now;
                    _context.SaveChanges();
                }
            }
        }

        public bool IsInWishlist(string userId, int productId)
        {
            var wishlist = GetWishlistByUserId(userId);
            return wishlist?.Items.Any(i => i.ProductId == productId) ?? false;
        }

        public int GetWishlistItemCount(string userId)
        {
            var wishlist = GetWishlistByUserId(userId);
            return wishlist?.Items.Count ?? 0;
        }
    }
}