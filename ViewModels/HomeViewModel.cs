namespace TP2.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
