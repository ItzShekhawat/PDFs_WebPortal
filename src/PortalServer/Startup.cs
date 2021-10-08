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

using PortalServer.Data;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Rewrite;
using System;

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

            // Config the DB Connection string 
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Unspecified;
                // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
                options.HandleSameSiteCookieCompatibility();
            });


            // Linking the Class and the Interface of login Repo
            services.AddScoped<ILoginRepo, LoginRepo>();

            // Linking the Default AutehticationStateProvider with  the custom one

            
            // Css Framework
            services.AddMudServices();

            // Session Storage Service Setup 
            services.AddBlazoredSessionStorage();

            //API Service
            services.AddSingleton<HttpClient>();


            // Caching
            services.AddMemoryCache();
           

            // Data Protection
            services.AddSingleton<UniqueCode>();
            services.AddSingleton<CustomIDataProtection>();

            // Azure AD Authentication 
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

            // Sign-in users with the Microsoft identity platform
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApp(Configuration)
                    .EnableTokenAcquisitionToCallDownstreamApi(new string[] { "User.Read" })
                    .AddInMemoryTokenCaches();

            // The following lines code instruct the asp.net core middleware to use the data in the "roles" claim in the Authorize attribute and User.IsInrole()
            services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                // The claim in the Jwt token where App roles are available.
                options.TokenValidationParameters.RoleClaimType = "roles";
                options.TokenValidationParameters.NameClaimType = "name";
            });

            services.AddRazorPages().AddMvcOptions(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                              .RequireAuthenticatedUser()
                              .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            }).AddMicrosoftIdentityUI();

            services.AddRazorPages();
            services.AddServerSideBlazor().AddMicrosoftIdentityConsentHandler();


            //Cookie staff
            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.LoginPath = "/";
                options.SlidingExpiration = true;
                options.LogoutPath = @"https://login.microsoftonline.com/common/oauth2/v2.0/logout";

            });
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
            app.UseCookiePolicy();
            app.UseRouting();
            app.UseStaticFiles();

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

            //Sign Out
            

            app.Use(async (context, next) => {
                if (context.Request.Path
                        .Equals("/signout-oidc", System.StringComparison.OrdinalIgnoreCase))
                {
                       context.Response.Redirect("/");
                    
                }
                await next();
            });



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
