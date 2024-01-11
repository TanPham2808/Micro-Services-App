using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Model;
using Mango.Services.AuthAPI.Service.IService;
using Mango.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if(!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    // Tạo role nếu không tồn tại
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDTO.UserName);
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (user==null || isValid == false)
            {
                return new LoginResponseDTO()
                {
                    User = null,
                    Token = ""
                };
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Generate JWT Toke
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDTO userDTO = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                User = userDTO,
                Token = token,
            };

            return loginResponseDTO;
        }

        public async Task<string> Register(RegisterationRequestDTO registerationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registerationRequestDTO.Email,
                Email = registerationRequestDTO.Email,
                NormalizedEmail = registerationRequestDTO.Email,
                Name = registerationRequestDTO.Name,
                PhoneNumber = registerationRequestDTO.PhoneNumber,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerationRequestDTO.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registerationRequestDTO.Email);

                    UserDTO userDTO = new()
                    {
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        Email = userToReturn.Email,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
                 
            }
            catch (Exception ex)
            {

            }

            return "Error Encountered";
        }
    }
}
