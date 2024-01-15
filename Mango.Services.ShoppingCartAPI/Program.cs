using Microsoft.EntityFrameworkCore;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI;
using AutoMapper;
using Mango.Services.ShoppingCartAPI.Extensions;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Mango.Services.ShoppingCartAPI.Service;
using Mango.Services.ShoppingCartAPI.Utility;
using Mango.MessageBus;

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
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IMessageBus, MessageBus>();

// Ủy quyền phụ trợ (Lấy token đã login được để authentication đến request ở project Coupon API)
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthentivationHttpClientHander>();

// Thêm ứng dụng khách http để triển khai gọi Services Product 
builder.Services.AddHttpClient(
    "Product", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:ProductAPI"]))
    .AddHttpMessageHandler<BackendApiAuthentivationHttpClientHander>();
builder.Services.AddHttpClient("Coupon", u => u.BaseAddress = new Uri(builder.Configuration["ServiceUrls:CouponAPI"]))
    .AddHttpMessageHandler<BackendApiAuthentivationHttpClientHander>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
