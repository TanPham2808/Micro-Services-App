using AutoMapper;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDTO _res;
        private readonly IMapper _mapper;

        private IProductService _productService;
        private ICouponService _couponService;

        public CartAPIController(AppDbContext db, IMapper mapper, IProductService productService, ICouponService couponService)
        {
            _db = db;
            _res = new ResponseDTO();
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDTO> GetCart(string userId)
        {
            try
            {
                CartDTO cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDTO>(_db.CartHeaders.First(x => x.UserId == userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailDTO>>(_db.CartDetails.Where(x => x.CartHeaderId == cart.CartHeader.CartHeaderId));

                // Đây là API giỏ hàng nên về sản phẩm sẽ ko thể lấy được
                // Danh sách sản phẩm call từ project Service Product API
                IEnumerable<ProductDTO> productDTOs = await _productService.GetProducts();

                foreach(var item in cart.CartDetails)
                {
                    item.Product = productDTOs.FirstOrDefault(x => x.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                // Apply coupon nếu có
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDTO coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if(coupon.CouponCode != null && cart.CartHeader.CartTotal > coupon.MinAmout)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                _res.Result = cart;

            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }

            return _res;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDTO cartDTO)
        {
            try
            {
                var cartDb = await _db.CartHeaders.FirstAsync(x => x.UserId == cartDTO.CartHeader.UserId);
                cartDb.CouponCode = cartDTO.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartDb);
                await _db.SaveChangesAsync();

                _res.Result = true;

            }
            catch (Exception ex)
            {
                _res.Message = ex.Message;
                _res.IsSuccess = false;
            }

            return _res;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] CartDTO cartDTO)
        {
            try
            {
                var cartDb = await _db.CartHeaders.FirstAsync(x => x.UserId == cartDTO.CartHeader.UserId);
                cartDb.CouponCode = "";
                _db.CartHeaders.Update(cartDb);
                await _db.SaveChangesAsync();

                _res.Result = true;

            }
            catch (Exception ex)
            {
                _res.Message = ex.Message;
                _res.IsSuccess = false;
            }

            return _res;
        }


        [HttpPost("CartUpsert")]
        public async Task<ResponseDTO> CartUpsert(CartDTO cartDTO)
        {
            try
            {
                var cartHederDB = await _db.CartHeaders.FirstOrDefaultAsync(c=>c.UserId == cartDTO.CartHeader.UserId);
                if (cartHederDB == null) // Tạo giỏ hàng
                {
                    CartHeader cartHeader = new CartHeader()
                    {
                        UserId = cartDTO.CartHeader.UserId,
                        CouponCode = cartDTO.CartHeader.CouponCode,
                        CartTotal = cartDTO.CartHeader.CartTotal,
                    };
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();

                    cartDTO.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                    await _db.SaveChangesAsync();

                }
                else // Nếu trong giỏ hàng có rồi thì Upsert
                {
                    var cartDetailDB = _db.CartDetails
                        .AsNoTracking()
                        .FirstOrDefault(
                        x=>x.CartHeaderId == cartHederDB.CartHeaderId &&
                        x.ProductId == cartDTO.CartDetails.First().ProductId);

                    if (cartDetailDB == null)
                    {
                        // Create CartDetail
                        cartDTO.CartDetails.First().CartHeaderId = cartHederDB.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _db.SaveChangesAsync();

                    }
                    else // Update lại số lượng
                    {
                        cartDTO.CartDetails.First().Count += cartDetailDB.Count;
                        cartDTO.CartDetails.First().CartHeaderId = cartDetailDB.CartHeaderId;
                        cartDTO.CartDetails.First().CartDetailsId = cartDetailDB.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                _res.Result = cartDTO;
            }
            catch (Exception ex)
            {
                _res.Message = ex.Message;
                _res.IsSuccess = false;
            }

            return _res;
        }


        [HttpPost("CartRemove")]
        public async Task<ResponseDTO> RemoveCart([FromBody] int cartDetailId)
        {
            try
            {
                var cartDetails = _db.CartDetails.First(x => x.CartDetailsId == cartDetailId);
                int totalCountCartItem = _db.CartDetails.Where(x => x.CartHeaderId == cartDetails.CartHeaderId).Count();
                
                _db.CartDetails.Remove(cartDetails);
                if(totalCountCartItem == 1)
                {
                    var carHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(x => x.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(carHeaderToRemove);

                }
                await _db.SaveChangesAsync();

                _res.Result = true;
            }
            catch (Exception ex)
            {
                _res.Message = ex.Message;
                _res.IsSuccess = false;
            }

            return _res;
        }
    }
}
