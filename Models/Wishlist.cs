using System.ComponentModel.DataAnnotations;

namespace TP2.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public List<WishlistItem> Items { get; set; } = new List<WishlistItem>();
    }

    public class WishlistItem
    {
        public int Id { get; set; }

        public int WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.Now;
    }
}