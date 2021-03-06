using CoreAppAuthDemo.CustomTokenProviders;
using CoreAppAuthDemo.CustomValdiators;
using CoreAppAuthDemo.Models;
using CoreAppAuthDemo.Repositories;
using CoreAppAuthDemo.Services;
using EmailService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreAppAuthDemo.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IApplicationUserDetailsRepo, ApplicationUserDetailsService>();
            services.AddScoped<IMasterDataServiceRepo, MasterDataService>();
        }

        public static void ConfigureJsonOptions(this IServiceCollection services)
        {
            services.AddMvc().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
            });
        }

        public static void ConfigureIdentityContext(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                opt.Lockout.MaxFailedAccessAttempts = 3;
            })
            .AddEntityFrameworkStores<ApplicationDataContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<EmailConfirmationTokenProvider<User>>("emailconfirmation")
            .AddPasswordValidator<CustomPasswordValidator<User>>();

            services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromMinutes(10));
            services.Configure<EmailConfirmationTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromHours(2));
        }

        public static void ConfigureEmailSettings(this IServiceCollection services, IConfiguration Configuration)
        {
            var emailConfig = Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IEmailSender, EmailSender>();
            services.Configure<FormOptions>(o =>
            {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
        }

        public static void ConfigureGoogleOAuth(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAuthentication().AddGoogle("google", options =>
            {
                var googleAuth = Configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuth["ClientId"];
                options.ClientSecret = googleAuth["ClientSecret"];
                options.SignInScheme = IdentityConstants.ExternalScheme;
            });
        }

        public static void ConfigureAzureAdOAuth(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddRazorPages().AddMicrosoftIdentityUI();
            services.AddAuthentication().AddOpenIdConnect(options =>
            {
                var azureAdAuth = Configuration.GetSection("Authentication:AzureAd");
                options.ClientId = azureAdAuth["ClientId"];
                options.Authority = $"{azureAdAuth["Instance"]}/{azureAdAuth["TenantId"]}";
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.ClientSecret = azureAdAuth["ClientSecret"];
                options.CallbackPath = azureAdAuth["CallbackPath"];
                options.SignedOutCallbackPath = azureAdAuth["SignedOutCallbackPath"];
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(azureAdAuth["ClientSecret"])),
                    ValidateIssuer = false
                };
            });
        }
    }
}
