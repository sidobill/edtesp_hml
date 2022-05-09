using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.ViewModels.Financeiro
{
    public class RemessaViewModel
    {
        [DisplayName("Cód.")]
        public int RemessaId { get; set; }

        [DisplayName("Criado Em")]
        public DateTime DataCriacao { get; set; }
        
        [DisplayName("Arquivo")]
        public string NomeTxt { get; set; }
        
        [DisplayName("Já Baixada?")]
        public string BaixadaStr { get; set; }

        [DisplayName("Modalidade")]
        public string ParametroBanco { get; set; }

        [DisplayName("Criado por")]
        public string UsuarioCriador { get; set; }
    }
}