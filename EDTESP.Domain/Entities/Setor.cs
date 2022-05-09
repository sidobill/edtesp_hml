using System.ComponentModel.DataAnnotations;

namespace EDTESP.Domain.Entities
{
    public class Setor
    {
        [Key]
        public int SetorId { get; set; }

        public string Descricao { get; set; }

        public bool Removido { get; set; }
    }
}