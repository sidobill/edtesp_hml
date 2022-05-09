using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.ViewModels.Cadastros
{
    public class SetorViewModel
    {
        [DisplayName("Cód.")]
        public int SetorId { get; set; }

        [DisplayName("Descrição")]
        [Required]
        [MaxLength(150)]
        public string Descricao { get; set; }
    }
}