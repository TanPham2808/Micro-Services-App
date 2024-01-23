using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDTO> CreateOrderAsync(CartDTO cartDTO);
        Task<ResponseDTO> CreateStripeSession(StripeRequestDTO stripeRequestDTO);
        Task<ResponseDTO> ValidateStripeSession(int orderHeaderId);
        Task<ResponseDTO> GetAllOrder(string? userId);
        Task<ResponseDTO> GetOrder(int orderId);
        Task<ResponseDTO> UpdateOrderStatus(int orderId, string newStatus);
        
    }
}
