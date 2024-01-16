using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Mango.GatewaySolution
{
    public static class WebApplicationBuilderExtensions
    {
        /// <summary>
        /// Authentication & Authorization call API
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
        {
            // Xác thực API
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidAudience = builder.Configuration["ApiSettings:Audience"],
                        ValidIssuer = builder.Configuration["ApiSettings:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["ApiSettings:Secret"]))
                    };
                });

            return builder;
        }
    }
}
