using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class TipoBaixa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TipoBaixaId { get; set; }

        public string Descricao { get; set; }
    }
}