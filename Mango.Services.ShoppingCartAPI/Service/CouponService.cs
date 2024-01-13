using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory clientFactory)
        {
            _httpClientFactory = clientFactory;
        }

        public async Task<CouponDTO> GetCoupon(string couponCode)
        {
            // Tạo connect đến Mango.Services.CouponAPI đã được config trong file program.cs
            var client = _httpClientFactory.CreateClient("Coupon");

            // Call API để nhận về responese
            var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();

            var resp = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(resp.Result));
            }

            return new CouponDTO();
        }
    }
}
