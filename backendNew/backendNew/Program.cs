using backendNew.AutoMapper;
using backendNew.DataAccessLayer;
using backendNew.Dtos;
using backendNew.Model;
using backendNew.Repository;
using backendNew.NewMiddleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace backendNew
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // ? Initialize Serilog BEFORE builder
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("Logs/api-log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var builder = WebApplication.CreateBuilder(args);

            // ? Hook Serilog into the host
            builder.Host.UseSerilog(Log.Logger);

            // ?? Configure Services
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IUser, UserRepo>();
            builder.Services.AddScoped<IItem, ItemRepo>();
            builder.Services.AddScoped<ILogin, LoginRepo>();
            // builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            var devCorsPolicy = "devCorsPolicy";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(devCorsPolicy, builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // ?? Seed Data (Optional)
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                if (context.Items.Count() < 1000)
                {
                    var currentCount = context.Items.AsNoTracking().Count();
                    var items = new List<Item>();

                    for (int i = currentCount + 1; i <= 1000; i++)
                    {
                        items.Add(new Item
                        {
                            Name = $"Item {i}",
                            Description = $"Description for item {i}",
                            Quantity = i,
                            Price = i * 10
                        });
                    }

                    context.Items.AddRange(items);
                    context.SaveChanges();
                }
            }

            // ?? Configure Middleware Pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("devCorsPolicy");
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();

            // Custom Middlewares
            app.UseMiddleware<ExceptionLoggingMiddleware>(); // catch errors first
            app.UseMiddleware<ApiLoggingMiddleware>();       // log request/response

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}