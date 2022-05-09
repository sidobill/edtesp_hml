using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class Titulo
    {
        [Key]
        public int TituloId { get; set; }

        public int ContratoId { get; set; }

        public int ClienteId { get; set; }

        public int VendedorId { get; set; }

        public int FormaPagamentoId { get; set; }

        public int CondicaoPagamentoId { get; set; }

        public int Parcela { get; set; }
        
        public DateTime DataCriacao { get; set; }

        public DateTime DataVencto { get; set; }

        public DateTime DataVenctoReal { get; set; }

        public decimal Valor { get; set; }

        public decimal Saldo { get; set; }

        public DateTime? DataBaixa { get; set; }

        public DateTime? DataProcessamento { get; set; }

        public DateTime? DataCancelamento { get; set; }

        public DateTime? DataAtualizacao { get; set; }
        
        public int UsuarioCriadorId { get; set; }

        public int? UsuarioAtualizadorId { get; set; }

        public bool Suspenso { get; set; }

        public bool Removido { get; set; }

        public int? TipoBaixaId { get; set; }

        [ForeignKey("ContratoId")]
        public virtual Contrato Contrato { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("VendedorId")]
        public virtual Vendedor Vendedor { get; set; }

        [ForeignKey("FormaPagamentoId")]
        public virtual FormaPagamento FormaPagamento { get; set; }

        [ForeignKey("CondicaoPagamentoId")]
        public virtual CondicaoPagamento CondicaoPagamento { get; set; }

        [ForeignKey("UsuarioCriadorId")]
        public virtual Usuario UsuarioCriador { get; set; }

        [ForeignKey("UsuarioAtualizadorId")]
        public virtual Usuario UsuarioAtualizador { get; set; }

        [ForeignKey("TituloId")]
        public virtual ICollection<Boleto> Boletos { get; set; }

        [ForeignKey("TipoBaixaId")]
        public virtual TipoBaixa TipoBaixa { get; set; }
    }
}