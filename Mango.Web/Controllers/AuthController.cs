using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDTO loginRequestDTO = new();
            
            return View(loginRequestDTO);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO obj)
        {
            ResponseDTO responseDTO = await _authService.LoginAsync(obj);

            if (responseDTO != null && responseDTO.IsSuccess)
            {
                LoginResponseDTO loginResponseDTO = 
                    JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(responseDTO.Result));

                // Xác định và lưu danh tính nhận dạng user
                await SignInUser(loginResponseDTO);

                // Set token sau khi login thanh cong
                _tokenProvider.SetToken(loginResponseDTO.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = responseDTO.Message;
                return View(obj);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin, Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer, Value=SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterationRequestDTO obj)
        {
            ResponseDTO result = await _authService.RegisterAsync(obj);
            ResponseDTO assignRole;

            if (result!= null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignAsync(obj);
                if(assignRole!=null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Register success";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = result.Message;
            }

            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin, Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer, Value=SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;

            return View();
        }

        [HttpGet]
        public IActionResult Assign()
        {
            RegisterationRequestDTO registerationRequestDTO = new();
            return View(registerationRequestDTO);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearedToken();

            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(LoginResponseDTO model)
        {
            var handler = new JwtSecurityTokenHandler();

            //Giải mã token
            var jwt = handler.ReadJwtToken(model.Token);

            // Định danh user cùng với các giá trị được giải
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, 
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));  
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            // Định danh user vào biến principal
            var principal = new ClaimsPrincipal(identity);

            // Đưa các giá trị vào Cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
