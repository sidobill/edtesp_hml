using System.ComponentModel.DataAnnotations;

namespace EDTESP.Domain.Entities
{
    public class Cargo
    {
        [Key]
        public int CargoId { get; set; }

        public string Descricao { get; set; }

        public bool Removido { get; set; }
    }
}