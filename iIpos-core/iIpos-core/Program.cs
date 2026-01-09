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

            const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

            // =========================
            // CORS (Render UI)
            // =========================
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins, policy =>
                {
                    policy
                        .WithOrigins("https://ui-ystf.onrender.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // =========================
            // Controllers + Swagger
            // =========================
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // =========================
            // DATABASE - POSTGRESQL
            // =========================
            builder.Services.AddDbContext<Data.MyDbContext>(options =>
            {
                var connectionString = builder.Configuration.GetConnectionString("MyDb");
                options.UseNpgsql(connectionString);
            });

            // =========================
            // DI Services
            // =========================
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ITableInfoService, TableInfoService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IStoreService, StoreService>();
            builder.Services.AddScoped<IBranchService, BranchService>();

            // =========================
            // SignalR
            // =========================
            builder.Services.AddSignalR();

            // =========================
            // JWT Authentication
            // =========================
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
                                path.StartsWithSegments("/orderHub"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                        )
                    };
                });

            var app = builder.Build();

            // =========================
            // Middleware
            // =========================
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "iIpos-core v1");
                c.RoutePrefix = "swagger";
            });

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapHub<iIpos_core.Hub.OrderHub>("/orderHub")
               .RequireCors(MyAllowSpecificOrigins);

            app.Run();
        }
    }
}
