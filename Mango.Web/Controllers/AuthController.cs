using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new();
            
            return View(loginRequestDTO);
        }

        [HttpGet]
        public IActionResult Register()
        {
            RegisterationRequestDTO registerationRequestDTO = new();
            return View(registerationRequestDTO);
        }

        [HttpGet]
        public IActionResult Assign()
        {
            RegisterationRequestDTO registerationRequestDTO = new();
            return View(registerationRequestDTO);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
