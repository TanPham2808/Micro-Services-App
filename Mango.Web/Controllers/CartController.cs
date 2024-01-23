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
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
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

        [Authorize]
        [ActionName("CheckOutCart")]
        public async Task<IActionResult> Checkout(CartDTO cartDTO)
        {
            CartDTO cart = await LoadCartDTOBaseOnLoggedInUser();
            cart.CartHeader.Phone = cartDTO.CartHeader.Phone;
            cart.CartHeader.Email = cartDTO.CartHeader.Email;
            cart.CartHeader.Name = cartDTO.CartHeader.Name;

            var response = await _orderService.CreateOrderAsync(cart);
            OrderHeaderDTO orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));

            if(response.IsSuccess)
            {
                // Mở cổng thanh toán Stripe (get session stripe session)
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDTO stripeRequestDTO = new()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDTO.OrderHeaderId,
                    CancelUrl = domain + "cart/CheckOutCart",
                    OrderHeader = orderHeaderDTO
                };

                var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDTO);
                StripeRequestDTO stripeReponseResult = JsonConvert.DeserializeObject<StripeRequestDTO>(Convert.ToString(stripeResponse.Result));

                // Chuyển page
                Response.Headers.Add("Location", stripeReponseResult.StripeSessionUrl);
                return new StatusCodeResult(303); // Mã 303 cho biết là có chuyển hướng
            }
            else
            {
                TempData["error"] = response.Message;
            }

            return RedirectToAction("Checkout");
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            return View(orderId);
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
            return RedirectToAction(nameof(CartIndex));
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
