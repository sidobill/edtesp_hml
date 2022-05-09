using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.ViewModels.Cadastros
{
    public class ProdutoViewModel
    {
        [DisplayName("Cód.")]
        public int ProdutoId { get; set; }

        [DisplayName("Descrição")]
        [Required]
        public string Descricao { get; set; }

        [DisplayName("Preço")]
        [Required]
        [DataType(DataType.Currency)]
        public decimal PrecoBase { get; set; }

        [DisplayName("Espécie")]
        [Required]
        public string Especie { get; set; }

        [DisplayName("Categoria")]
        [Required]
        public string Categoria { get; set; }

        public DateTime DataCadastro { get; set; }

        public DateTime UltimaAtualizacao { get; set; }

        public string UsuarioCriador { get; set; }

        public string UsuarioAtualizador { get; set; }
    }
}