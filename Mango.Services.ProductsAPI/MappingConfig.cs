using AutoMapper;
using Mango.Services.ProductsAPI.Models;
using Mango.Services.ProductsAPI.Models.DTO;

namespace Mango.Services.ProductsAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMap()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDTO, Product>();
                config.CreateMap<Product, ProductDTO>();
            });

            return mappingConfig;
        }
    }
}
