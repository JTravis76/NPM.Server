using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Mail;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace NPM.Server.Extensions
{
    public static class ServiceExt
    {
        public static void AddEFContext(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<DataContext>(opt => opt.UseSqlite(configuration["sqlconnection:Sqlite"]));
            //services.AddDbContext<DataContext>( opt => opt.UseSqlServer(configuration["sqlconnection:MyDbName"]));
        }

        public static void AddMVC(this IServiceCollection services)
        {
            services.AddMvc(config =>
            {
                config.MaxModelValidationErrors = 10;
            })
            .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1)
            .AddJsonOptions(opt =>
            {
                //switch back to PascalCase from default camelCase
                opt.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
            });
        }

        public static void AddCORS(this IServiceCollection services, string policyName = "CorsPolicy")
        {
            services.AddCors(opt => {
                opt.AddPolicy(policyName,
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });
        }

        public static void AddIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(opt => { });
        }

        public static void AddSmtpClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(serviceProvider =>
            {
                return new SmtpClient()
                {
                    Host = configuration["Email:Smtp:Host"],
                    Port = Convert.ToInt32(configuration["Email:Smtp:Port"]),
                    EnableSsl = false,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = true
                };
            });
        }

    }
}
