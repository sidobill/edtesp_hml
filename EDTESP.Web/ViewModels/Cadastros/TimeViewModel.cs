using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.ViewModels.Cadastros
{
    public class TimeViewModel
    {
        [DisplayName("Cód.")]
        public int TimeId { get; set; }

        [DisplayName("Descrição")]
        [Required]
        [MaxLength(150)]
        public string Descricao { get; set; }
    }
}