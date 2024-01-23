using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Mango.Services.OrderAPI.Models.DTO;
using Mango.Services.ShoppingCartAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Stripe;
using Stripe.Checkout;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDTO _res;
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public OrderAPIController(AppDbContext db, IMapper mapper, IProductService productService)
        {
            _db = db;
            this._res = new ResponseDTO();
            _mapper = mapper;
            _productService = productService;
        }

        [HttpPost("CreateOrder")]
        public async Task<ResponseDTO> CreateOrder([FromBody] CartDTO cartDTO)
        {
            try
            {
                OrderHeaderDTO orderHeaderDTO = _mapper.Map<OrderHeaderDTO>(cartDTO.CartHeader);
                orderHeaderDTO.OrderTime = DateTime.Now;
                orderHeaderDTO.Status = SD.Status_Pending;
                orderHeaderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailDTO>>(cartDTO.CartDetails);

                // Add vào DB
                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDTO)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDTO.OrderHeaderId = orderCreated.OrderHeaderId;
                _res.Result = orderHeaderDTO;
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }

            return _res;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDTO> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDTO)
        {
            try
            {
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDTO.ApprovedUrl,
                    CancelUrl = stripeRequestDTO.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var DiscountObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon = stripeRequestDTO.OrderHeader.CouponCode
                    }
                };

                foreach(var item in stripeRequestDTO.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), //$20.99 -> 2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                // Kiểm tra tổng tiền có nhiều hơn tiền tối thiểu hay ko ? Nếu có mới áp dụng giảm giá
                if(stripeRequestDTO.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountObj;
                }

                var service = new SessionService();
                Session session = service.Create(options);

                stripeRequestDTO.StripeSessionUrl = session.Url;
                
                OrderHeader orderHeader = _db.OrderHeaders.First(x => x.OrderHeaderId == stripeRequestDTO.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                await _db.SaveChangesAsync();

                _res.Result = stripeRequestDTO;

            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }

            return _res;
        }

        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDTO Get(string? userId = "")
        {
            try
            {
                IEnumerable<OrderHeader> objList;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    objList = _db.OrderHeaders.Include(x => x.OrderDetails).OrderByDescending(x => x.OrderHeaderId).ToList();
                }
                else
                {
                    objList = _db.OrderHeaders.Include(x => x.OrderDetails).Where(x => x.UserId == userId).OrderByDescending(x => x.OrderHeaderId).ToList();
                }
                _res.Result = _mapper.Map<IEnumerable<OrderHeaderDTO>>(objList);
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }
            return _res;
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDTO Get(int id)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.Include(x => x.OrderDetails).First(x => x.OrderHeaderId == id);
                _res.Result = _mapper.Map<OrderHeaderDTO>(orderHeader);
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }
            return _res;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDTO> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(x=>x.OrderHeaderId == orderId);
                if(orderHeader != null)
                {
                    // Hoàn tiền nếu Cancel Order
                    if(newStatus == SD.Status_Cancelled)
                    {
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = orderHeader.PaymentIntentId
                        };

                        var service = new RefundService();
                        Refund refund = service.Create(options);
                    }
                    orderHeader.Status = newStatus;
                    await _db.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }

            return _res;
        }
    }
}
