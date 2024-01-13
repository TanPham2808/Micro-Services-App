using Mango.Web.Service;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor(); // Dịch vụ này cung cấp quyền truy cập vào ngữ cảnh HTTP hiện tại, bao gồm thông tin về yêu cầu và phản hồi.
builder.Services.AddHttpClient(); // Cho phép bạn cấu hình và sử dụng các phiên bản HttpClient trong ứng dụng của bạn để thực hiện các yêu cầu HTTP đến các dịch vụ hoặc API khác.

// AddHttpClient được sử dụng để đăng ký một dịch vụ HTTP client trong container dịch vụ của ứng dụng
// Sử dụng để giao tiếp với các dịch vụ RESTful hoặc các endpoint HTTP khác
builder.Services.AddHttpClient<ICouponService, CouponService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<IProductService, ProductServices>();
builder.Services.AddHttpClient<ICartService, CartService>();

// Gán string localhost:700x từ config vào string để bên trong lấy ra sử dụng 
SD.CouponAPIBase = builder.Configuration["ServiceUrls:CouponAPI"]; 
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"];
SD.ProductAPIBase = builder.Configuration["ServiceUrls:ProductAPI"];
SD.ShoppingCartAPIBase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];

builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IProductService, ProductServices>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();

// Thêm xác thực thông qua cookie 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.ExpireTimeSpan = TimeSpan.FromHours(10);
        option.LoginPath = "/Auth/Login"; // Nếu chưa đăng nhập sẽ chuyển hướng đến đăng nhập
        option.AccessDeniedPath = "/Auth/AccessDenied"; // Chuyển đến trang truy cập bị từ chối
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
