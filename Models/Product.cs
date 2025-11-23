using System.ComponentModel.DataAnnotations;

namespace TP2.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Prix en dinar :")]
        public float Price { get; set; }

        [Required]
        [Display(Name = "Quantité en unité :")]
        public int QteStock { get; set; }

        [Required] // ← Add this
        public int CategoryId { get; set; }

        public string Image { get; set; }
        public Category Category { get; set; }
        // Add to your Product model
        public bool IsInWishlist { get; set; } = false;
    }
}
