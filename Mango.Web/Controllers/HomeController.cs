using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;

        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            ResponseDTO respone = await _productService.GetAllProductAsync();
            List<ProductDTO> lstProduct = new();

            if (respone != null && respone.IsSuccess)
            {
                lstProduct = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(respone.Result));
                return View(lstProduct);
            }
            else
            {
                TempData["error"] = respone.Message;
            }

            return View(lstProduct);
        }

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ResponseDTO respone = await _productService.GetProductByIdAsync(productId);
            ProductDTO model = new();

            if (respone != null && respone.IsSuccess)
            {
                model = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(respone.Result));
            }
            else
            {
                TempData["error"] = respone?.Message;
            }

            return View(model);
        }

        

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}