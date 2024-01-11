using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDTO> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<ResponseDTO> RegisterAsync(RegisterationRequestDTO registerationRequestDTO);
        Task<ResponseDTO> AssignAsync(RegisterationRequestDTO registerationRequestDTO);
    }
}
