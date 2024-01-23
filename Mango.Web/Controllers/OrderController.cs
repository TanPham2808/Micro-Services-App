using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public IActionResult OrderIndex()
        {
            //var lstOrder = _orderService.GetAllOrder
            return View();
        }

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeaderDTO> list;
            string userId = "";
            if (!User.IsInRole(SD.RoleAdmin))
            {
                userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            ResponseDTO response = _orderService.GetAllOrder(userId).GetAwaiter().GetResult();
            if (response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<OrderHeaderDTO>>(Convert.ToString(response.Result));
                switch (status)
                {
                    case "approved":
                        list = list.Where(x => x.Status == SD.Status_Approved);
                        break;
                    case "readyforpickup":
                        list = list.Where(x => x.Status == SD.Status_ReadyForPickup);
                        break;
                    case "cancelled":
                        list = list.Where(x => x.Status == SD.Status_Cancelled);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                list = new List<OrderHeaderDTO>();
            }

            return Json(new { data = list });
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDTO orderHeaderDTO = new OrderHeaderDTO();
            string userId = User.Claims.Where(x => x.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var response = await _orderService.GetOrder(orderId);
            
            if (response.IsSuccess)
            {
                orderHeaderDTO = JsonConvert.DeserializeObject<OrderHeaderDTO>(Convert.ToString(response.Result));
            }
            if(!User.IsInRole(SD.RoleAdmin) && userId != orderHeaderDTO.UserId)
            {
                return NotFound();
            }
            return View(orderHeaderDTO);
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup);
            if(response.IsSuccess)
            {
                TempData["success"] = "Status Update successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }

            return View();
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Completed);
            if (response.IsSuccess)
            {
                TempData["success"] = "Status Update successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }

            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            if (response.IsSuccess)
            {
                TempData["success"] = "Status Update successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }

            return View();
        }
    }
}
