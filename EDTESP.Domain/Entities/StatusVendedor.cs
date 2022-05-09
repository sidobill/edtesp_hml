using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class StatusVendedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int StatusVendedorId { get; set; }
        
        public string Descricao { get; set; }
    }
}