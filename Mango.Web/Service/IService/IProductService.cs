using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
	public interface IProductService
	{
		Task<ResponseDTO> GetAllProductAsync();
		Task<ResponseDTO> GetProductByIdAsync(int id);
		Task<ResponseDTO> CreateProductAsync(CouponDTO couponDTO);
		Task<ResponseDTO> UpdateProductAsync(CouponDTO couponDTO);
		Task<ResponseDTO> DeleteProductAsync(int id);
	}
}
