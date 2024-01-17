using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
			_productService = productService;

		}

		// GET: ProductController
		public async Task<ActionResult> ProductIndex()
        {
			ResponseDTO respone = await _productService.GetAllProductAsync();

			if (respone != null && respone.IsSuccess)
			{
				var lstCoupon = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(respone.Result));
				return View(lstCoupon);
			}
			else
			{
				TempData["error"] = respone.Message;
			}

			return RedirectToAction("Index", "Home");
        }

        // GET: ProductController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductController/Create
        public ActionResult ProductCreate()
        {
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        public async Task<ActionResult> ProductCreate(ProductDTO model)
        {
			if (ModelState.IsValid)
			{
				ResponseDTO respone = await _productService.CreateProductAsync(model);
				if (respone != null && respone.IsSuccess)
				{
					TempData["success"] = "Coupon create successfully";
					return RedirectToAction(nameof(ProductIndex));
				}
				else
				{
					TempData["error"] = respone?.Message;
				}
			}
			return View(model);
		}

        // GET: ProductController/Edit/5
        public async Task<ActionResult> ProductEdit(int productId)
        {
			ResponseDTO respone = await _productService.GetProductByIdAsync(productId);
			if (respone.IsSuccess)
			{
				var product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(respone.Result));
				return View(product);

			}
			return NotFound();
		}

        // POST: ProductController/Edit/5
        [HttpPost]
        public async Task<ActionResult> ProductEdit(ProductDTO productDTO)
        {
			ResponseDTO respone = await _productService.UpdateProductAsync(productDTO);

			if (respone!= null & respone.IsSuccess)
			{
				TempData["success"] = "Product update successfully";
				return RedirectToAction(nameof(ProductIndex));
			}
			else
			{
				TempData["error"] = respone.Message;
			}
			return View();
		}

        // GET: ProductController/Delete/5
        public async Task<ActionResult> ProductDelete(int productId)
        {
            ResponseDTO respone = await _productService.GetProductByIdAsync(productId);
            if (respone.IsSuccess)
            {
                var product = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(respone.Result));
                return View(product);

            }
            return NotFound();
        }

        // POST: ProductController/Delete/5
        [HttpPost]
        public async Task<ActionResult> ProductDelete(ProductDTO productDTO)
        {
            ResponseDTO respone = await _productService.DeleteProductAsync(productDTO.ProductId);

            if (respone.IsSuccess)
            {
                TempData["success"] = "Product delete successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = respone.Message;
            }
            return View();
        }
    }
}
