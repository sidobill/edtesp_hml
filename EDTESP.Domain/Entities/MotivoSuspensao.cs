using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDTESP.Domain.Entities
{
    public class MotivoSuspensao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int MotivoSuspensaoId { get; set; }

        public string Descricao { get; set; }
    }
}