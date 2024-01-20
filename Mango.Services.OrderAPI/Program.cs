using Microsoft.EntityFrameworkCore;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI;
using AutoMapper;
using Mango.Services.OrderAPI.Extensions;
using Mango.Services.OrderAPI.Service.IService;
using Mango.Services.OrderAPI.Service;
using Mango.Services.OrderAPI.Utility;
using Mango.MessageBus;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#region Setup Auto Mapper
// Tạo 1 class MappingConfig
IMapper mapper = MappingConfig.RegisterMap().CreateMapper();

builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion

// Khai báo scope để dùng DI
builder.Services.AddScoped<IProductService, Mango.Services.OrderAPI.Service.ProductService>();
builder.Services.AddScoped<IMessageBus, MessageBus>();

// Ủy quyền phụ trợ (Lấy token đã login được để authentication đến request ở project Coupon API)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthentivationHttpClientHander>();

// Thêm ứng dụng khách http để triển khai gọi Services Product 
builder.Services.AddHttpClient(
    "Product", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]))
    .AddHttpMessageHandler<BackendApiAuthentivationHttpClientHander>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Extention tự tạo để Author API
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

app.Run();
