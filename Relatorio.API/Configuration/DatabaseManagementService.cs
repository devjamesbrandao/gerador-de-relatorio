using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Relatorio.Data.Context;

namespace Relatorio.Configuration
{
    public static class DatabaseManagementService
    {
        public static void MigrationInitialisation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var database = serviceScope.ServiceProvider.GetService<RelatorioContext>();

                database.Database.EnsureDeleted();

                database.Database.Migrate();
            }
        }
    }
}