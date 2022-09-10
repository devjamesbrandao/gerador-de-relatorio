using Microsoft.EntityFrameworkCore;
using Relatorio.Core.Interfaces;
using Relatorio.Data.Context;

namespace Relatorio.Data.Repository
{
    public class RelatorioRepository : IRelatorioRepository
    {
        private readonly RelatorioContext _context;

        public RelatorioRepository(RelatorioContext context)
        {
            _context = context;
        }

        public async Task<List<Relatorio.Core.Models.Relatorio>> ObterTodosRelatorios()
        {
            return await _context.Relatorios.AsNoTracking().ToListAsync();
        }

        public async Task<Relatorio.Core.Models.Relatorio> AdicionarRelatorio(Relatorio.Core.Models.Relatorio relatorio)
        {
            _context.Relatorios.Add(relatorio);

            await _context.SaveChangesAsync();

            return relatorio;
        }

        public async Task AtualizarStatusRelatorio(int idRelatorio)
        {
            await _context.Database.ExecuteSqlRawAsync("update Relatorios set Concluido = 1 where Id = {0}", idRelatorio);
        }
    }
}