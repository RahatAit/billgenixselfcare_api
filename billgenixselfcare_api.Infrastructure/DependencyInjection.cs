using billgenixselfcare_api.Application.Interfaces;
using billgenixselfcare_api.Domain.Entities;
using billgenixselfcare_api.Infrastructure.Data;
using billgenixselfcare_api.Infrastructure.Repositories;
using billgenixselfcare_api.Infrastructure.Requirements;
using billgenixselfcare_api.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace billgenixselfcare_api.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            // Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? ""))
                };
            });

            // Authorization Policies
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DynamicPermission", policy =>
                    policy.Requirements.Add(new DynamicPermissionRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, DynamicPermissionHandler>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            // Repository Pattern
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Services
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
