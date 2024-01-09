using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDTO _res;
        private readonly IMapper _mapper;

        public CouponAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _res = new ResponseDTO();
            _mapper = mapper;
        }

        [HttpGet]
        public ResponseDTO Get()
        {
            try
            {
                var lstCoupon = _db.Coupons.ToList();
                _res.Result = _mapper.Map<List<CouponDTO>>(lstCoupon);
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }

            return _res;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDTO Get(int id)
        {
            try
            {
                var coupon = _db.Coupons.First(x => x.CouponId == id);
                _res.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }

            return _res;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDTO GetByCode(string code)
        {
            try
            {
                var coupon = _db.Coupons.First(x => x.CouponCode.ToLower() == code.ToLower());
                _res.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }

            return _res;
        }

        [HttpPost]
        public ResponseDTO POST([FromBody] CouponDTO couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Add(coupon);
                _db.SaveChanges();


                _res.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }

            return _res;
        }

        [HttpPut]
        public ResponseDTO PUT([FromBody] CouponDTO couponDto)
        {
            try
            {
                var coupon = _db.Coupons.FirstOrDefault(x => x.CouponId == couponDto.CouponId);
                if(coupon == null)
                {
                    _res.Message = "Don't have Coupon";
                    return _res;
                }

                coupon.CouponCode = couponDto.CouponCode;
                coupon.DiscountAmount = couponDto.DiscountAmount;
                coupon.MinAmout = couponDto.MinAmout;

                _db.Coupons.Update(coupon);
                _db.SaveChanges();

                _res.Result = coupon;
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }

            return _res;
        }

        [HttpDelete]
        public ResponseDTO Delete(int id)
        {
            try
            {
                var coupon = _db.Coupons.FirstOrDefault(x => x.CouponId == id);
                if (coupon == null)
                {
                    _res.Message = "Don't have Coupon";
                    return _res;
                }

                _db.Coupons.Remove(coupon);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }

            return _res;
        }
    }
}
