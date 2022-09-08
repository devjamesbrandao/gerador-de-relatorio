using Microsoft.EntityFrameworkCore;

namespace Relatorio.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Relatorio.Core.Models.Relatorio> Relatorios { get;set; }
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
    }
}