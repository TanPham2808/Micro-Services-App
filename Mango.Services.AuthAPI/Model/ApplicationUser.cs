using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
