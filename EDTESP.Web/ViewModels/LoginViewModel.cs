using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

    }
}