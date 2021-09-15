using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using PortalAPI_Service.Repositories.AccessRepos;
using PortalAPI_Service.Repositories.DirectoryRepos;
using PortalAPI_Service.Repositories.FoldersRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;


namespace PortalAPI_Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PortalAPI_Service", Version = "v1" });
            });
            /*
            services.AddHttpClient("somename", c =>
            {
                c.BaseAddress = new Uri(Configuration.GetValue<string>("baseURL"));
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
            {
                UseDefaultCredentials = true
            });
            */
            // Linking the Class and the Interface
            services.AddScoped<ILoginRepo, LoginRepo>();
            services.AddScoped<IFoldersRepo, FoldersRepo>();
            services.AddScoped<IDirRepo, DirRepo>();

            services.AddMemoryCache();

            //Servizi di Autenticazione
            //services.AddAuthentication(IISDefaults.AuthenticationScheme);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PortalAPI_Service v1"));
            }

            app.UseHttpsRedirection();


            app.UseRouting();

            // Enable Authentication and Autorization
            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            /*
            app.Run(async (context) =>
            {
                try
                {
                    var user = (WindowsIdentity)context.User.Identity;

                    #pragma warning disable CA1416 // Validate platform compatibility
                    await context.Response
                        .WriteAsync($"User: {user.Name}\tState: {user.ImpersonationLevel}\n");

                    WindowsIdentity.RunImpersonated(user.AccessToken, () =>
                    {
                        var impersonatedUser = WindowsIdentity.GetCurrent();
                        var message =
                            $"User: {impersonatedUser.Name}\t" +
                            $"State: {impersonatedUser.ImpersonationLevel}";
                        Console.WriteLine("This is the message : \n" + message + "\n");

                        var bytes = Encoding.UTF8.GetBytes(message);
                        context.Response.Body.Write(bytes, 0, bytes.Length);
                    });

                    #pragma warning restore CA1416 // Validate platform compatibility

                }
                catch (Exception e)
                {
                    await context.Response.WriteAsync(e.ToString());
                }
            });
            */


        }
    }
}
