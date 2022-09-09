using System.ComponentModel.DataAnnotations;

namespace Relatorio.Core.Models
{
    public class Relatorio
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Por favor, preencha este campo")]
        [Display(Name="Tipo de relatório")]
        public string TipoRelatorio { get; set; }
        [Display(Name="Duração em segundos")]
        public int Duracao { get; set; }
        public bool Concluido { get; set; }
    }
}