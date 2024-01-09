using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

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
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            return View(model);
        }
    }
}
