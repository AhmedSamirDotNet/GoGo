using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Snap.Core.Entities;
using Snap.Core.Services;
using Snap.Repository.Data;
using Snap.Service.Token;

namespace Snap.APIs.Extensions
{
    public static class IdentityServicesExtension
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services) 
        {
            services.AddScoped<ITokenService, TokenService>();

            services.AddIdentity<User , IdentityRole>()
                .AddEntityFrameworkStores<SnapDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        
            return services;
        }
    }
}
