using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities.Edtesp
{
    [Table("classificados")]
    public class ClassificadosEdtesp
    {
        [Key]
        public int Id { get; set; }

        public int Codigo { get; set; }
        
        public string Categoria { get; set; }

        public int Quantidade { get; set; }
    }
}