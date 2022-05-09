using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class FormaPagamento
    {
        public int FormaPagamentoId { get; set; }

        public string Descricao { get; set; }

        public bool GeraBoleto { get; set; }

        public bool Removido { get; set; }

        public int UsuarioCriadorId { get; set; }

        public DateTime DataCadastro { get; set; }

        public int? UsuarioAtualizadorId { get; set; }

        public DateTime? UltimaAtualizacao { get; set; }

        [ForeignKey("UsuarioCriadorId")]
        public virtual Usuario UsuarioCriador { get; set; }

        [ForeignKey("UsuarioAtualizadorId")]
        public virtual Usuario UsuarioAtualizador { get; set; }

        public virtual ICollection<CondicaoPagamento> CondicaoPagamentos { get; set; }
    }
}