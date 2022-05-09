using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class VendedorAlcada
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int VendedorAlcadaId { get; set; }

        public string Descricao { get; set; }
    }
}