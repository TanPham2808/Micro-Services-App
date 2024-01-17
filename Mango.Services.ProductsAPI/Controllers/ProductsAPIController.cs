using AutoMapper;
using Mango.Services.ProductsAPI.Data;
using Mango.Services.ProductsAPI.Models;
using Mango.Services.ProductsAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mango.Services.ProductsAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductsAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDTO _res;
        private readonly IMapper _mapper;

        public ProductsAPIController(AppDbContext db, IMapper mapper)
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
        public ResponseDTO Post([FromForm] ProductDTO productDTO)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDTO);
                _db.Products.Add(product);
                _db.SaveChanges();

                // Xử lý image
                if (productDTO.Image != null)
                {
                    string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
                    string filePath = @"wwwroot\ProductImage\" + fileName;
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        productDTO.Image.CopyTo(fileStream);
                    }

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImage/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/603x403";
                }

                _db.Products.Update(product);
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
                    _res.Result = "";
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

                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if (file.Exists)
                    {
                        file.Delete();
                    }

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
