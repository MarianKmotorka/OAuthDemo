using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OauthDemoApi.Hubs;
using OauthDemoApi.Persistance;

namespace OauthDemoApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJwtAuthentication(Configuration);
            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.ApiVersionSelector = new LowestImplementedApiVersionSelector(options);
                options.AssumeDefaultVersionWhenUnspecified = true;
            });

            services.AddDbContext<Db>(o =>
                o.UseSqlServer(Configuration.GetConnectionString("OAuthDemoDb")));

            services.AddControllers();
            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCors(config =>
                {
                    config.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            }
            else
                app.UseHsts();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(cfg =>
            {
                cfg.MapHub<ChatHub>("api/chat");
                cfg.MapControllers();
            });
        }
    }
}
