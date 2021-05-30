using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Tasktower.OcelotGateway.Configuration.StartupExtensions;
using Tasktower.OcelotGateway.Security.Middlewares;

namespace Tasktower.OcelotGateway
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
            services.ConfigureCors(Configuration);
            services.ConfigureCookies(Configuration);
            services.ConfigureWebAppAuth(Configuration);
            services.ConfigureAntiForgery(Configuration);
            services.AddOcelot(Configuration);
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseForwardedHeaders();

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCorsConfig(Configuration);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<CsrfMiddleware>();
            app.UseMiddleware<AccessTokenGetMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            await app.UseOcelot();
        }
    }
}
