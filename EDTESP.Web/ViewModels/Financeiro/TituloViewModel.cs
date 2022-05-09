using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using EDTESP.Web.ViewModels.Cadastros;

namespace EDTESP.Web.ViewModels.Financeiro
{
    public class TituloViewModel
    {
        [DisplayName("Cód")]
        public int TituloId { get; set; }
        
        [DisplayName("Parcela")]
        public int Parcela { get; set; }

        [DisplayName("Dt. Criação")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DataCriacao { get; set; }

        [DisplayName("Vencto.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataVencto { get; set; }

        [DisplayName("Vencto. real")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataVenctoReal { get; set; }

        [DisplayName("Valor (R$)")]
        public decimal Valor { get; set; }

        [DisplayName("Saldo (R$)")]
        public decimal Saldo { get; set; }

        [DisplayName("Dt. Baixa")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DataBaixa { get; set; }

        [DisplayName("Dt. Proc")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DataProcessamento { get; set; }

        [DisplayName("Dt. Cancel.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DataCancelamento { get; set; }

        [DisplayName("Dt. Atual.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DataAtualizacao { get; set; }
        
        [DisplayName("Atual. em")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public int? UsuarioAtualizadorId { get; set; }

        [DisplayName("Suspenso")]
        public bool Suspenso { get; set; }
        
        [DisplayName("Contrato")]
        public int Contrato { get; set; }

        [DisplayName("Cliente")]
        public string Cliente { get; set; }

        [DisplayName("Vendedor")]
        public string Vendedor { get; set; }

        [DisplayName("Forma Pgto.")]
        public string FormaPagamento { get; set; }

        [DisplayName("Cond. Pgto.")]
        public string CondicaoPagamento { get; set; }

        [DisplayName("Criador por")]
        public string UsuarioCriador { get; set; }

        [DisplayName("Atual. por")]
        public string UsuarioAtualizador { get; set; }

        [DisplayName("Boletos")]
        public ICollection<BoletoViewModel> Boletos { get; set; }

        public FormaPagamentoViewModel FormaPagamentoObj { get; set; }

        public bool EmAberto => Saldo > 0;

        public bool Atrasado => DataVenctoReal.Date < DateTime.Now.Date;

        public double DiasEmAtraso => (DateTime.Now.Date - DataVenctoReal.Date).TotalDays;

        public bool GeraBoleto => FormaPagamentoObj.GeraBoleto;

        public bool PossuiBoletoGerado => Boletos.Any(x => !x.Cancelado && x.Gerado);

        public bool PossuiBoleto => Boletos.Any(x => !x.Cancelado);

        public BoletoViewModel UltimoBoleto => Boletos.OrderByDescending(x => x.BoletoId).FirstOrDefault(x => !x.Cancelado);

        public BoletoViewModel UltimoBoletoGerado => Boletos.OrderByDescending(x => x.BoletoId).FirstOrDefault(x => !x.Cancelado && x.Gerado);

        [DisplayName("Status Pgto.")]
        public string StatusPgto => Suspenso ? "Suspenso" : DataBaixa.HasValue ? "Pago" : Atrasado ? "Em Atraso" : "Em Aberto";

        public bool PermissaoParaMarcarComoPago { get; set; }

        public bool PermissaoParaMarcarComoNaoPago { get; set; }
    }
}