using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.ViewModels.Cadastros
{
    public class CargoViewModel
    {
        [DisplayName("Cód.")]
        public int CargoId { get; set; }

        [DisplayName("Descrição")]
        [Required]
        [MaxLength(150)]
        public string Descricao { get; set; }
    }
}