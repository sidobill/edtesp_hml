using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.ViewModels.Cadastros
{
    public class FormaPagamentoViewModel
    {
        [DisplayName("Cód")]
        public int FormaPagamentoId { get; set; }

        [DisplayName("Descrição")]
        [Required]
        [MaxLength(150,ErrorMessage = "Máx 150 caracteres")]
        public string Descricao { get; set; }

        [DisplayName("Gera Boleto?")]
        public bool GeraBoleto { get; set; }
        
        [DisplayName("Dt. Cadastro")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataCadastro { get; set; }
        
        [DisplayName("Ult.Atual.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime? UltimaAtualizacao { get; set; }

        [DisplayName("Usu. Criador")]
        public string UsuarioCriador { get; set; }

        [DisplayName("Atual. por")]
        public string UsuarioAtualizador { get; set; }
    }
}