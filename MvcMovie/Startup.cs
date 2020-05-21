using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SCIM;
using Microsoft.SCIM.WebHostSample.Provider;
using MvcMovie.Data;
using MvcMovie.Services;

namespace MvcMovie
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
            // These are the important implementations for integrating with Microsoft.SCIM
            services.AddSingleton<IMonitor, LoggerMonitor>();
            services.AddScoped<IProvider, ScimProvider>();
            services.AddScoped<ScimUserProvider>();

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)

                .AddAzureAD(options => Configuration.Bind("AzureAd", options))

                // This adds an scim authentication policy so that
                // SCIM endpoints have bearer authentication while the
                // UI has AzureAD authentication
                .AddJwtBearer("scim", options =>
                {
                    options.Authority = Configuration["Scim:TokenIssuer"];
                    options.Audience = Configuration["Scim:TokenAudience"];
                });

            services.AddRazorPages();

            services.AddDbContext<UserDataContext>(options =>
                {
                    //options.UseSqlServer(Configuration.GetConnectionString("MvcMovieContext");
                    options.UseSqlite("Data Source=users.db");
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
