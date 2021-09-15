using Blazored.SessionStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;
using MudBlazor.Services;
using PortalAPI_Service.DbContextConnection;
using PortalAPI_Service.Repositories.AccessRepos;
using PortalServer.CacheRepo;
using PortalServer.Data;
using System.Net.Http;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Http;

namespace PortalServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            // Config the DB Connection string 
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            // Linking the Class and the Interface of login Repo
            services.AddScoped<ILoginRepo, LoginRepo>();

            // Linking the Default AutehticationStateProvider with  the custom one
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            
            // Css Framework
            services.AddMudServices();

            // Session Storage Service Setup 
            services.AddBlazoredSessionStorage();

            //API Service
            services.AddSingleton<HttpClient>();


            // Caching
            services.AddMemoryCache();
            services.AddSingleton<ICacheClass, CacheClass>();

            // Data Protection
            services.AddSingleton<UniqueCode>();
            services.AddSingleton<CustomIDataProtection>();

            services.AddMicrosoftIdentityWebApiAuthentication(Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int durationInSeconds = 60 * 60 * 24;
                    ctx.Context.Response.Headers[HeaderNames.CacheControl] =
                        "public,max-age=" + durationInSeconds;
                }
            });

            // Enable Authentication and Autorization
            app.UseAuthentication();
            app.Use(async (context, next) =>
            {
                if (!context.User.Identity?.IsAuthenticated ?? false)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Not Authenticated");
                }
                else
                {
                    await next();
                }
            });

            //app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
