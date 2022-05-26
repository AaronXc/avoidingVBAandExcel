using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MotorsAndBatteries.data;
using MotorsAndBatteries.repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//added
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
//using WebApiAndAngular;

namespace MotorsAndBatteries
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

            services.AddDbContext<ComponentContext>();
            services.AddSingleton<ISheetMap, SheetMap>();
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddControllersWithViews();
            //services.AddControllers();
            services.AddMvc(options => options.EnableEndpointRouting=false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "wwwroot";
            });

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MotorsAndBatteries", Version = "v1" });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               //app.UseSwagger();
               // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MotorsAndBatteries v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseCors();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllerRoute("default", "{controller=Values}/{action=somea_action}");
                endpoints.MapControllers();
            });
            
            app.UseMvc();
            //ApiDocStartup.Configure(app);
            /*Spa Startup*/

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                //if (env.IsDevelopment())
                //{
                //    spa.UseAngularCliServer(npmScript: "start");
                //}
            });


        }
    }
}
