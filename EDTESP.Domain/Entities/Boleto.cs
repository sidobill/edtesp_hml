using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class Boleto
    {
        [Key]
        public int BoletoId { get; set; }

        public int TituloId { get; set; }
        
        public int? ModalidadeId { get; set; }

        public int EmpresaId { get; set; }

        public int ClienteId { get; set; }

        public DateTime DataEmissao { get; set; }

        public DateTime DataVencto { get; set; }

        public DateTime DataVenctoReal { get; set; }

        public decimal Valor { get; set; }

        public decimal Saldo { get; set; }

        public decimal Multa { get; set; }

        public decimal Juros { get; set; }

        public DateTime? DataPgto { get; set; }

        public DateTime? DataDisponib { get; set; }

        public decimal? Desconto { get; set; }

        public decimal? Acrescimo { get; set; }

        public string NossoNumero { get; set; }

        public string NossoNumeroBanco { get; set; }

        public string LinhaDigitavel { get; set; }

        public string CodigoBarras { get; set; }

        public bool EnviadoAoBanco { get; set; }

        public DateTime? DataEnvioBanco { get; set; }

        public DateTime? DataProcessamentoBanco { get; set; }

        public DateTime DataCadastro { get; set; }

        public int UsuarioCriadorId { get; set; }

        public bool Cancelado { get; set; }

        public bool Gerado { get; set; }

        public bool EnviadoAoCliente { get; set; }

        public string NomePdf { get; set; }

        public DateTime? DataEnvioCliente { get; set; }

        public int? RemessaId { get; set; }

        public int? TipoBaixaId { get; set; }

        [ForeignKey("UsuarioCriadorId")]
        public virtual Usuario UsuarioCriador { get; set; }
        
        [ForeignKey("EmpresaId")]
        public virtual Empresa Empresa { get; set; }

        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("TituloId")]
        public virtual Titulo Titulo { get; set; }

        [ForeignKey("ModalidadeId")]
        public virtual ParametroBanco ParametroBanco { get; set; }

        [ForeignKey("TipoBaixaId")]
        public virtual TipoBaixa TipoBaixa { get; set; }
    }
}