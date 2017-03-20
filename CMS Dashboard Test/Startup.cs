using CMS.Dashboard.Test.Managers;
using CMS.Dashboard.Test.Models;
using CMS.Dashboard.Test.Serialization;
using CMS.Dashboard.Test.Services;
using CMS.Web.Configuration.Data;
using CMS.Web.Configuration.Data.Models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PushSharp.Google;

namespace CMS_Dashboard_Test
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebEncoders();
            services.AddDataProtection();

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });

            // Add framework services.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<Entities>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionStringAzure"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRoleManager<ApplicationRoleManager>()
                .AddEntityFrameworkStores<Entities>()
                .AddDefaultTokenProviders();

            services.AddAuthorization();

            services.AddSingleton<MyJsonSerializerSettings>();
            services.AddSingleton<EmailSenderService>();
            services.AddScoped<GCMNotificationService>();
            services.AddScoped<INotifyService<GcmNotification>, GCMNotificationService>();
            services.AddScoped<INotify<EmailMessage>, EmailSenderService>();

            services.AddCaching();
            services.AddSession();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // roleManager.EnsureRolesCreated(); RoleManager<IdentityRole> roleManager

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseSession();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            if (env.IsProduction())
            {
                app.Use(async (context, next) =>
                {
                    if (context.Request.IsHttps) //Before RC1, this was called 
                    {
                        await next();
                    }
                    else
                    {
                        var withHttps = "https://" + context.Request.Host + context.Request.Path;
                        context.Response.Redirect(withHttps);
                    }
                });
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles(); // this has to be before use Mvc, static content shouldn't go through the MVC pipeline

            app.UseIdentity();

            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}", // question mark says that the id part of the routing is not required and is optional, if id is provided, it will go in as an argument to the method of the action
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
