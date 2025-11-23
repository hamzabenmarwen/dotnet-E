using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TP2.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        public string CategoryName { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
