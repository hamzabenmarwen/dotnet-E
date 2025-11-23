using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TP2.ViewModels
{
    public class CreateViewModel
    {
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Prix en dinar :")]
        public float Price { get; set; }

        [Required]
        [Display(Name = "Quantité en unité :")]
        public int QteStock { get; set; }

        [Required]
        public int CategoryId { get; set; }

        // Removed: public Category Category { get; set; }  ← DELETE THIS

        [Required]
        [Display(Name = "Image :")]
        public IFormFile ImagePath { get; set; }
    }
}