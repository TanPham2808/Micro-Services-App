using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDTO> AssignAsync(RegisterationRequestDTO registerationRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.POST,
                Data = registerationRequestDTO,
                Url = SD.AuthAPIBase + $"/api/auth/AssignRole"
            });
        }

        public async Task<ResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDTO,
                Url = SD.AuthAPIBase + $"/api/auth/login"
            }, withBearer: false);
        }

        public async Task<ResponseDTO> RegisterAsync(RegisterationRequestDTO registerationRequestDTO)
        {
            return await _baseService.SendAsync(new RequestDTO
            {
                ApiType = SD.ApiType.POST,
                Data = registerationRequestDTO,
                Url = SD.AuthAPIBase + $"/api/auth/register"
            }, withBearer: false);
        }
    }
}
