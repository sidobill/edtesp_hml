using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.ViewModels.Cadastros
{
    public class UsuarioViewModel
    {
        [DisplayName("Cód.")]
        public int UsuarioId { get; set; }

        [DisplayName("Grupo")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Informe o Grupo")]
        public int GrupoUsuarioId { get; set; }

        [DisplayName("Login")]
        [Required]
        public string Login { get; set; }

        [DisplayName("Senha")]
        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [DisplayName("Nome")]
        [Required]
        public string Nome { get; set; }

        [DisplayName("Sobrenome*")]
        public string Sobrenome { get; set; }

        [DisplayName("E-mail")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DisplayName("Dt. Cad.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DataCadastro { get; set; }

        [DisplayName("StatusVendedor")]
        [Range(0, int.MaxValue, ErrorMessage = "Informe o StatusVendedor")]
        public int StatusId { get; set; }

        [DisplayName("Dt. Atua.")]
        public DateTime? DataAtualizacao { get; set; }
        
        [DisplayName("Grupo")]
        public string GrupoUsuario { get; set; }
        
        [DisplayName("Status")]
        public string Status { get; set; }
    }

    public class EditarUsuarioViewModel
    {
        [DisplayName("Cód.")]        
        public int UsuarioId { get; set; }

        [DisplayName("Grupo")]
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Informe o Grupo")]
        public int GrupoUsuarioId { get; set; }

        [DisplayName("Login")]
        public string Login { get; set; }
        
        [DisplayName("Nome")]
        [Required]
        public string Nome { get; set; }

        [DisplayName("Sobrenome*")]
        public string Sobrenome { get; set; }

        [DisplayName("E-mail")]
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        
        [DisplayName("Status")]
        [Range(0, int.MaxValue, ErrorMessage = "Informe o StatusVendedor")]
        public int StatusId { get; set; }
    }
}