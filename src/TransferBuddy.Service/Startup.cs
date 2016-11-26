using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.Swagger.Model;
using Messenger.Client.Extensions;
using TransferBuddy.Repositories;
using TransferBuddy.Service.Services;

namespace TransferBuddy.Service
{
    /// <summary>
    /// The startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initialize a new instance of the <see cref="Startup"/> class
        /// </summary>
        /// <param name="env">The environment.</param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        /// <summary>
        /// The configuration.
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services to configure.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            var pageAccessToken = Environment.GetEnvironmentVariable("PAGE_ACCESS_TOKEN");
            if (pageAccessToken == null)
            {
                throw new Exception("Cannot find PAGE_ACCESS_TOKEN in this env.");
            }

            var verifyToken = Environment.GetEnvironmentVariable("VERIFY_TOKEN");
            if (verifyToken == null)
            {
                throw new Exception("Cannot find VERIFY_TOKEN in this env.");
            }
 
            services.AddAuthentication(options => {
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            services.AddMvc();

            services.AddScoped<UserRepository, UserRepository>();
            services.AddSingleton<RedisService, RedisService>();
            services.AddSingleton<MessageProcessorService, MessageProcessorService>();

            services.AddSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
                {
                    Version = "v1",
                    Title = "Price TAG Cloud API",
                    Description = "API Version 1",
                    TermsOfService = "None"
                });
            });

            services.AddMessengerClient(pageAccessToken);
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="app">The application to configure.</param>
        /// <param name="env">The environment to configure.</param>
        /// <param name="loggerFactory">The log factory to configure.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();
            app.UseBrowserLink();

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
 
            app.UseCookieAuthentication(new CookieAuthenticationOptions {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true, 
                LoginPath = new PathString("/signin"),
                LogoutPath = new PathString("/signout")
            });  

            app.UseTransferwiseAuthentication(options =>  {
                options.ClientId = "f272f4a3-ecc1-44fe-b3f4-9a20e9433f4e";
                options.ClientSecret = "534cda42-719c-4b26-86c2-c96b7cb03437";
            });
 
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();

            app.UseSwaggerUi();
        }
    }
}
