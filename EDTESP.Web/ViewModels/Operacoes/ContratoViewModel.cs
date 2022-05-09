using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using EDTESP.Web.Helpers;
using EDTESP.Web.ViewModels.Cadastros;
using EDTESP.Web.ViewModels.Financeiro;

namespace EDTESP.Web.ViewModels.Operacoes
{
    public class ContratoViewModel
    {
        [DisplayName("Cód")]
        public int ContratoId { get; set; }

        [DisplayName("Numero")]
        [Required]
        [RegularExpression("[0-9]+", ErrorMessage = "Informe um número Válido")]
        [Remote("ContratoExiste","Contratos",AdditionalFields = "ContratoId", ErrorMessage = "Número de Contrato já utilizado")]
        [Range(1,int.MaxValue, ErrorMessage = "Informe um número Válido")]
        public int? Numero { get; set; }

        [DisplayName("Produto/Cedente")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Informe o Produto/Cedente")]
        public int EmpresaId { get; set; }

        [DisplayName("Cliente")]
        [Required]
        [Range(1,int.MaxValue, ErrorMessage = "Informe o Cliente")]
        public int ClienteId { get; set; }

        [DisplayName("Vendedor")]
        [Range(1, int.MaxValue, ErrorMessage = "Informe o Vendedor")]
        public int VendedorId { get; set; }
        
        [DisplayName("Criado em")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataCriacao { get; set; }

        [DisplayName("Ult. Atual.")]
        public DateTime DataAtualizacao { get; set; }

        [DisplayName("Ano Edição")]
        [Required]
        [Range(1000,9999, ErrorMessage = "Digite um ano válido")]
        public int AnoEdicao { get; set; }

        [DisplayName("Dt. Início")]
        [Required]
        [DateTimeRange(ErrorMessage = "Data Inválida")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataInicio { get; set; }

        [DisplayName("Dt. Termino")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DataTermino { get; set; }

        [DisplayName("Dt. Cancelamento")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? DataCancelamento { get; set; }
        
        [DisplayName("Dt. Ult. Sit.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataUltimaSituacao { get; set; }

        [DisplayName("Vlr. base")]
        [Required]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal ValorBase { get; set; }

        [DisplayName("Tarifas")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal Tarifas { get; set; }

        [DisplayName("Descontos")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal Desconto { get; set; }

        [DisplayName("Vlr. Final")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal ValorFinal { get; set; }

        [DisplayName("1° Parcela")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal PrimeiraParcela { get; set; }

        [DisplayName("Dt. Vencto 1° Parcela")]
        [Required]
        [DateTimeRange(ErrorMessage = "Data Inválida")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataVencto1Parc { get; set; }

        [DisplayName("Cond. Pgto.")]
        public int CondicaoPagamentoId { get; set; }

        [DisplayName("Cond. Pgto.")]
        [Required]
        public string CondicaoPagamentoIdStr { get; set; }

        [DisplayName("Cond. Pgto. 1° Parc.")]
        //[Required]
        public string CondicaoPagamento1ParcIdStr { get; set; }

        [DisplayName("Forma Pgto.")]
        public int FormaPagamentoId { get; set; }

        [DisplayName("Cliente")]
        public string Cliente { get; set; }

        [DisplayName("Vendedor")]
        public string Vendedor { get; set; }

        [DisplayName("Forma Pgto.")]
        public string FormaPagamento { get; set; }

        [DisplayName("Cond. Pgto.")]
        public string CondicaoPagamento { get; set; }

        [DisplayName("Usu. Criador")]
        public string UsuarioCriador { get; set; }

        [DisplayName("Usu. Atual.")]
        public string UsuarioAtualizador { get; set; }

        [DisplayName("Situação")]
        public int StatusContratoId { get; set; }

        [DisplayName("Situação")]
        public string StatusContrato { get; set; }

        [DisplayName("Obs.")]
        public string Observacao { get; set; }

        [DisplayName("Descrição Produto")]
        [Required]
        public string Descricao { get; set; }

        [DisplayName("Categoria Produto")]
        [Required]
        public string Categoria { get; set; }

        [DisplayName("Espécie Produto")]
        [Required]
        public string Especie { get; set; }

        public string Produtos { get; set; }

        public string Empresa { get; set; }

        public ICollection<TituloViewModel> Titulos { get; set; }

        public ICollection<ContratoEventoViewModel> ContratoEventos { get; set; }

        public bool PermiteDownload { get; set; }

        public bool PermiteEnviarEmail { get; set; }

        public bool PermiteCartaCancelamento { get; set; }
    }
}