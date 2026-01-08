

using iIpos_core.Service.Branch;
using iIpos_core.Service.Category;
using iIpos_core.Service.Order;
using iIpos_core.Service.Product;
using iIpos_core.Service.StoreService;
using iIpos_core.Service.TableInfo;
using ilpos_core.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace iIpos_core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {

                                      policy.WithOrigins("http://localhost:4200")
                                             .AllowAnyHeader()
                                            .AllowAnyMethod()
                                             .AllowCredentials();
                                  });
            });
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<Data.MyDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("MyDb");
                options.UseSqlServer(connectionString);
            });
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ITableInfoService, TableInfoService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IStoreService, StoreService>();
            builder.Services.AddScoped<IBranchService, BranchService>();
            builder.Services.AddSignalR();
         

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/orderHub"))) 
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

            var app = builder.Build();
            app.UseStaticFiles();

            // 2. Cấu hình Swagger (Tạo file .json)
            app.UseSwagger();

            // 3. Cấu hình Swagger UI (Trang web giao diện)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "iipos-core v1");
                c.RoutePrefix = "swagger";
            });

            // 4. Bật Routing (Định tuyến) - Phải đứng TRƯỚC Cors và Authorization
            app.UseRouting();

            // 5. Sử dụng CORS (Dùng ĐÚNG tên bạn đã định nghĩa)
            app.UseCors(MyAllowSpecificOrigins);

            // 6. Bật xác thực
            app.UseAuthentication();

            // 7. Bật phân quyền
            app.UseAuthorization();

            // 8. Map API Controllers (Chỉ gọi MỘT LẦN)
            app.MapControllers();

            // 9. Map SignalR Hub
            app.MapHub<iIpos_core.Hub.OrderHub>("/orderHub");

            // 10. Chạy ứng dụng
            app.Run();
        }
    }
}
