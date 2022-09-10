using System;
using DevFreela.infrastructure.MessageBus;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Relatorio.Configuration;
using Relatorio.Core.Interfaces;
using Relatorio.Data.Context;
using Relatorio.Data.Hubs;
using Relatorio.Data.MessageBus.Consumer;
using Relatorio.Data.Repository;

namespace Relatorio
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
            var server = Configuration["DbServer"] ?? "localhost";
            var port = Configuration["DbPort"] ?? "1433"; 
            var user = Configuration["DbUser"] ?? "sa"; 
            var password = Configuration["Password"] ?? "1q2w3e4r@#$";
            var database = Configuration["Database"] ?? "Relatorios";

            var connectionString = $"Server={server}, {port};Initial Catalog={database};User ID={user};Password={password}";

            services.AddDbContext<RelatorioContext>(
                options => 
                    options.UseSqlServer(
                        connectionString,
                        options => options.MigrationsAssembly("Relatorio.Data")
                    )
                    .LogTo(Console.WriteLine)
            );

            //Add Sql Server Health Check 
            services.AddHealthChecks()
            .AddSqlServer(
                connectionString: connectionString, 
                name: "Sql Server instance",
                tags: new[] { "database" }
            ).AddRabbitMQ(
                rabbitConnectionString: Configuration.GetConnectionString("RabbitMQ"), 
                name: "rabbitMQ",
                tags: new[] { "message-broker"}
            );
            
            // Add Health Check UI
            services.AddHealthChecksUI().AddInMemoryStorage();

            services.AddControllersWithViews();

            services.AddCors(options =>
            {
                options.AddPolicy("ClientPermission", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowed(x => true)
                        .AllowCredentials();
                });
            });

            services.AddSignalR();

            services.AddScoped<IMessageBusPublishService, MessageBusPublishService>();

            services.AddScoped<IRelatorioRepository, RelatorioRepository>();

            services.AddHostedService<RelatorioConsumer>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            DatabaseManagementService.MigrationInitialisation(app);

            app.UseCors("ClientPermission");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecksUI();

                //Route to all Health Checks
                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = p => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

                endpoints.MapHub<RelatorioHub>("/relatorioHub");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            //Route to acess health check UI
            app.UseHealthChecksUI(options => { options.UIPath = "/dashboard"; });
        }
    }
}
