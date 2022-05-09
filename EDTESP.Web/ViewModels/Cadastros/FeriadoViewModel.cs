using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EDTESP.Web.ViewModels.Cadastros
{
    public class FeriadoViewModel
    {
        [DisplayName("Cód.")]
        public int FeriadoId { get; set; }

        [DisplayName("Data")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Data { get; set; }

        [DisplayName("Descrição")]
        public string Descricao { get; set; }
    }
}