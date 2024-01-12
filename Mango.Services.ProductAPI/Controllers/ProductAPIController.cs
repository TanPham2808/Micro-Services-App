using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDTO _res;
        private readonly IMapper _mapper;

        public ProductAPIController(AppDbContext db, IMapper mapper)
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
                var lstProduct = _db.Products.ToList();
                _res.Result = _mapper.Map<List<ProductDTO>>(lstProduct);
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }

            return _res;
        }

        // GET api/<ProductAPIController>/5
        [HttpGet("{id}")]
        public ResponseDTO Get(int id)
        {
            try
            {
                var product = _db.Products.First(x=>x.ProductId == id);
                _res.Result = product;
            }
            catch (Exception ex)
            {
                _res.IsSuccess=false;
                _res.Message = ex.Message;
            }

            return _res;
        }

        // POST api/<ProductAPIController>
        [HttpPost]
        public ResponseDTO Post([FromBody] ProductDTO productDTO)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDTO);
                _db.Products.Add(product);
                _db.SaveChanges();

                _res.Result = _mapper.Map<ProductDTO>(product);
            }
            catch (Exception ex)
            {
                _res.IsSuccess=false;
                _res.Message = ex.Message;
            }

            return _res;
        }

        // PUT api/<ProductAPIController>/5
        [HttpPut("{id}")]
        public ResponseDTO Put(int id, [FromBody] ProductDTO productDTO)
        {
            try
            {
                var product = _db.Products.FirstOrDefault(x=>x.ProductId == id);
                if(product == null)
                {
                    _res.Message = "Don't have product";
                    return _res;
                }

                product.Name = productDTO.Name;
                product.Price = productDTO.Price;
                product.Description = productDTO.Description;

                _db.Products.Update(product);
                _db.SaveChanges();

                _res.Result = product;
            }
            catch (Exception ex)
            {
                _res.IsSuccess = false;
                _res.Message = ex.Message;
            }
            return _res;
        }

        // DELETE api/<ProductAPIController>/5
        [HttpDelete("{id}")]
        public ResponseDTO Delete(int id)
        {
            try
            {
                var product = _db.Products.FirstOrDefault(x => x.ProductId == id);
                if (product == null)
                {
                    _res.Message = "Don't have Product";
                    return _res;
                }

                _db.Products.Remove(product);
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
