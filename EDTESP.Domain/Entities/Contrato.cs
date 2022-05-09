using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class Contrato
    {
        [Key]
        public int ContratoId { get; set; }

        public int Numero { get; set; }

        public int ClienteId { get; set; }

        public int VendedorId { get; set; }

        public int AnoEdicao { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime DataAtualizacao { get; set; }

        public DateTime DataInicio { get; set; }
        
        public DateTime? DataTermino { get; set; }

        public DateTime? DataAprovacao { get; set; }

        public DateTime? DataCancelamento { get; set; }

        public DateTime DataUltimaSituacao { get; set; }

        public decimal ValorBase { get; set; }

        public decimal PrimeiraParcela { get; set; }

        public decimal Tarifas { get; set; }

        public decimal Desconto { get; set; }

        public decimal ValorFinal { get; set; }

        public int FormaPagamentoId { get; set; }

        public int CondicaoPagamentoId { get; set; }

        public int FormaPagamento1ParcId { get; set; }

        public int CondicaoPagamento1ParcId { get; set; }

        public DateTime DataVencto1Parc { get; set; }

        public int UsuarioCriadorId { get; set; }

        public int? UsuarioAtualizadorId { get; set; }

        public int StatusContratoId { get; set; }

        public int EmpresaId { get; set; }
        
        public string Observacao { get; set; }

        public string Descricao { get; set; }

        public string Especie { get; set; }

        public string Categoria { get; set; }

        public int? MotivoSuspensaoId { get; set; }

        public bool EnviadoAoCliente { get; set; }

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

        [ForeignKey("ContratoId")]
        public virtual ICollection<Titulo> Titulos { get; set; }

        [ForeignKey("StatusContratoId")]
        public virtual StatusContrato StatusContrato { get; set; }

        [ForeignKey("ContratoId")]
        public virtual ICollection<ContratoEvento> ContratoEventos { get; set; }
        
        [ForeignKey("EmpresaId")]
        public virtual Empresa Empresa { get; set; }
        
        [ForeignKey("MotivoSuspensaoId")]
        public virtual MotivoSuspensao MotivoSuspensao { get; set; }
    }
}