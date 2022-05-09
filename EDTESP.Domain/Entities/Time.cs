using System.ComponentModel.DataAnnotations;

namespace EDTESP.Domain.Entities
{
    public class Time
    {
        [Key]
        public int TimeId { get; set; }

        public string Descricao { get; set; }

        public bool Removido { get; set; }
    }
}