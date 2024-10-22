using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MpgWebService.Business.Interface.Service;
using MpgWebService.Business.Interface.Settings;
using MpgWebService.Business.Service;
using MpgWebService.Business.Settings;
using MpgWebService.Repository;
using MpgWebService.Repository.Clients;
using MpgWebService.Repository.Command;
using MpgWebService.Repository.Interface;
using System.Text.Json;

namespace MpgWebService
{

    public class Startup {

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddControllers();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MpgWebSerice", Version = "v1" });
            });

            // add DI services
            services.AddTransient<ICommandService, CommandService>();
            services.AddTransient<IMpgService, MpgService>();
            services.AddTransient<IProductionService, ProductionService>();
            services.AddTransient<IReportService, ReportService>();
            services.AddTransient<ISettingsService, SettingsService>();

            // add other DI
            services.AddSingleton<ICommandRepository, MesCommandRepository>();
            services.AddSingleton<IMpgRepository, MpgRepository>();
            services.AddSingleton<IProductionRepository, ProductionRepository>();
            services.AddSingleton<IReportRepository, ReportRepository>();
            services.AddSingleton<ISettingsRepository, SettingsRepository>();
            services.AddSingleton<ISettings, ConfigSettings>();

            // add DbClient DI
            services.AddSingleton<MpgClient, MpgClient>();
            services.AddSingleton<MesClient, MesClient>();
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

                app.UseExceptionHandler(handler => {
                    handler.Run(async context => {
                        var exception = context.Features.Get<IExceptionHandlerPathFeature>();
                        var jsonMessage = JsonSerializer.Serialize(exception.Error.Message);

                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";

                        await context.Response.WriteAsync(jsonMessage);
                    });
                });

                app.UseHsts();
            }

            app.UseRouting();
            
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapSwagger();
            });
        }
    }
}
