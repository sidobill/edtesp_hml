using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.ViewModels.Cadastros
{
    public class GrupoUsuarioViewModel
    {
        [DisplayName("Cód.")]
        public int GrupoUsuarioId { get; set; }

        [DisplayName("Descrição")]
        [Required]
        public string Descricao { get; set; }

        [DisplayName("Dt. Cadastro")]
        public DateTime DataCadastro { get; set; }

        [DisplayName("Criado por")]
        public string UsuarioCriador { get; set; }
    }
}