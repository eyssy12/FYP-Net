namespace TestApi
{
    using System;
    using System.IdentityModel.Tokens;
    using System.IO;
    using System.Security.Cryptography;
    using CMS.Web.Configuration.Data;
    using CMS.Web.Configuration.Data.Models;
    using Extensions;
    using Microsoft.AspNet.Authentication.JwtBearer;
    using Microsoft.AspNet.Authorization;
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Diagnostics;
    using Microsoft.AspNet.Hosting;
    using Microsoft.AspNet.Http;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.PlatformAbstractions;
    using Newtonsoft.Json;
    using TestApi.Library.Security;
    using TestApi.Serialization;

    public class Startup
    {
        private const string TokenAudience = "Client";
        private const string TokenIssuer = "FYPCMS";

        private RsaSecurityKey key;
        private TokenAuthOptions tokenOptions;

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<Entities>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<Entities>()
                .AddDefaultTokenProviders();

            services.AddSingleton<MyJsonSerializerSettings>();
            services.AddSingleton<TokenStorageService>();

            RSAParameters keyParams = RsaExtensions.GetKeyParameters(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "keys.json"));
            key = new RsaSecurityKey(keyParams);
            tokenOptions = new TokenAuthOptions()
            {
                Audience = TokenAudience,
                Issuer = TokenIssuer,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature)
            };

            // Save the token options into an instance so they're accessible to the controller.
            services.AddInstance(tokenOptions);

            // Enable the use of an [Authorize("Bearer")] attribute on methods and classes to protect.
            services.AddAuthorization(auth =>
            {
                auth.AddPolicy(JwtBearerDefaults.AuthenticationScheme, new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

                    if (error != null && error.Error is SecurityTokenExpiredException)
                    {
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject(
                                new
                                {
                                    authenticated = false,
                                    tokenExpired = true
                                }));
                    }
                    else if (error != null && error.Error != null)
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";
                        
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject(
                                new
                                {
                                    success = false,
                                    error = error.Error.Message
                                }));
                    }
                    else
                    {
                        await next();
                    }
                });
            });

            app.UseJwtBearerAuthentication(options =>
            {
                options.TokenValidationParameters.IssuerSigningKey = key;
                options.TokenValidationParameters.ValidAudience = tokenOptions.Audience;
                options.TokenValidationParameters.ValidIssuer = tokenOptions.Issuer;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters.ValidateSignature = true;
                options.TokenValidationParameters.ValidateLifetime = true;
                options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(0);
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentity();
            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}