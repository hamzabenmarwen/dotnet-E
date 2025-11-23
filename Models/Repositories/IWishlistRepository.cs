using TP2.Models;

namespace TP2.Models.Repositories
{
    public interface IWishlistRepository
    {
        Wishlist GetWishlistByUserId(string userId);
        void AddToWishlist(string userId, int productId);
        void RemoveFromWishlist(string userId, int productId);
        bool IsInWishlist(string userId, int productId);
        int GetWishlistItemCount(string userId);
    }
}