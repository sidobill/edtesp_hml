using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace EDTESP.Web.ViewModels.Cadastros
{
    public class CondicaoPagamentoViewModel
    {
        [DisplayName("Cód.")]
        public int CondicaoPagamentoId { get; set; }

        [DisplayName("Modelo")]
        [Required]
        //[RegularExpression("([0-9]+,{0,1})+", ErrorMessage = "Modelo inválido")]
        [Remote("ValidarModelo", "cadastros",ErrorMessage = "Modelo inválido")]
        public string Modelo { get; set; }

        [DisplayName("Descrição")]
        [Required]
        public string Descricao { get; set; }

        [DisplayName("Dt. Criação")]
        public DateTime DataCadastro { get; set; }

        [DisplayName("Ult. Atual.")]
        public DateTime? DataAtualizacao { get; set; }

        [DisplayName("Criado por")]
        public string UsuarioCriador { get; set; }

        [DisplayName("Atual. por")]
        public string UsuarioAtualizador { get; set; }

        [DisplayName("Usado na 1° Parcela")]
        public bool Usado1Parc { get; set; }

        [DisplayName("Usada em")]
        [Required]
        public int[] FormasPgto { get; set; }

        public string Formas { get; set; }
    }
}