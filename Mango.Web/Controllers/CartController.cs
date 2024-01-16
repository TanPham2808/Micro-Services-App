using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            var cartDTO = await LoadCartDTOBaseOnLoggedInUser();
            return View(cartDTO);
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var cartDTO = await LoadCartDTOBaseOnLoggedInUser();

            cartDTO.CartHeader.Email = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            return View(cartDTO);
        }

        private async Task<CartDTO> LoadCartDTOBaseOnLoggedInUser()
        {
            // Lấy UserID từ claim
            var userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var res = await _cartService.GetCartByUserIdAsync(userId);
            if (res != null && res.IsSuccess)
            {
                CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(res.Result));
                return cartDTO;
            }

            return new CartDTO();
        }

        public async Task<IActionResult> Remove(int cartDetailId)
        {
            var userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var res = await _cartService.RemoveFromCartAsync(cartDetailId);
            if (res != null && res.IsSuccess)
            {
                CartDTO cartDTO = JsonConvert.DeserializeObject<CartDTO>(Convert.ToString(res.Result));
                TempData["success"] = "Remove product in cart success";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDTO cartDTO)
        {
            var res = await _cartService.ApplyCouponAsync(cartDTO);
            if (res != null && res.IsSuccess)
            {
                TempData["success"] = "Apply coupon success";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDTO cartDTO)
        {
            cartDTO.CartHeader.CouponCode = "";
            var res = await _cartService.ApplyCouponAsync(cartDTO);
            if (res != null && res.IsSuccess)
            {
                TempData["success"] = "Remove coupon success";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> EmailCart()
        {
            CartDTO cart = await LoadCartDTOBaseOnLoggedInUser();
            cart.CartHeader.Email = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;

            // Gửi mail lên services bus
            var res = await _cartService.EmailCart(cart);
            if (res != null && res.IsSuccess)
            {
                TempData["success"] = "Email will be processed and sent shortly";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        

    }
}
