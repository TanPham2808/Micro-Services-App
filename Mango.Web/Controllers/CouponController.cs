using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            ResponseDTO respone = await _couponService.GetAllCouponAsync();
            List<CouponDTO> lstCoupon = null;
            if (respone.IsSuccess)
            {
                lstCoupon = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(respone.Result));
            }

            return View(lstCoupon);
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDTO model)
        {
            if (ModelState.IsValid)
            {
                ResponseDTO respone = await _couponService.CreateCouponAsync(model);
                if (respone.IsSuccess)
                {
                    TempData["success"] = "Coupon create successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = respone?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            ResponseDTO respone = await _couponService.GetCouponByIdAsync(couponId);
            if (respone.IsSuccess)
            {
				var coupon = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(respone.Result));
                return View(coupon);

			}
            return NotFound();

        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDTO couponDTO)
        {
            ResponseDTO respone = await _couponService.DeleteCouponAsync(couponDTO.CouponId);

            if (respone.IsSuccess)
            {
                TempData["success"] = "Coupon delete successfully";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = respone.Message;
            }
            return View();

        }
    }
}
