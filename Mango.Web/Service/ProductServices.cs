using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
	public class ProductServices : IProductService
	{
		private readonly IBaseService _baseService;

        public ProductServices(IBaseService baseService)
        {
			_baseService = baseService;
		}

        public async Task<ResponseDTO> CreateProductAsync(ProductDTO productDTO)
		{
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.POST,
                Data = productDTO,
                Url = SD.ProductAPIBase + $"/api/product/"
            });
        }

		public async Task<ResponseDTO> DeleteProductAsync(int id)
		{
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.DELETE,
                Url = SD.ProductAPIBase + $"/api/product/{id}"
            });
        }

		public async Task<ResponseDTO> GetAllProductAsync()
		{
			return await _baseService.SendAsync(new RequestDTO
			{
				ApiType = SD.ApiType.GET,
				Url = SD.ProductAPIBase + "/api/product"
			});
		}

		public async Task<ResponseDTO> GetProductByIdAsync(int id)
		{
			return await _baseService.SendAsync(new RequestDTO
			{
				ApiType = SD.ApiType.GET,
				Url = SD.ProductAPIBase + $"/api/product/{id}"
			});
		}

		public async Task<ResponseDTO> UpdateProductAsync(ProductDTO productDTO)
		{
			return await _baseService.SendAsync(new RequestDTO
			{
				ApiType = SD.ApiType.PUT,
				Data = productDTO,
				Url = SD.ProductAPIBase + $"/api/product/{productDTO.ProductId}"
			});
		}
	}
}
