using System;
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
            services.AddMvc();
        }

	    private static bool IsAdminPath(HttpContext httpContext)
	    {
		    return httpContext.Request.Path.Value.StartsWith(@"/api/admin/", StringComparison.OrdinalIgnoreCase);
	    }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

			// Install-Package Microsoft.AspNetCore.Proxy
			app.MapWhen(IsAdminPath, builder => builder.RunProxy(new ProxyOptions
	        {
		        Scheme = "http",
		        Host = "localhost",
		        Port = "55329"
			}));

	        app.Use(async (context, next) =>
	        {
		        await context.Response.WriteAsync("Hello from Frond End");
		        return;
	        });

			app.UseMvc();
        }
    }
}
