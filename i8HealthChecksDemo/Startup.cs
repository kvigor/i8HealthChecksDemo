using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace i8HealthChecksDemo
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
            services.AddControllersWithViews();

            // <FirstShowcase name="HealthChecks">
            // Shorten health checks. Add health logic w/in
            // the lambda body for health checks
            services.AddHealthChecks()
                .AddCheck("API-X", () =>
                    HealthCheckResult.Healthy("The service is Healthy"));
                //.AddCheck("AzureTable", () =>
                //    HealthCheckResult.Unhealthy("The Azure Table Storage is Unhealthy"))
                //.AddCheck("AzureDatabase", () =>
                //    HealthCheckResult.Degraded("The AzureDatabase is Degraded"));
            // </FirstShowcase>

            //<SecondShowcase name="HealthChecks">
            //services.AddHealthChecks()
            //    .AddCheck("API", () =>
            //    {
            //        // Perform work checks here
            //        // ...
            //        // ...
            //        return HealthCheckResult.Healthy("The service is Healthy");
            //    });
            //</SecondShowcase>

            //<ThirdShowcase name="HealthChecks">
            //services.AddHealthChecks()
            //    .AddCheck("SideCarAPI", () =>
            //    {
            //        // Perform work checks here
            //        // ...
            //        // ...
            //        return HealthCheckResult.Degraded("The service is Healthy");
            //    }, new [] {"LoyaltyAPI", "SideCarContainer"}
            //    );
            //</ThirdShowcase>
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Simple check to day yes the system is running
                // However NO HEALTH CHECKS ARE BEING PERFORMED
                // You can use a service to check poll this URI
                // Predicate false mean do NOT run any of the 
                // health checks in the above ConfigureServices()
                endpoints.MapHealthChecks("/SimpleCheck", new HealthCheckOptions()
                {
                    // The _ meets the need of the method signature or lambda w/o the compiler warning
                    Predicate = _ => false
                });
                endpoints.MapHealthChecks("/HealthChecks", new HealthCheckOptions() 
                { 
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/HealthChecks/Services", new HealthCheckOptions()
                {
                    // The _ meets the need of the method signature or lambda w/o the compiler warning
                    Predicate = config =>  config.Tags.Contains("LoyaltyAPI"),
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse 
                });


                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
