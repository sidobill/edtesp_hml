using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class FormaCondicaoPagamento
    {
        [Key, Column(Order = 0)]  
        public int FormaPagamentoId { get; set; }

        [Key, Column(Order = 1)]
        public int CondicaoPagamentoId { get; set; }

        [ForeignKey("FormaPagamentoId")]
        public FormaPagamento FormaPagamento { get; set; }

        [ForeignKey("CondicaoPagamentoId")]
        public CondicaoPagamento CondicaoPagamento { get; set; }
    }
}