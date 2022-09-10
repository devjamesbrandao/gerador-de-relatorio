namespace Relatorio.Core.Interfaces
{
    public interface IRelatorioRepository
    {
        Task AtualizarStatusRelatorio(int idRelatorio);

        Task<Relatorio.Core.Models.Relatorio> AdicionarRelatorio(Relatorio.Core.Models.Relatorio relatorio);

        Task<List<Relatorio.Core.Models.Relatorio>> ObterTodosRelatorios();
    }
}