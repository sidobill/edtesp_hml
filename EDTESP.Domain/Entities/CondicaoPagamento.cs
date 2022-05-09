using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace EDTESP.Domain.Entities
{
    public class CondicaoPagamento
    {
        [Key]
        public int CondicaoPagamentoId { get; set; }

        public string Modelo { get; set; }

        public string Descricao { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime? DataAtualizacao { get; set; }

        public int UsuarioCriadorId { get; set; }

        public int? UsuarioAtualizadorId { get; set; }

        public bool Removido { get; set; }

        public bool Usado1Parc { get; set; }

        [ForeignKey("UsuarioCriadorId")]
        public virtual Usuario UsuarioCriador { get; set; }

        [ForeignKey("UsuarioAtualizadorId")]
        public virtual Usuario UsuarioAtualizador { get; set; }

        public virtual ICollection<FormaPagamento> FormasPagamentos { get; set; }

    }
}