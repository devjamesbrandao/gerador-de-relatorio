using Microsoft.EntityFrameworkCore;

namespace Relatorio.Data.Context
{
    public class RelatorioContext : DbContext
    {
        public DbSet<Relatorio.Core.Models.Relatorio> Relatorios { get;set; }
        
        public RelatorioContext(DbContextOptions<RelatorioContext> options) : base(options)
        {
            
        }
    }
}