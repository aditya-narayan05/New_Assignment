
using backendNew.AutoMapper;
using backendNew.DataAccessLayer;
using backendNew.Dtos;
using backendNew.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;

namespace backendNew
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
                    RoleClaimType = ClaimTypes.Role
                };
            });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(Options =>
            {
                Options.UseSqlServer(connectionString);
            });

            builder.Services.AddScoped<IUser, UserRepo>();
            builder.Services.AddScoped<IItem, ItemRepo>();
            builder.Services.AddScoped<ILogin, LoginRepo>();
            //builder.Services.AddScoped<IItem, ItemRepo>();

            //builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

            var devCorsPolicy = "devCorsPolicy";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(devCorsPolicy, builder =>
                {
                    //builder.WithOrigins("http://localhost:800").AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                    //builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
                    //builder.SetIsOriginAllowed(origin => true);
                });
            });
            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("devCorsPolicy");
            app.UseCors("AllowAll");
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
