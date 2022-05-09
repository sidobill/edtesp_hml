using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using EDTESP.Web.ViewModels.Cadastros;

namespace EDTESP.Web.ViewModels.Financeiro
{
    public class BoletoViewModel
    {
        [DisplayName("Cód")]
        public int BoletoId { get; set; }

        [DisplayName("Titulo")]
        public int TituloId { get; set; }

        [DisplayName("Cedente")]
        public int CedenteId { get; set; }

        [DisplayName("Banco")]
        public int BancoId { get; set; }

        [DisplayName("Cliente")]
        public int ClienteId { get; set; }

        [DisplayName("Emissão")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DataEmissao { get; set; }

        [DisplayName("Vencto.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataVencto { get; set; }

        [DisplayName("Vencto. Real")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataVenctoReal { get; set; }

        [DisplayName("Valor")]
        public decimal Valor { get; set; }
        
        [DisplayName("Multa")]
        public decimal Multa { get; set; }

        [DisplayName("Juros")]
        public decimal Juros { get; set; }

        [DisplayName("Dt. Pgto.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DataPgto { get; set; }

        [DisplayName("Dt. Disp.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DataDisponib { get; set; }

        [DisplayName("Desconto")]
        public decimal? Desconto { get; set; }

        [DisplayName("Acresc.")]
        public decimal? Acrescimo { get; set; }

        [DisplayName("Nosso Num.")]
        public string NossoNumero { get; set; }

        [DisplayName("Nosso Num. Bco.")]
        public string NossoNumeroBanco { get; set; }

        [DisplayName("Linha Dig.")]
        public string LinhaDigitavel { get; set; }

        [DisplayName("Cod. Barras")]
        public string CodigoBarras { get; set; }

        [DisplayName("No Banco")]
        public bool EnviadoAoBanco { get; set; }

        [DisplayName("No Banco")]
        public string EnviadoAoBancoStr { get; set; }

        [DisplayName("Dt. Envio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DataEnvioBanco { get; set; }

        [DisplayName("Dt. Proc.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DataProcessamentoBanco { get; set; }

        [DisplayName("Dt. Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DataCadastro { get; set; }

        [DisplayName("Usuario Criador")]
        public int UsuarioCriadorId { get; set; }

        [DisplayName("Cancelado")]
        public bool Cancelado { get; set; }

        [DisplayName("Cancelado")]
        public string CanceladoStr { get; set; }

        [DisplayName("Cedente")]
        public string Empresa { get; set; }

        [DisplayName("Cliente")]
        public string Cliente { get; set; }

        [DisplayName("Banco")]
        public string Banco { get; set; }

        [DisplayName("Criador por")]
        public string UsuarioCriador { get; set; }

        public bool Gerado { get; set; }
        
        public ClienteViewModel ClienteObj { get; set; }

        public EmpresaViewModel EmpresaObj { get; set; }

        public bool Atrasado => DataVenctoReal.Date < DateTime.Now.Date;

        public double DiasEmAtraso => (DateTime.Now.Date - DataVenctoReal.Date).TotalDays;

        public string StatusPgto => DataPgto.HasValue ? "Pago" : Atrasado ? "Em Atraso" : "Em Aberto";
    }
}