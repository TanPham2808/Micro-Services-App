using AutoMapper;
using Mango.Services.CouponAPI;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stripe;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#region Auto Mapper
// Thêm mapper config
IMapper mapper = MappingConfig.RegisterMap().CreateMapper();
builder.Services.AddSingleton(mapper);

// Method này trả về danh sách các assembly được load trong domain hiện tại
// Thường được sử dụng để AutoMapper có thể quét và tìm các profile ánh xạ đã được định nghĩa trong các assembly này.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

// Extention để Auth API
builder.AddAppAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
ApplyMigration(); // Kiểm tra xem có quá trình Migrate nào đang chờ xử lý hay không. Nếu có thì chạy Migrate

app.Run();

void ApplyMigration()
{
    using(var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Kiểm tra có Migra nào chưa chạy ko
        if(_db.Database.GetPendingMigrations().Count() > 0)
        {
            // Sẽ tự động chạy Migration
            _db.Database.Migrate();
        }
    }
}
