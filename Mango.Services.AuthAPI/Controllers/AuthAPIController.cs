using Mango.Services.AuthAPI.Model.DTO;
using Mango.Services.AuthAPI.Service.IService;
using Mango.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDTO _responseDTO;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _responseDTO = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequestDTO model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = errorMessage;
                return BadRequest(errorMessage);
            }

            return Ok(_responseDTO);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _authService.Login(model);
            if(loginResponse.User == null) 
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Username or password is incorrect";
                return BadRequest(_responseDTO);
            }
            _responseDTO.Result = loginResponse;
            return Ok(_responseDTO);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterationRequestDTO model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Error assign";
                return BadRequest(_responseDTO);
            }

            return Ok(_responseDTO);
        }
    }
}
