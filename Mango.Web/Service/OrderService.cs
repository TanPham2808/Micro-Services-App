using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO> CreateOrderAsync(CartDTO cartDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.POST,
                Data = cartDTO,
                Url = SD.OrderAPIBase + $"/api/order/CreateOrder"
            });
        }

        public async Task<ResponseDTO> CreateStripeSession(StripeRequestDTO stripeRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.POST,
                Data = stripeRequestDTO,
                Url = SD.OrderAPIBase + $"/api/order/CreateStripeSession"
            });
        }

        public async Task<ResponseDTO> ValidateStripeSession(int orderHeaderId)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.POST,
                Data = orderHeaderId,
                Url = SD.OrderAPIBase + $"/api/order/ValidateStripeSession"
            });
        }

        public async Task<ResponseDTO> GetAllOrder(string? userId)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + $"/api/order/GetOrders/?userId=" + userId
            });
        }

        public async Task<ResponseDTO> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + $"/api/order/GetOrder/" + orderId
            });
        }

        public async Task<ResponseDTO> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.POST,
                Data = newStatus,
                Url = SD.OrderAPIBase + $"/api/order/UpdateOrderStatus/"+orderId
            });
        }
    }
}
