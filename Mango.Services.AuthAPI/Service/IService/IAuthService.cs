using Mango.Services.CouponAPI.Models.DTO;

namespace Mango.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegisterationRequestDTO userDTO);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
    }
}
