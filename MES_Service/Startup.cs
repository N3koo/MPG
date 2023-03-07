using MpgWebService.Repository.Interface;
using MpgWebService.Repository.Command;
using MpgWebService.Repository;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace MpgWebService {

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddSingleton<ICommandRepository, SapCommandRepository>();
            services.AddSingleton<IProductionRepository, ProductionRepository>();
            services.AddSingleton<IReportRepository, ReportRepository>();
            services.AddSingleton<IMpgRepository, MpgRepository>();
            services.AddSingleton<ISettingsRepository, SettingsRepository>();

            services.AddControllers();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MpgWebSerice", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MpgWebSerice v1");
                    c.DisplayOperationId();
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapSwagger();
            });
        }
    }
}
