using Microsoft.AspNetCore.Mvc;
using TP2.Models.Help;

namespace TP2.Components
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cartCount = ListeCart.Instance.Items.Sum(item => item.quantite);
            return View(cartCount);
        }
    }
}