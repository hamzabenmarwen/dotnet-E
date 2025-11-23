using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TP2.Models.Repositories;

namespace TP2.Components
{
    public class WishlistViewComponent : ViewComponent
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public WishlistViewComponent(IWishlistRepository wishlistRepository, UserManager<IdentityUser> userManager)
        {
            _wishlistRepository = wishlistRepository;
            _userManager = userManager;
        }

        public IViewComponentResult Invoke()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(HttpContext.User);
                var wishlistCount = _wishlistRepository.GetWishlistItemCount(userId);
                return View(wishlistCount);
            }

            return View(0);
        }
    }
}