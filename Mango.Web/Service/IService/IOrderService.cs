using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDTO> CreateOrderAsync(CartDTO cartDTO);
        Task<ResponseDTO> CreateStripeSession(StripeRequestDTO stripeRequestDTO);

    }
}
