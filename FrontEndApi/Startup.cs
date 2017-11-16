using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FrontEndApi
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
            // If you aren't using the Microsoft.AspNetCore.All metapackage, install version 2.0+ of the Microsoft.AspNetCore.Authentication.Cookies NuGet package
            // services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

            services.AddAuthentication(o =>
            {
                o.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.LoginPath = new PathString("/api/auth-status");
            });

            services.AddMvc();
        }

        private static bool IsAdminPath(HttpContext httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            return httpContext.Request.Path.Value.StartsWith(@"/api/admin/", StringComparison.OrdinalIgnoreCase);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.Lax //required by OAuth2
            };

            app.UseCookiePolicy(cookiePolicyOptions);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //If you aren't using the Microsoft.AspNetCore.All metapackage, install version 2.0+ of the Microsoft.AspNetCore.Authentication.Cookies NuGet package
            app.UseAuthentication();

            app.Use(async (context, next) =>
            {
                if (!context.Request.Path.Value.Contains("/api/auth") && (context.User?.Identity == null || !context.User.Identity.IsAuthenticated))
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                await next();
            });

            // Install-Package Microsoft.AspNetCore.Proxy
            app.MapWhen(IsAdminPath, builder => builder.RunProxy(new ProxyOptions
            {
                Scheme = "http",
                Host = "localhost",
                Port = "55329"
            }));

            app.UseMvc();
        }
    }
}
